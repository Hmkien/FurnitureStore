import { useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { ShoppingBag, User, LogOut, Menu, X, Search, Package } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { useCart } from '../context/CartContext'

const nav = [
  { to: '/', label: 'Trang chủ', end: true },
  { to: '/products', label: 'Sản phẩm' },
  { to: '/tin-tuc', label: 'Tin tức' },
  { to: '/ve-chung-toi', label: 'Về chúng tôi' },
]

export default function Layout() {
  const { user, logout } = useAuth()
  const { count } = useCart()
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)
  const [q, setQ] = useState('')

  const submitSearch = (e: React.FormEvent) => {
    e.preventDefault()
    navigate(q.trim() ? `/products?keyword=${encodeURIComponent(q.trim())}` : '/products')
    setOpen(false)
  }

  const linkCls = ({ isActive }: { isActive: boolean }) =>
    `relative text-sm font-medium transition-colors after:absolute after:-bottom-1.5 after:left-0 after:h-px after:bg-clay-600 after:transition-all ${
      isActive ? 'text-ink after:w-full' : 'text-ink-soft after:w-0 hover:text-ink hover:after:w-full'
    }`

  return (
    <div className="flex min-h-screen flex-col bg-cream">
      {/* Thanh thông báo */}
      <div className="bg-ink text-center text-[12px] tracking-wide text-paper/85">
        <div className="u-container py-2">Miễn phí vận chuyển cho đơn từ 5.000.000₫ · Bảo hành 24 tháng</div>
      </div>

      {/* Header */}
      <header className="sticky top-0 z-40 border-b border-line/80 bg-cream/85 backdrop-blur-md">
        <div className="u-container flex h-[72px] items-center gap-6">
          <button className="lg:hidden" onClick={() => setOpen(true)} aria-label="Mở menu">
            <Menu className="h-6 w-6" />
          </button>

          <Link to="/" className="flex items-baseline gap-1.5">
            <span className="font-display text-2xl font-semibold tracking-tight text-ink">Furnitura</span>
            <span className="h-1.5 w-1.5 rounded-full bg-clay-500" />
          </Link>

          <nav className="ml-4 hidden items-center gap-8 lg:flex">
            {nav.map((n) => (
              <NavLink key={n.to} to={n.to} end={n.end} className={linkCls}>
                {n.label}
              </NavLink>
            ))}
          </nav>

          <div className="ml-auto flex items-center gap-3 sm:gap-4">
            <form onSubmit={submitSearch} className="hidden items-center md:flex">
              <div className="flex items-center rounded-full border border-line bg-paper/70 px-3.5 py-2 focus-within:border-clay-400">
                <Search className="h-4 w-4 text-ink-muted" />
                <input
                  value={q}
                  onChange={(e) => setQ(e.target.value)}
                  placeholder="Tìm nội thất..."
                  className="w-36 bg-transparent px-2 text-sm outline-none placeholder:text-ink-muted xl:w-48"
                />
              </div>
            </form>

            <Link to="/cart" className="relative grid h-10 w-10 place-items-center rounded-full hover:bg-ink/5" aria-label="Giỏ hàng">
              <ShoppingBag className="h-5 w-5" />
              {count > 0 && (
                <span className="absolute -right-0.5 -top-0.5 grid h-5 min-w-5 place-items-center rounded-full bg-clay-600 px-1 text-[11px] font-semibold text-paper">
                  {count}
                </span>
              )}
            </Link>

            {user ? (
              <div className="hidden items-center gap-2 sm:flex">
                <Link to="/tai-khoan/don-hang" className="grid h-10 w-10 place-items-center rounded-full hover:bg-ink/5" title="Đơn của tôi">
                  <Package className="h-5 w-5" />
                </Link>
                <Link to="/tai-khoan" className="hidden items-center gap-1.5 text-sm text-ink-soft hover:text-ink xl:flex" title="Tài khoản">
                  <User className="h-4 w-4" /> {user.fullName || user.userName}
                </Link>
                <button onClick={() => logout().then(() => navigate('/'))} className="grid h-10 w-10 place-items-center rounded-full text-ink-muted hover:bg-ink/5 hover:text-ink" title="Đăng xuất">
                  <LogOut className="h-5 w-5" />
                </button>
              </div>
            ) : (
              <Link to="/login" className="hidden rounded-full bg-ink px-5 py-2.5 text-sm font-medium text-paper hover:bg-black sm:inline-flex">
                Đăng nhập
              </Link>
            )}
          </div>
        </div>
      </header>

      {/* Menu di động */}
      {open && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="absolute inset-0 bg-ink/40 backdrop-blur-sm" onClick={() => setOpen(false)} />
          <div className="absolute left-0 top-0 h-full w-80 max-w-[85%] animate-fade-in bg-cream p-6 shadow-lift">
            <div className="flex items-center justify-between">
              <span className="font-display text-xl font-semibold">Furnitura</span>
              <button onClick={() => setOpen(false)}><X className="h-6 w-6" /></button>
            </div>
            <form onSubmit={submitSearch} className="mt-6">
              <div className="flex items-center rounded-full border border-line bg-paper px-3.5 py-2.5">
                <Search className="h-4 w-4 text-ink-muted" />
                <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Tìm nội thất..." className="w-full bg-transparent px-2 text-sm outline-none" />
              </div>
            </form>
            <nav className="mt-6 flex flex-col gap-1">
              {nav.map((n) => (
                <NavLink key={n.to} to={n.to} end={n.end} onClick={() => setOpen(false)} className="rounded-xl px-3 py-3 text-base font-medium hover:bg-ink/5">
                  {n.label}
                </NavLink>
              ))}
              {user && (
                <>
                  <NavLink to="/tai-khoan" end onClick={() => setOpen(false)} className="rounded-xl px-3 py-3 text-base font-medium hover:bg-ink/5">
                    Tài khoản
                  </NavLink>
                  <NavLink to="/tai-khoan/don-hang" onClick={() => setOpen(false)} className="rounded-xl px-3 py-3 text-base font-medium hover:bg-ink/5">
                    Đơn hàng của tôi
                  </NavLink>
                </>
              )}
            </nav>
            <div className="mt-6 border-t border-line pt-6">
              {user ? (
                <button onClick={() => { logout().then(() => navigate('/')); setOpen(false) }} className="btn-outline w-full">
                  <LogOut className="h-4 w-4" /> Đăng xuất
                </button>
              ) : (
                <Link to="/login" onClick={() => setOpen(false)} className="btn-dark w-full">Đăng nhập</Link>
              )}
            </div>
          </div>
        </div>
      )}

      <main className="flex-1">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="mt-20 border-t border-line bg-paper">
        <div className="u-container grid grid-cols-2 gap-10 py-14 md:grid-cols-4">
          <div className="col-span-2 md:col-span-1">
            <div className="flex items-baseline gap-1.5">
              <span className="font-display text-2xl font-semibold text-ink">Furnitura</span>
              <span className="h-1.5 w-1.5 rounded-full bg-clay-500" />
            </div>
            <p className="mt-3 max-w-xs text-sm leading-relaxed text-ink-soft">
              Nội thất thiết kế lấy cảm hứng từ thiên nhiên — bền vững, tinh tế, dành cho tổ ấm của bạn.
            </p>
          </div>
          <FooterCol title="Khám phá" links={[['Sản phẩm', '/products'], ['Giỏ hàng', '/cart'], ['Đơn của tôi', '/orders']]} />
          <FooterCol title="Hỗ trợ" links={[['Chính sách đổi trả', '/products'], ['Bảo hành 24 tháng', '/products'], ['Vận chuyển', '/products']]} />
          <div>
            <h4 className="eyebrow">Liên hệ</h4>
            <ul className="mt-4 space-y-2 text-sm text-ink-soft">
              <li>1900 0000</li>
              <li>hello@furnitura.vn</li>
              <li>TP. Hồ Chí Minh</li>
            </ul>
          </div>
        </div>
        <div className="border-t border-line">
          <div className="u-container flex flex-col items-center justify-between gap-2 py-5 text-xs text-ink-muted sm:flex-row">
            <span>© {new Date().getFullYear()} Furnitura. Bảo lưu mọi quyền.</span>
            <span>Thanh toán an toàn qua VNPAY · Momo · COD</span>
          </div>
        </div>
      </footer>
    </div>
  )
}

function FooterCol({ title, links }: { title: string; links: [string, string][] }) {
  return (
    <div>
      <h4 className="eyebrow">{title}</h4>
      <ul className="mt-4 space-y-2 text-sm">
        {links.map(([label, to]) => (
          <li key={label}>
            <Link to={to} className="text-ink-soft transition hover:text-clay-600">{label}</Link>
          </li>
        ))}
      </ul>
    </div>
  )
}
