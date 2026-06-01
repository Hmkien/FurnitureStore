import axios, { AxiosError, AxiosRequestConfig, InternalAxiosRequestConfig } from 'axios'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:3001'
const ACCESS_KEY = 'sf_access'
const REFRESH_KEY = 'sf_refresh'
const CART_KEY = 'sf_cart_token'

/** Token giỏ hàng cho KHÁCH (chưa đăng nhập) — gửi qua header X-Cart-Token. */
export const cartToken = {
  get(): string {
    let t = localStorage.getItem(CART_KEY)
    if (!t) {
      t =
        typeof crypto !== 'undefined' && 'randomUUID' in crypto
          ? crypto.randomUUID()
          : `cart-${Date.now()}-${Math.random().toString(36).slice(2)}`
      localStorage.setItem(CART_KEY, t)
    }
    return t
  },
  clear() {
    localStorage.removeItem(CART_KEY)
  },
}

export const tokenStore = {
  get access() {
    return localStorage.getItem(ACCESS_KEY)
  },
  get refresh() {
    return localStorage.getItem(REFRESH_KEY)
  },
  set(a: string, r: string) {
    localStorage.setItem(ACCESS_KEY, a)
    localStorage.setItem(REFRESH_KEY, r)
  },
  clear() {
    localStorage.removeItem(ACCESS_KEY)
    localStorage.removeItem(REFRESH_KEY)
  },
}

export const api = axios.create({ baseURL: `${API_URL}/api`, headers: { 'Content-Type': 'application/json' } })

api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const t = tokenStore.access
  if (t) config.headers.Authorization = `Bearer ${t}`
  // Luôn kèm token giỏ khách; backend bỏ qua khi đã đăng nhập.
  config.headers['X-Cart-Token'] = cartToken.get()
  return config
})

let refreshing = false
let waiters: Array<(t: string | null) => void> = []

async function doRefresh(): Promise<string | null> {
  const r = tokenStore.refresh
  if (!r) return null
  try {
    const res = await axios.post(`${API_URL}/api/Auth/refresh-token`, { refreshToken: r })
    tokenStore.set(res.data.accessToken, res.data.refreshToken)
    return res.data.accessToken
  } catch {
    return null
  }
}

api.interceptors.response.use(
  (res) => res,
  async (error: AxiosError) => {
    const original = error.config as AxiosRequestConfig & { _retry?: boolean }
    const url = original?.url ?? ''
    const isAuth = url.includes('/Auth/login') || url.includes('/Auth/refresh-token')
    if (error.response?.status === 401 && original && !original._retry && !isAuth) {
      original._retry = true
      if (refreshing) {
        const t = await new Promise<string | null>((r) => waiters.push(r))
        if (!t) return Promise.reject(error)
        original.headers = { ...original.headers, Authorization: `Bearer ${t}` }
        return api(original)
      }
      refreshing = true
      const t = await doRefresh()
      refreshing = false
      waiters.forEach((cb) => cb(t))
      waiters = []
      if (!t) {
        tokenStore.clear()
        return Promise.reject(error)
      }
      original.headers = { ...original.headers, Authorization: `Bearer ${t}` }
      return api(original)
    }
    return Promise.reject(error)
  },
)

export function getErrorMessage(error: unknown, fallback = 'Có lỗi xảy ra'): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { error?: string; title?: string } | undefined
    return data?.error ?? data?.title ?? error.message ?? fallback
  }
  return fallback
}

export { API_URL }
