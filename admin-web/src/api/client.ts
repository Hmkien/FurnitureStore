import axios, {
  AxiosError,
  AxiosRequestConfig,
  InternalAxiosRequestConfig,
} from 'axios'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:3001'

const ACCESS_KEY = 'fs_access_token'
const REFRESH_KEY = 'fs_refresh_token'

export const tokenStore = {
  get access() {
    return localStorage.getItem(ACCESS_KEY)
  },
  get refresh() {
    return localStorage.getItem(REFRESH_KEY)
  },
  set(access: string, refresh: string) {
    localStorage.setItem(ACCESS_KEY, access)
    localStorage.setItem(REFRESH_KEY, refresh)
  },
  clear() {
    localStorage.removeItem(ACCESS_KEY)
    localStorage.removeItem(REFRESH_KEY)
  },
}

export const api = axios.create({
  baseURL: `${API_URL}/api`,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = tokenStore.access
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ===== Refresh token: hàng đợi để tránh refresh nhiều lần khi gặp 401 đồng thời =====
let isRefreshing = false
let waiters: Array<(token: string | null) => void> = []

function onRefreshed(token: string | null) {
  waiters.forEach((cb) => cb(token))
  waiters = []
}

async function refreshAccessToken(): Promise<string | null> {
  const refresh = tokenStore.refresh
  if (!refresh) return null
  try {
    const res = await axios.post(`${API_URL}/api/Auth/refresh-token`, {
      refreshToken: refresh,
    })
    const { accessToken, refreshToken } = res.data
    tokenStore.set(accessToken, refreshToken)
    return accessToken
  } catch {
    return null
  }
}

api.interceptors.response.use(
  (res) => res,
  async (error: AxiosError) => {
    const original = error.config as AxiosRequestConfig & { _retry?: boolean }
    const status = error.response?.status
    const url = original?.url ?? ''

    const isAuthCall = url.includes('/Auth/login') || url.includes('/Auth/refresh-token')

    if (status === 401 && original && !original._retry && !isAuthCall) {
      original._retry = true

      if (isRefreshing) {
        const token = await new Promise<string | null>((resolve) => waiters.push(resolve))
        if (!token) return Promise.reject(error)
        original.headers = { ...original.headers, Authorization: `Bearer ${token}` }
        return api(original)
      }

      isRefreshing = true
      const token = await refreshAccessToken()
      isRefreshing = false
      onRefreshed(token)

      if (!token) {
        tokenStore.clear()
        // Tôn trọng base path (vd: /quan-tri) khi điều hướng về trang đăng nhập.
        const loginPath = import.meta.env.BASE_URL.replace(/\/$/, '') + '/login'
        if (!location.pathname.startsWith(loginPath)) {
          location.href = loginPath
        }
        return Promise.reject(error)
      }

      original.headers = { ...original.headers, Authorization: `Bearer ${token}` }
      return api(original)
    }

    return Promise.reject(error)
  },
)

/** Lấy message lỗi thân thiện từ response { error } của backend. */
export function getErrorMessage(error: unknown, fallback = 'Có lỗi xảy ra'): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { error?: string; title?: string } | undefined
    return data?.error ?? data?.title ?? error.message ?? fallback
  }
  return fallback
}

export { API_URL }
