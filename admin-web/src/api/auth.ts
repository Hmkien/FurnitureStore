import { api, tokenStore } from './client'
import type { AuthResponse, CurrentUser } from '../types'

export async function login(username: string, password: string): Promise<AuthResponse> {
  const res = await api.post<AuthResponse>('/Auth/login', { username, password })
  tokenStore.set(res.data.accessToken, res.data.refreshToken)
  return res.data
}

export async function fetchCurrentUser(): Promise<CurrentUser> {
  const res = await api.get<CurrentUser>('/User/current')
  return res.data
}

export async function logout(): Promise<void> {
  try {
    await api.delete('/Auth/logout')
  } catch {
    // bỏ qua lỗi mạng khi đăng xuất
  }
  tokenStore.clear()
}
