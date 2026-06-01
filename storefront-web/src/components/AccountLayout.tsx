import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { User, MapPin, KeyRound, Package, LogOut } from 'lucide-react'
import { useAuth } from '../context/AuthContext'

const items = [
  { to: '/tai-khoan', icon: User, label: 'Hồ sơ', end: true },
  { to: '/tai-khoan/dia-chi', icon: MapPin, label: 'Sổ địa chỉ' },
  { to: '/tai-khoan/don-hang', icon: Package, label: 'Đơn hàng của tôi' },
  { to: '/tai-khoan/doi-mat-khau', icon: KeyRound, label: 'Đổi mật khẩu' },
]

export default function AccountLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const linkCls = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-3 rounded-xl px-3.5 py-2.5 text-sm font-medium transition ${
      isActive ? 'bg-ink text-paper' : 'text-ink-soft hover:bg-ink/5 hover:text-ink'
    }`

  return (
    <div className="u-container py-10">
      <span className="eyebrow">Tài khoản</span>
      <h1 className="display mt-2 text-4xl text-ink">Xin chào, {user?.fullName || user?.userName}</h1>

      <div className="mt-8 grid gap-8 lg:grid-cols-[260px_1fr]">
        <aside className="h-fit lg:sticky lg:top-28">
          <div className="surface p-3">
            <div className="flex items-center gap-3 px-2 py-3">
              <span className="grid h-11 w-11 place-items-center rounded-full bg-clay-100 font-display text-lg text-clay-700">
                {(user?.fullName || user?.userName || 'U').charAt(0).toUpperCase()}
              </span>
              <div className="min-w-0">
                <div className="truncate text-sm font-semibold text-ink">{user?.fullName || user?.userName}</div>
                <div className="truncate text-xs text-ink-muted">{user?.email}</div>
              </div>
            </div>
            <nav className="mt-1 space-y-1">
              {items.map((i) => (
                <NavLink key={i.to} to={i.to} end={i.end} className={linkCls}>
                  <i.icon className="h-[18px] w-[18px]" /> {i.label}
                </NavLink>
              ))}
              <button
                onClick={() => logout().then(() => navigate('/'))}
                className="flex w-full items-center gap-3 rounded-xl px-3.5 py-2.5 text-sm font-medium text-ink-soft transition hover:bg-red-50 hover:text-red-600"
              >
                <LogOut className="h-[18px] w-[18px]" /> Đăng xuất
              </button>
            </nav>
          </div>
        </aside>

        <div className="min-w-0">
          <Outlet />
        </div>
      </div>
    </div>
  )
}
