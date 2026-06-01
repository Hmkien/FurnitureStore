import { useEffect, useState } from 'react'
import { userApi } from '../../api/services'
import { useAuth } from '../../context/AuthContext'
import { useToast } from '../../context/Toast'
import { getErrorMessage } from '../../api/client'

const toDateInput = (iso?: string) => (iso ? new Date(iso).toISOString().slice(0, 10) : '')

export default function ProfilePage() {
  const { user, refresh } = useAuth()
  const toast = useToast()
  const [form, setForm] = useState({ email: '', lastName: '', firstName: '', phoneNumber: '', address: '', birthday: '' })
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!user) return
    setForm({
      email: user.email ?? '',
      lastName: user.lastName ?? '',
      firstName: user.firstName ?? '',
      phoneNumber: user.phoneNumber ?? '',
      address: user.address ?? '',
      birthday: toDateInput(user.birthDay),
    })
  }, [user])

  const set = (k: keyof typeof form, v: string) => setForm((f) => ({ ...f, [k]: v }))

  const save = async () => {
    if (!user) return
    if (!form.email.trim()) return toast.error('Vui lòng nhập email')
    setSaving(true)
    try {
      await userApi.updateProfile(user.userId, form)
      await refresh()
      toast.success('Đã cập nhật hồ sơ')
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="surface p-6 sm:p-8">
      <h2 className="font-display text-2xl text-ink">Hồ sơ của tôi</h2>
      <p className="mt-1 text-sm text-ink-muted">Quản lý thông tin cá nhân của bạn.</p>

      <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field label="Họ" value={form.lastName} onChange={(v) => set('lastName', v)} />
        <Field label="Tên" value={form.firstName} onChange={(v) => set('firstName', v)} />
        <Field label="Email" type="email" value={form.email} onChange={(v) => set('email', v)} />
        <Field label="Số điện thoại" value={form.phoneNumber} onChange={(v) => set('phoneNumber', v)} />
        <Field label="Ngày sinh" type="date" value={form.birthday} onChange={(v) => set('birthday', v)} />
        <div className="sm:col-span-2">
          <Field label="Địa chỉ" value={form.address} onChange={(v) => set('address', v)} />
        </div>
      </div>

      <div className="mt-2 text-xs text-ink-muted">Tên đăng nhập: <span className="font-medium text-ink-soft">{user?.userName}</span> (không thể đổi)</div>

      <button onClick={save} disabled={saving} className="btn-dark mt-6">
        {saving ? 'Đang lưu...' : 'Lưu thay đổi'}
      </button>
    </div>
  )
}

function Field({ label, value, onChange, type = 'text' }: { label: string; value: string; onChange: (v: string) => void; type?: string }) {
  return (
    <div>
      <label className="mb-1.5 block text-sm font-medium text-ink-soft">{label}</label>
      <input type={type} className="field" value={value} onChange={(e) => onChange(e.target.value)} />
    </div>
  )
}
