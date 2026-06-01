import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'

/** Khung đăng nhập / đăng ký dạng split-screen sang trọng. */
export default function AuthShell({ title, subtitle, children }: { title: string; subtitle: string; children: ReactNode }) {
  return (
    <div className="grid min-h-screen lg:grid-cols-2">
      {/* Bảng thương hiệu */}
      <div className="relative hidden flex-col justify-between overflow-hidden bg-ink p-12 text-paper lg:flex">
        <div className="absolute -right-24 top-10 h-80 w-80 rounded-full bg-clay-600/25 blur-3xl" />
        <div className="absolute -bottom-24 -left-10 h-80 w-80 rounded-full bg-clay-500/15 blur-3xl" />
        <Link to="/" className="relative flex items-baseline gap-1.5">
          <span className="font-display text-2xl font-semibold">Furnitura</span>
          <span className="h-1.5 w-1.5 rounded-full bg-clay-400" />
        </Link>
        <div className="relative">
          <h2 className="display text-4xl leading-tight">
            Không gian sống
            <br />
            bắt đầu từ <span className="italic text-clay-300">chi tiết</span>
          </h2>
          <p className="mt-4 max-w-sm text-sm text-paper/65">
            Nội thất thiết kế, vật liệu tự nhiên và sự tinh tế trong từng đường nét — dành riêng cho tổ ấm của bạn.
          </p>
        </div>
        <p className="relative text-xs text-paper/45">© {new Date().getFullYear()} Furnitura</p>
      </div>

      {/* Khung form */}
      <div className="flex items-center justify-center bg-cream p-6">
        <div className="w-full max-w-sm">
          <Link to="/" className="mb-8 flex items-baseline justify-center gap-1.5 lg:hidden">
            <span className="font-display text-2xl font-semibold text-ink">Furnitura</span>
            <span className="h-1.5 w-1.5 rounded-full bg-clay-500" />
          </Link>
          <h1 className="display text-3xl text-ink">{title}</h1>
          <p className="mb-7 mt-1.5 text-sm text-ink-muted">{subtitle}</p>
          {children}
        </div>
      </div>
    </div>
  )
}
