import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { tokenStore, cartToken } from '../api/client'
import { authApi, cartApi } from '../api/services'
import type { CurrentUser } from '../types'

/** Gộp giỏ khách (X-Cart-Token) vào giỏ user sau khi đăng nhập, rồi bỏ token khách. */
async function mergeGuestCart() {
  try {
    await cartApi.merge(cartToken.get())
    cartToken.clear()
  } catch {
    /* giỏ khách trống hoặc gộp lỗi — bỏ qua */
  }
}

interface AuthState {
  user: CurrentUser | null
  loading: boolean
  login: (u: string, p: string) => Promise<void>
  register: (u: string, e: string, p: string) => Promise<void>
  logout: () => Promise<void>
  refresh: () => Promise<void>
}

const Ctx = createContext<AuthState | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let active = true
    ;(async () => {
      if (!tokenStore.access) {
        setLoading(false)
        return
      }
      try {
        const me = await authApi.me()
        if (active) setUser(me)
      } catch {
        tokenStore.clear()
      } finally {
        if (active) setLoading(false)
      }
    })()
    return () => {
      active = false
    }
  }, [])

  const login = async (u: string, p: string) => {
    await authApi.login(u, p)
    await mergeGuestCart()
    setUser(await authApi.me())
  }
  const register = async (u: string, e: string, p: string) => {
    await authApi.register(u, e, p)
    await authApi.login(u, p)
    await mergeGuestCart()
    setUser(await authApi.me())
  }
  const logout = async () => {
    await authApi.logout()
    setUser(null)
  }
  const refresh = async () => {
    try {
      setUser(await authApi.me())
    } catch {
      /* giữ nguyên user hiện tại nếu refresh lỗi */
    }
  }

  return <Ctx.Provider value={{ user, loading, login, register, logout, refresh }}>{children}</Ctx.Provider>
}

export function useAuth() {
  const c = useContext(Ctx)
  if (!c) throw new Error('useAuth must be inside AuthProvider')
  return c
}
