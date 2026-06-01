import { useState } from 'react'
import { userApi } from '../../api/services'
import { useToast } from '../../context/Toast'
import { getErrorMessage } from '../../api/client'

export default function ChangePasswordPage() {
  const toast = useToast()
  const [current, setCurrent] = useState('')
  const [next, setNext] = useState('')
  const [confirm, setConfirm] = useState('')
  const [saving, setSaving] = useState(false)

  const save = async () => {
    if (!current) return toast.error('Nhập mật khẩu hiện tại')
    if (next.length < 6) return toast.error('Mật khẩu mới phải từ 6 ký tự')
    if (next !== confirm) return toast.error('Mật khẩu xác nhận không khớp')
    setSaving(true)
    try {
      await userApi.changePassword(current, next)
      toast.success('Đổi mật khẩu thành công')
      setCurrent(''); setNext(''); setConfirm('')
    } catch (e) {
      toast.error(getErrorMessage(e, 'Mật khẩu hiện tại không đúng'))
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="surface max-w-lg p-6 sm:p-8">
      <h2 className="font-display text-2xl text-ink">Đổi mật khẩu</h2>
      <p className="mt-1 text-sm text-ink-muted">Để bảo mật, hãy dùng mật khẩu mạnh và không chia sẻ.</p>

      <div className="mt-6 space-y-4">
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Mật khẩu hiện tại</label>
          <input type="password" className="field" value={current} onChange={(e) => setCurrent(e.target.value)} />
        </div>
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Mật khẩu mới</label>
          <input type="password" className="field" value={next} onChange={(e) => setNext(e.target.value)} />
        </div>
        <div>
          <label className="mb-1.5 block text-sm font-medium text-ink-soft">Xác nhận mật khẩu mới</label>
          <input type="password" className="field" value={confirm} onChange={(e) => setConfirm(e.target.value)} />
        </div>
      </div>

      <button onClick={save} disabled={saving} className="btn-dark mt-6">
        {saving ? 'Đang lưu...' : 'Cập nhật mật khẩu'}
      </button>
    </div>
  )
}
