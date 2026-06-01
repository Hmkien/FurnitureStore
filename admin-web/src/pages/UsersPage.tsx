import { useCallback, useEffect, useState } from 'react'
import { Plus, Pencil, Trash2, Check, Ban, Search, KeyRound, ShieldCheck, UserCog, X } from 'lucide-react'
import DataTable, { type Column } from '@/components/DataTable'
import RolePickerDialog from '@/components/RolePickerDialog'
import { useToast } from '@/components/Toast'
import { usersApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { cn } from '@/lib/utils'
import { mediaUrl } from '@/lib/format'
import { StatusEntity, type UserAccount } from '@/types'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'

const statusMeta: Record<StatusEntity, { text: string; cls: string }> = {
  [StatusEntity.Approved]: { text: 'Hoạt động', cls: 'bg-emerald-100 text-emerald-700' },
  [StatusEntity.Pending]: { text: 'Chờ duyệt', cls: 'bg-amber-100 text-amber-700' },
  [StatusEntity.Rejected]: { text: 'Đã khóa', cls: 'bg-red-100 text-red-700' },
  [StatusEntity.Draft]: { text: 'Nháp', cls: 'bg-slate-100 text-slate-600' },
}

interface FormState {
  userName: string
  email: string
  firstName: string
  lastName: string
  phoneNumber: string
  birthday: string
  address: string
  password: string
  avatarFile: File | null
  removeImage: boolean
}

const emptyForm: FormState = {
  userName: '',
  email: '',
  firstName: '',
  lastName: '',
  phoneNumber: '',
  birthday: '',
  address: '',
  password: '',
  avatarFile: null,
  removeImage: false,
}

const pageSize = 10
const toDateInput = (iso?: string) => (iso ? new Date(iso).toISOString().slice(0, 10) : '')
const fullName = (u: UserAccount) => [u.lastName, u.firstName].filter(Boolean).join(' ').trim()

export default function UsersPage() {
  const toast = useToast()
  const [data, setData] = useState<UserAccount[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [keyword, setKeyword] = useState('')
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)

  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<UserAccount | null>(null)
  const [form, setForm] = useState<FormState>(emptyForm)
  const [saving, setSaving] = useState(false)

  const [pwOpen, setPwOpen] = useState(false)
  const [pwUser, setPwUser] = useState<UserAccount | null>(null)
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')

  const [roleUser, setRoleUser] = useState<UserAccount | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await usersApi.paged({
        pageNumber: page,
        pageSize,
        keyword: keyword || undefined,
        searchIn: ['UserName', 'Email', 'PhoneNumber'],
        sortBy: 'Created',
        sortDesc: true,
      })
      setData(res.data ?? [])
      setTotal(res.recordsFiltered ?? 0)
    } catch (e) {
      toast.error(getErrorMessage(e, 'Không tải được dữ liệu'))
    } finally {
      setLoading(false)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, keyword])

  useEffect(() => {
    load()
  }, [load])

  const setField = <K extends keyof FormState>(name: K, value: FormState[K]) =>
    setForm((f) => ({ ...f, [name]: value }))

  const openCreate = () => {
    setEditing(null)
    setForm(emptyForm)
    setOpen(true)
  }

  const openEdit = (u: UserAccount) => {
    setEditing(u)
    setForm({
      ...emptyForm,
      userName: u.userName,
      email: u.email,
      firstName: u.firstName ?? '',
      lastName: u.lastName ?? '',
      phoneNumber: u.phoneNumber ?? '',
      birthday: toDateInput(u.birthday),
      address: u.address ?? '',
    })
    setOpen(true)
  }

  const submit = async () => {
    if (!editing && !form.userName.trim()) return toast.error('Vui lòng nhập tên đăng nhập')
    if (!form.email.trim()) return toast.error('Vui lòng nhập email')
    if (!editing && !form.password.trim()) return toast.error('Vui lòng nhập mật khẩu')

    const fd = new FormData()
    fd.append('Email', form.email)
    if (form.firstName) fd.append('FirstName', form.firstName)
    if (form.lastName) fd.append('LastName', form.lastName)
    if (form.phoneNumber) fd.append('PhoneNumber', form.phoneNumber)
    if (form.birthday) fd.append('Birthday', form.birthday)
    if (form.address) fd.append('Address', form.address)
    if (form.avatarFile) fd.append('ImageAvatar', form.avatarFile)
    fd.append('RemoveImage', String(form.removeImage))
    if (!editing) {
      fd.append('UserName', form.userName)
      fd.append('Password', form.password)
    }

    setSaving(true)
    try {
      if (editing) await usersApi.update(editing.id, fd)
      else await usersApi.create(fd)
      toast.success('Đã lưu')
      setOpen(false)
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

  const remove = (u: UserAccount) => {
    if (!window.confirm(`Xóa vĩnh viễn tài khoản "${u.userName}"?`)) return
    act(() => usersApi.remove(u.id), 'Đã xóa')
  }

  const openResetPw = (u: UserAccount) => {
    setPwUser(u)
    setNewPassword('')
    setConfirmPassword('')
    setPwOpen(true)
  }

  const submitResetPw = async () => {
    if (newPassword.length < 6) return toast.error('Mật khẩu mới phải từ 6 ký tự')
    if (newPassword !== confirmPassword) return toast.error('Mật khẩu xác nhận không khớp')
    if (!pwUser) return
    setSaving(true)
    try {
      await usersApi.resetPassword(pwUser.id, newPassword, confirmPassword)
      toast.success('Đã đổi mật khẩu')
      setPwOpen(false)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  const columns: Column<UserAccount>[] = [
    {
      header: 'STT',
      className: 'w-14 text-center',
      cell: (_r, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span>,
    },
    {
      header: 'Tài khoản',
      cell: (u) => (
        <div className="flex items-center gap-3">
          {u.imageAvatar ? (
            <img src={mediaUrl(u.imageAvatar)} alt="" className="h-9 w-9 rounded-full border object-cover" />
          ) : (
            <span className="flex h-9 w-9 items-center justify-center rounded-full bg-muted text-sm font-semibold uppercase text-muted-foreground">
              {u.userName.charAt(0)}
            </span>
          )}
          <div className="leading-tight">
            <div className="font-medium">{u.userName}</div>
            {fullName(u) && <div className="text-xs text-muted-foreground">{fullName(u)}</div>}
          </div>
        </div>
      ),
    },
    { header: 'Email', cell: (u) => u.email },
    { header: 'Điện thoại', cell: (u) => u.phoneNumber || <span className="text-muted-foreground">—</span> },
    {
      header: 'Loại',
      cell: (u) =>
        u.isSuperUser ? (
          <span className="inline-flex items-center gap-1 rounded-full bg-violet-100 px-2 py-0.5 text-xs font-medium text-violet-700">
            <ShieldCheck className="h-3.5 w-3.5" /> Quản trị cấp cao
          </span>
        ) : (
          <span className="text-muted-foreground">Nhân viên</span>
        ),
    },
    {
      header: 'Trạng thái',
      cell: (u) => (
        <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', statusMeta[u.status].cls)}>
          {statusMeta[u.status].text}
        </span>
      ),
    },
    {
      header: '',
      className: 'text-right',
      cell: (u) => (
        <div className="flex justify-end gap-1">
          {u.status === StatusEntity.Approved ? (
            <Button variant="ghost" size="icon" title="Khóa tài khoản" onClick={() => act(() => usersApi.reject(u.id), 'Đã khóa')}>
              <Ban className="h-4 w-4" />
            </Button>
          ) : (
            <Button variant="ghost" size="icon" title="Kích hoạt" className="text-emerald-600" onClick={() => act(() => usersApi.approve(u.id), 'Đã kích hoạt')}>
              <Check className="h-4 w-4" />
            </Button>
          )}
          <Button variant="ghost" size="icon" title="Phân vai trò" className="text-indigo-600" onClick={() => setRoleUser(u)}>
            <UserCog className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon" title="Đổi mật khẩu" onClick={() => openResetPw(u)}>
            <KeyRound className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon" title="Sửa" onClick={() => openEdit(u)}>
            <Pencil className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon" title="Xóa" className="text-red-600" onClick={() => remove(u)}>
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      ),
    },
  ]

  const currentAvatar = editing?.imageAvatar && !form.avatarFile && !form.removeImage ? mediaUrl(editing.imageAvatar) : null
  const previewAvatar = form.avatarFile ? URL.createObjectURL(form.avatarFile) : currentAvatar

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Tài khoản</h1>
        <div className="flex items-center gap-2">
          <div className="relative">
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  setPage(1)
                  setKeyword(search)
                }
              }}
              placeholder="Tìm theo tên, email, SĐT..."
              className="w-64 pl-8"
            />
          </div>
          <Button onClick={openCreate}>
            <Plus className="mr-2 h-4 w-4" /> Thêm tài khoản
          </Button>
        </div>
      </div>

      <DataTable columns={columns} data={data} rowKey={(u) => u.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} />

      {/* Dialog tạo/sửa tài khoản */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-h-[88vh] overflow-y-auto sm:max-w-2xl">
          <DialogHeader>
            <DialogTitle>{editing ? 'Sửa tài khoản' : 'Thêm tài khoản'}</DialogTitle>
          </DialogHeader>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div className="space-y-1.5">
              <Label>Tên đăng nhập</Label>
              <Input value={form.userName} disabled={!!editing} onChange={(e) => setField('userName', e.target.value)} placeholder="username" />
            </div>
            <div className="space-y-1.5">
              <Label>Email</Label>
              <Input type="email" value={form.email} onChange={(e) => setField('email', e.target.value)} placeholder="email@vidu.com" />
            </div>
            <div className="space-y-1.5">
              <Label>Họ</Label>
              <Input value={form.lastName} onChange={(e) => setField('lastName', e.target.value)} />
            </div>
            <div className="space-y-1.5">
              <Label>Tên</Label>
              <Input value={form.firstName} onChange={(e) => setField('firstName', e.target.value)} />
            </div>
            <div className="space-y-1.5">
              <Label>Số điện thoại</Label>
              <Input value={form.phoneNumber} onChange={(e) => setField('phoneNumber', e.target.value)} />
            </div>
            <div className="space-y-1.5">
              <Label>Ngày sinh</Label>
              <Input type="date" value={form.birthday} onChange={(e) => setField('birthday', e.target.value)} />
            </div>
            {!editing && (
              <div className="space-y-1.5">
                <Label>Mật khẩu</Label>
                <Input type="password" value={form.password} onChange={(e) => setField('password', e.target.value)} placeholder="••••••" />
              </div>
            )}
            <div className="space-y-1.5 sm:col-span-2">
              <Label>Địa chỉ</Label>
              <Input value={form.address} onChange={(e) => setField('address', e.target.value)} />
            </div>
            <div className="space-y-1.5 sm:col-span-2">
              <Label>Ảnh đại diện</Label>
              <div className="flex items-center gap-3">
                {previewAvatar && <img src={previewAvatar} alt="" className="h-16 w-16 rounded-full border object-cover" />}
                <Input
                  type="file"
                  accept="image/*"
                  className="max-w-xs"
                  onChange={(e) => setField('avatarFile', e.target.files?.[0] ?? null)}
                />
                {editing && (currentAvatar || form.avatarFile) && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    title="Xóa ảnh"
                    onClick={() => setForm((f) => ({ ...f, avatarFile: null, removeImage: true }))}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                )}
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>Hủy</Button>
            <Button onClick={submit} disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu'}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Dialog đổi mật khẩu (admin) */}
      <Dialog open={pwOpen} onOpenChange={setPwOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Đổi mật khẩu {pwUser ? `– ${pwUser.userName}` : ''}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-1.5">
              <Label>Mật khẩu mới</Label>
              <Input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
            </div>
            <div className="space-y-1.5">
              <Label>Xác nhận mật khẩu</Label>
              <Input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setPwOpen(false)}>Hủy</Button>
            <Button onClick={submitResetPw} disabled={saving}>{saving ? 'Đang lưu...' : 'Đổi mật khẩu'}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Dialog phân vai trò */}
      <RolePickerDialog
        open={!!roleUser}
        onOpenChange={(v) => !v && setRoleUser(null)}
        userId={roleUser?.id}
        userName={roleUser?.userName}
      />
    </div>
  )
}
