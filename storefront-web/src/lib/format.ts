export const currency = (v?: number) => (v ?? 0).toLocaleString('vi-VN', { maximumFractionDigits: 0 }) + '₫'
export const dateTime = (v?: string) => (v ? new Date(v).toLocaleString('vi-VN') : '')
export const dateOnly = (v?: string) => (v ? new Date(v).toLocaleDateString('vi-VN') : '')
export const img = (url?: string) => {
  if (!url) return ''
  if (url.startsWith('http')) return url
  return (import.meta.env.VITE_API_URL ?? 'http://localhost:3001') + url
}
