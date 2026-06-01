import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from 'react'
import { cartApi } from '../api/services'
import { useAuth } from './AuthContext'
import type { Cart } from '../types'

interface CartState {
  cart: Cart | null
  count: number
  reload: () => Promise<void>
  add: (variantId: string, qty: number) => Promise<void>
  update: (itemId: string, qty: number) => Promise<void>
  remove: (itemId: string) => Promise<void>
}

const Ctx = createContext<CartState | undefined>(undefined)

export function CartProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  const [cart, setCart] = useState<Cart | null>(null)

  // Tải giỏ cho cả khách (qua X-Cart-Token) lẫn user đã đăng nhập.
  const reload = useCallback(async () => {
    try {
      setCart(await cartApi.get())
    } catch {
      setCart(null)
    }
  }, [])

  useEffect(() => {
    reload()
  }, [user, reload])

  const add = async (variantId: string, qty: number) => setCart(await cartApi.add(variantId, qty))
  const update = async (itemId: string, qty: number) => setCart(await cartApi.update(itemId, qty))
  const remove = async (itemId: string) => setCart(await cartApi.remove(itemId))

  return <Ctx.Provider value={{ cart, count: cart?.totalItems ?? 0, reload, add, update, remove }}>{children}</Ctx.Provider>
}

export function useCart() {
  const c = useContext(Ctx)
  if (!c) throw new Error('useCart must be inside CartProvider')
  return c
}
