import { useEffect, useState } from 'react'
import { Plus, MapPin, Pencil, Trash2, Check, Star } from 'lucide-react'
import { addressApi } from '../../api/services'
import { useToast } from '../../context/Toast'
import { getErrorMessage } from '../../api/client'
import type { Address } from '../../types'

const empty = { receiverName: '', receiverPhone: '', addressLine: '', label: '', isDefault: false }

export default function AddressesPage() {
  const toast = useToast()
  const [list, setList] = useState<Address[]>([])
  const [loading, setLoading] = useState(true)
  const [editing, setEditing] = useState<Address | null>(null)
  const [form, setForm] = useState<Omit<Address, 'id'>>(empty)
  const [showForm, setShowForm] = useState(false)
  const [saving, setSaving] = useState(false)

  const load = () => {
    setLoading(true)
    addressApi.list().then(setList).catch((e) => toast.error(getErrorMessage(e))).finally(() => setLoading(false))
  }
  useEffect(load, []) // eslint-disable-line react-hooks/exhaustive-deps

  const openCreate = () => {
    setEditing(null)
    setForm({ ...empty, isDefault: list.length === 0 })
    setShowForm(true)
  }
  const openEdit = (a: Address) => {
    setEditing(a)
    setForm({ receiverName: a.receiverName, receiverPhone: a.receiverPhone, addressLine: a.addressLine, label: a.label ?? '', isDefault: a.isDefault })
    setShowForm(true)
  }

  const save = async () => {
    if (!form.receiverName || !form.receiverPhone || !form.addressLine) return toast.error('Vui lòng nhập đủ tên, SĐT và địa chỉ')
    setSaving(true)
    try {
      if (editing) await addressApi.update(editing.id, form)
      else await addressApi.create(form)
      toast.success('Đã lưu địa chỉ')
      setShowForm(false)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  const act = async (fn: () => Promise<unknown>, ok: string) => {
    try {
      await fn()
      toast.success(ok)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="font-display text-2xl text-ink">Sổ địa chỉ</h2>
          <p className="mt-1 text-sm text-ink-muted">Quản lý nhiều địa chỉ giao hàng, chọn một địa chỉ mặc định.</p>
        </div>
        <button onClick={openCreate} className="btn-primary shrink-0"><Plus className="h-4 w-4" /> Thêm địa chỉ</button>
      </div>

      {showForm && (
        <div className="surface p-5">
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <input className="field" placeholder="Tên người nhận" value={form.receiverName} onChange={(e) => setForm({ ...form, receiverName: e.target.value })} />
            <input className="field" placeholder="Số điện thoại" value={form.receiverPhone} onChange={(e) => setForm({ ...form, receiverPhone: e.target.value })} />
            <input className="field sm:col-span-2" placeholder="Địa chỉ chi tiết (số nhà, đường, phường, quận, tỉnh)" value={form.addressLine} onChange={(e) => setForm({ ...form, addressLine: e.target.value })} />
            <input className="field" placeholder="Nhãn (Nhà riêng, Công ty...)" value={form.label} onChange={(e) => setForm({ ...form, label: e.target.value })} />
            <label className="flex items-center gap-2 text-sm text-ink-soft">
              <input type="checkbox" className="h-4 w-4 accent-clay-600" checked={form.isDefault} onChange={(e) => setForm({ ...form, isDefault: e.target.checked })} />
              Đặt làm địa chỉ mặc định
            </label>
          </div>
          <div className="mt-4 flex gap-2">
            <button onClick={save} disabled={saving} className="btn-dark">{saving ? 'Đang lưu...' : 'Lưu địa chỉ'}</button>
            <button onClick={() => setShowForm(false)} className="btn-ghost">Hủy</button>
          </div>
        </div>
      )}

      {loading ? (
        <p className="text-ink-muted">Đang tải...</p>
      ) : list.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-line py-16 text-center text-ink-muted">
          <MapPin className="mx-auto h-8 w-8 opacity-50" />
          <p className="mt-3">Bạn chưa có địa chỉ nào.</p>
        </div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {list.map((a) => (
            <div key={a.id} className={`surface p-5 ${a.isDefault ? 'ring-1 ring-clay-400' : ''}`}>
              <div className="flex items-start justify-between">
                <div className="flex items-center gap-2">
                  <span className="font-semibold text-ink">{a.receiverName}</span>
                  {a.label && <span className="chip">{a.label}</span>}
                </div>
                {a.isDefault && (
                  <span className="inline-flex items-center gap-1 rounded-full bg-clay-50 px-2 py-0.5 text-xs font-medium text-clay-700">
                    <Star className="h-3 w-3 fill-current" /> Mặc định
                  </span>
                )}
              </div>
              <p className="mt-1 text-sm text-ink-soft">{a.receiverPhone}</p>
              <p className="mt-1 text-sm text-ink-soft">{a.addressLine}</p>
              <div className="mt-4 flex flex-wrap items-center gap-2 border-t border-line pt-3 text-sm">
                {!a.isDefault && (
                  <button onClick={() => act(() => addressApi.setDefault(a.id), 'Đã đặt mặc định')} className="inline-flex items-center gap-1 font-medium text-clay-600 hover:underline">
                    <Check className="h-4 w-4" /> Đặt mặc định
                  </button>
                )}
                <button onClick={() => openEdit(a)} className="ml-auto inline-flex items-center gap-1 text-ink-soft hover:text-ink"><Pencil className="h-4 w-4" /> Sửa</button>
                <button onClick={() => window.confirm('Xóa địa chỉ này?') && act(() => addressApi.remove(a.id), 'Đã xóa')} className="inline-flex items-center gap-1 text-ink-soft hover:text-red-600"><Trash2 className="h-4 w-4" /> Xóa</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
