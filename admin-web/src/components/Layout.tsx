import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import {
  LayoutDashboard,
  ShoppingCart,
  Package,
  Tags,
  Boxes,
  Ticket,
  Crown,
  FileText,
  Image as ImageIcon,
  GalleryHorizontal,
  FolderOpen,
  LogOut,
  Search,
  User,
  Users,
  Shield,
  KeyRound,
  RefreshCcw,
  ShieldCheck,
} from 'lucide-react'
import { cn } from '@/lib/utils'
import { Input } from '@/components/ui/input'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { useAuth } from '@/context/AuthContext'
import NotificationBell from './NotificationBell'

interface MenuItem {
  to: string
  icon: typeof LayoutDashboard
  label: string
  end?: boolean
  /** Quyền cần có để thấy menu. Bỏ trống = luôn hiển thị. SuperUser thấy tất cả. */
  perm?: string
}

const groups: { label: string; items: MenuItem[] }[] = [
  {
    label: 'QUẢN LÝ',
    items: [
      { to: '/', icon: LayoutDashboard, label: 'Tổng quan', end: true },
      { to: '/orders', icon: ShoppingCart, label: 'Đơn hàng', perm: 'ORDER_VIEW' },
      { to: '/returns', icon: RefreshCcw, label: 'Đổi / Trả', perm: 'RETURN_MANAGE' },
      { to: '/warranty', icon: ShieldCheck, label: 'Bảo hành', perm: 'WARRANTY_MANAGE' },
      { to: '/products', icon: Package, label: 'Sản phẩm', perm: 'PRODUCT_VIEW' },
      { to: '/categories', icon: Tags, label: 'Danh mục', perm: 'CATEGORY_MANAGE' },
      { to: '/inventory', icon: Boxes, label: 'Kho hàng', perm: 'INVENTORY_MANAGE' },
      { to: '/coupons', icon: Ticket, label: 'Mã giảm giá', perm: 'COUPON_MANAGE' },
      { to: '/membership-tiers', icon: Crown, label: 'Hạng thành viên', perm: 'MEMBERSHIP_MANAGE' },
    ],
  },
  {
    label: 'NỘI DUNG',
    items: [
      { to: '/posts', icon: FileText, label: 'Tin bài', perm: 'CONTENT_MANAGE' },
      { to: '/banners', icon: ImageIcon, label: 'Banner', perm: 'CONTENT_MANAGE' },
      { to: '/slides', icon: GalleryHorizontal, label: 'Slide', perm: 'CONTENT_MANAGE' },
      { to: '/media', icon: FolderOpen, label: 'Kho media', perm: 'CONTENT_MANAGE' },
    ],
  },
  {
    label: 'HỆ THỐNG',
    items: [
      { to: '/users', icon: Users, label: 'Tài khoản', perm: 'USER_MANAGE' },
      { to: '/roles', icon: Shield, label: 'Vai trò', perm: 'ROLE_MANAGE' },
      { to: '/permissions', icon: KeyRound, label: 'Quyền', perm: 'PERMISSION_MANAGE' },
    ],
  },
]

export default function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate('/login', { replace: true })
  }

  const can = (perm?: string) => !perm || user?.isSuperUser || (user?.permissions?.includes(perm) ?? false)
  const visibleGroups = groups
    .map((g) => ({ ...g, items: g.items.filter((i) => can(i.perm)) }))
    .filter((g) => g.items.length > 0)

  return (
    <div className="flex min-h-screen bg-background">
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-64 flex-col border-r bg-card lg:flex">
        <div className="flex h-16 items-center gap-2.5 px-5">
          <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-lg text-primary-foreground">🪑</span>
          <span className="text-lg font-bold tracking-tight">Furnitura</span>
        </div>
        <nav className="flex-1 space-y-6 overflow-y-auto px-3 py-2">
          {visibleGroups.map((g) => (
            <div key={g.label}>
              <div className="px-3 pb-1 text-[11px] font-semibold uppercase tracking-wider text-muted-foreground">{g.label}</div>
              <div className="space-y-1">
                {g.items.map((item) => (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end={item.end}
                    className={({ isActive }) =>
                      cn(
                        'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                        isActive
                          ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                          : 'text-muted-foreground hover:bg-accent hover:text-foreground',
                      )
                    }
                  >
                    <item.icon className="h-[18px] w-[18px]" />
                    {item.label}
                  </NavLink>
                ))}
              </div>
            </div>
          ))}
        </nav>
      </aside>

      <div className="flex flex-1 flex-col lg:pl-64">
        <header className="sticky top-0 z-20 flex h-16 items-center gap-4 border-b bg-card/80 px-5 backdrop-blur">
          <div className="relative hidden w-full max-w-md sm:block">
            <Search className="absolute left-3 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input placeholder="Tìm kiếm hoặc gõ lệnh..." className="bg-muted/50 pl-9" />
          </div>
          <div className="ml-auto flex items-center gap-3">
            <NotificationBell />
            <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button className="flex items-center gap-2.5 rounded-lg p-1 hover:bg-accent">
                <Avatar className="h-9 w-9">
                  <AvatarFallback className="bg-primary text-primary-foreground">
                    <User className="h-4 w-4" />
                  </AvatarFallback>
                </Avatar>
                <div className="hidden text-left leading-tight sm:block">
                  <div className="text-sm font-semibold">{user?.fullName || user?.userName}</div>
                  <div className="text-xs text-muted-foreground">{user?.isSuperUser ? 'Quản trị cấp cao' : 'Nhân viên'}</div>
                </div>
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
              <DropdownMenuLabel>{user?.fullName || user?.userName}</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={handleLogout}>
                <LogOut className="mr-2 h-4 w-4" /> Đăng xuất
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
          </div>
        </header>

        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
