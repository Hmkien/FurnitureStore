import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import AuthShell from '../components/AuthShell'

export default function RegisterPage() {
  const { register } = useAuth()
  const navigate = useNavigate()
  const toast = useToast()
  const [form, setForm] = useState({ userName: '', email: '', password: '' })
  const [loading, setLoading] = useState(false)

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await register(form.userName, form.email, form.password)
      toast.success('Đăng ký thành công')
      navigate('/')
    } catch (err) {
      toast.error(getErrorMessage(err, 'Đăng ký thất bại'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthShell title="Tạo tài khoản" subtitle="Gia nhập Furnitura để mua sắm dễ dàng hơn">
      <form onSubmit={submit} className="space-y-4">
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Tên đăng nhập</label>
          <input className="field" value={form.userName} onChange={(e) => setForm({ ...form, userName: e.target.value })} required autoFocus />
        </div>
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Email</label>
          <input type="email" className="field" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
        </div>
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Mật khẩu</label>
          <input type="password" className="field" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} required />
        </div>
        <button type="submit" disabled={loading} className="btn-dark w-full">
          {loading ? 'Đang tạo...' : 'Đăng ký'}
        </button>
      </form>
      <p className="mt-6 text-center text-sm text-ink-muted">
        Đã có tài khoản? <Link to="/login" className="font-medium text-clay-600 hover:underline">Đăng nhập</Link>
      </p>
    </AuthShell>
  )
}
