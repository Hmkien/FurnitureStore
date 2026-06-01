import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import AuthShell from '../components/AuthShell'

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const toast = useToast()
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await login(username, password)
      toast.success('Đăng nhập thành công')
      navigate('/')
    } catch (err) {
      toast.error(getErrorMessage(err, 'Đăng nhập thất bại'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthShell title="Chào mừng trở lại" subtitle="Đăng nhập để tiếp tục mua sắm">
      <form onSubmit={submit} className="space-y-4">
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Tên đăng nhập</label>
          <input className="field" value={username} onChange={(e) => setUsername(e.target.value)} required autoFocus />
        </div>
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Mật khẩu</label>
          <input type="password" className="field" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </div>
        <button type="submit" disabled={loading} className="btn-dark w-full">
          {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
        </button>
      </form>
      <p className="mt-6 text-center text-sm text-ink-muted">
        Chưa có tài khoản? <Link to="/register" className="font-medium text-clay-600 hover:underline">Đăng ký</Link>
      </p>
    </AuthShell>
  )
}
