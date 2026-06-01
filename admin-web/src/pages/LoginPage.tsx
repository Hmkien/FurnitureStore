import { useState, useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { CheckCircle2, Loader2, Sofa } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useAuth } from '@/context/AuthContext'
import { useToast } from '@/components/Toast'
import { getErrorMessage } from '@/api/client'

const features = [
  'Quản lý sản phẩm, biến thể & tồn kho theo thời gian thực',
  'Xử lý đơn hàng, thanh toán VNPAY / Momo',
  'Khuyến mãi, hạng thành viên & báo cáo doanh thu',
]

export default function LoginPage() {
  const { login, user } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const toast = useToast()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)

  const from = (location.state as { from?: { pathname: string } })?.from?.pathname ?? '/'

  useEffect(() => {
    if (user) navigate(from, { replace: true })
  }, [user, from, navigate])

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await login(username, password)
      toast.success('Đăng nhập thành công')
      navigate(from, { replace: true })
    } catch (err) {
      toast.error(getErrorMessage(err, 'Đăng nhập thất bại'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen bg-background">
      {/* Brand panel */}
      <div className="relative hidden flex-1 flex-col justify-between overflow-hidden bg-zinc-950 p-12 text-zinc-100 lg:flex">
        <div className="absolute -left-24 -top-24 h-72 w-72 rounded-full bg-primary/30 blur-3xl" />
        <div className="absolute bottom-0 right-0 h-72 w-72 rounded-full bg-indigo-500/20 blur-3xl" />

        <div className="relative flex items-center gap-3">
          <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-primary text-primary-foreground">
            <Sofa className="h-6 w-6" />
          </span>
          <span className="text-xl font-bold tracking-tight">Furnitura</span>
        </div>

        <div className="relative space-y-6">
          <h1 className="text-4xl font-bold leading-tight">
            Bảng điều khiển
            <br />
            quản trị nội thất
          </h1>
          <p className="max-w-md text-base leading-relaxed text-zinc-400">
            Quản lý toàn bộ cửa hàng nội thất của bạn ở một nơi: sản phẩm, đơn hàng, kho, khuyến mãi và báo cáo.
          </p>
          <div className="space-y-3">
            {features.map((f) => (
              <div key={f} className="flex items-center gap-3 text-sm text-zinc-300">
                <CheckCircle2 className="h-5 w-5 flex-none text-primary" />
                <span>{f}</span>
              </div>
            ))}
          </div>
        </div>

        <p className="relative text-sm text-zinc-500">© {new Date().getFullYear()} Furnitura. Hệ thống quản trị thương mại điện tử.</p>
      </div>

      {/* Form panel */}
      <div className="flex w-full items-center justify-center px-6 py-12 lg:w-[480px]">
        <form onSubmit={submit} className="w-full max-w-sm space-y-6">
          <div className="space-y-2 text-center lg:text-left">
            <div className="mb-2 flex justify-center lg:hidden">
              <span className="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground">
                <Sofa className="h-6 w-6" />
              </span>
            </div>
            <h2 className="text-3xl font-bold tracking-tight">Chào mừng trở lại 👋</h2>
            <p className="text-muted-foreground">Đăng nhập vào tài khoản quản trị của bạn để tiếp tục.</p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="u">Tên đăng nhập</Label>
            <Input id="u" value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Nhập tên đăng nhập" autoFocus className="h-11" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="p">Mật khẩu</Label>
            <Input id="p" type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Nhập mật khẩu" className="h-11" />
          </div>

          <Button type="submit" className="h-11 w-full text-base" disabled={loading}>
            {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            Đăng nhập
          </Button>

          <p className="text-center text-sm text-muted-foreground">
            Chưa có tài khoản quản trị? Đăng ký <span className="font-medium text-foreground">spadmin</span> qua API.
          </p>
        </form>
      </div>
    </div>
  )
}
