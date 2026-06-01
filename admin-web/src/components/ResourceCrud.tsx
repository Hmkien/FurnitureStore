import { useCallback, useEffect, useState, type ReactNode } from 'react'
import { Plus, Pencil, Trash2, Check, Ban, Search } from 'lucide-react'
import DataTable, { type Column } from './DataTable'
import RichTextEditor from './RichTextEditor'
import ImageField from './ImageField'
import SyncButton from './SyncButton'
import { useToast } from './Toast'
import { getErrorMessage } from '@/api/client'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Switch } from '@/components/ui/switch'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { StatusEntity, type BaseQuery, type PagedResult } from '@/types'

export interface CrudField {
  name: string
  label: string
  type?: 'text' | 'textarea' | 'number' | 'select' | 'switch' | 'image' | 'datetime' | 'richtext'
  options?: { value: number | string; label: string }[]
  required?: boolean
  folder?: string
  placeholder?: string
  full?: boolean
}

export interface ResourceApi<T> {
  paged: (q: BaseQuery) => Promise<PagedResult<T>>
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  create: (form: any) => Promise<unknown>
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  update: (id: string, form: any) => Promise<unknown>
  remove: (id: string) => Promise<unknown>
  approve: (id: string) => Promise<unknown>
  reject: (id: string) => Promise<unknown>
}

interface RecordBase {
  id: string
  status: StatusEntity
}

interface Props<T extends RecordBase> {
  title: string
  api: ResourceApi<T>
  columns: Column<T>[]
  fields: CrudField[]
  searchIn?: string[]
  syncResource?: string
  defaults?: Record<string, unknown>
  dialogWide?: boolean
  /** Nút thao tác thêm cho mỗi dòng (vd: phân quyền), hiển thị trước nút sửa/xóa. */
  rowActions?: (row: T) => ReactNode
  /** Nút bổ sung trên thanh tiêu đề (vd: đồng bộ RBAC). Nhận hàm reload danh sách. */
  headerActions?: (reload: () => void) => ReactNode
}

const statusMeta: Record<StatusEntity, { text: string; cls: string }> = {
  [StatusEntity.Approved]: { text: 'Đã duyệt', cls: 'bg-emerald-100 text-emerald-700' },
  [StatusEntity.Pending]: { text: 'Chờ duyệt', cls: 'bg-amber-100 text-amber-700' },
  [StatusEntity.Rejected]: { text: 'Đã hủy duyệt', cls: 'bg-red-100 text-red-700' },
  [StatusEntity.Draft]: { text: 'Nháp', cls: 'bg-slate-100 text-slate-600' },
}

const toLocalInput = (iso?: string) => (iso ? new Date(iso).toISOString().slice(0, 16) : '')

export default function ResourceCrud<T extends RecordBase>({
  title,
  api,
  columns,
  fields,
  searchIn,
  syncResource,
  defaults = {},
  dialogWide,
  rowActions,
  headerActions,
}: Props<T>) {
  const toast = useToast()
  const [data, setData] = useState<T[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [keyword, setKeyword] = useState('')
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<T | null>(null)
  const [form, setForm] = useState<Record<string, unknown>>({})
  const [saving, setSaving] = useState(false)
  const pageSize = 10

  const dateFields = fields.filter((f) => f.type === 'datetime').map((f) => f.name)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await api.paged({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn, sortBy: 'Created', sortDesc: true })
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

  const openCreate = () => {
    setEditing(null)
    setForm({ ...defaults })
    setOpen(true)
  }
  const openEdit = (row: T) => {
    setEditing(row)
    const init: Record<string, unknown> = { ...(row as Record<string, unknown>) }
    dateFields.forEach((n) => (init[n] = toLocalInput(init[n] as string)))
    setForm(init)
    setOpen(true)
  }

  const setField = (name: string, value: unknown) => setForm((f) => ({ ...f, [name]: value }))

  const submit = async () => {
    for (const f of fields) {
      if (f.required && !form[f.name]) return toast.error(`Vui lòng nhập ${f.label.toLowerCase()}`)
    }
    const payload: Record<string, unknown> = { ...form }
    dateFields.forEach((n) => {
      payload[n] = payload[n] ? new Date(payload[n] as string).toISOString() : undefined
    })
    setSaving(true)
    try {
      if (editing) await api.update(editing.id, payload)
      else await api.create(payload)
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

  const remove = (row: T) => {
    if (!window.confirm('Xóa vĩnh viễn bản ghi này?')) return
    act(() => api.remove(row.id), 'Đã xóa')
  }

  const fullColumns: Column<T>[] = [
    {
      header: 'STT',
      className: 'w-14 text-center',
      cell: (_r, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span>,
    },
    ...columns,
    {
      header: 'Trạng thái',
      cell: (r) => <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', statusMeta[r.status].cls)}>{statusMeta[r.status].text}</span>,
    },
    {
      header: '',
      className: 'text-right',
      cell: (r) => (
        <div className="flex justify-end gap-1">
          {rowActions?.(r)}
          {r.status === StatusEntity.Approved ? (
            <Button variant="ghost" size="icon" title="Hủy duyệt" onClick={() => act(() => api.reject(r.id), 'Đã hủy duyệt')}>
              <Ban className="h-4 w-4" />
            </Button>
          ) : (
            <>
              <Button variant="ghost" size="icon" title="Duyệt" className="text-emerald-600" onClick={() => act(() => api.approve(r.id), 'Đã duyệt')}>
                <Check className="h-4 w-4" />
              </Button>
              <Button variant="ghost" size="icon" title="Sửa" onClick={() => openEdit(r)}>
                <Pencil className="h-4 w-4" />
              </Button>
              <Button variant="ghost" size="icon" title="Xóa" className="text-red-600" onClick={() => remove(r)}>
                <Trash2 className="h-4 w-4" />
              </Button>
            </>
          )}
        </div>
      ),
    },
  ]

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">{title}</h1>
        <div className="flex items-center gap-2">
          {searchIn && (
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
                placeholder="Tìm kiếm..."
                className="w-56 pl-8"
              />
            </div>
          )}
          {headerActions?.(load)}
          {syncResource && <SyncButton resource={syncResource} onDone={load} />}
          <Button onClick={openCreate}>
            <Plus className="mr-2 h-4 w-4" /> Thêm mới
          </Button>
        </div>
      </div>

      <DataTable columns={fullColumns} data={data} rowKey={(r) => r.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} />

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className={cn('max-h-[88vh] overflow-y-auto', dialogWide ? 'sm:max-w-4xl' : 'sm:max-w-2xl')}>
          <DialogHeader>
            <DialogTitle>{editing ? `Sửa ${title.toLowerCase()}` : `Thêm ${title.toLowerCase()}`}</DialogTitle>
          </DialogHeader>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {fields.map((f) => (
              <div
                key={f.name}
                className={cn('space-y-1.5', (f.full || f.type === 'textarea' || f.type === 'richtext' || f.type === 'image') && 'sm:col-span-2')}
              >
                <Label>{f.label}</Label>
                {renderField(f, form[f.name], (v) => setField(f.name, v))}
              </div>
            ))}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>Hủy</Button>
            <Button onClick={submit} disabled={saving}>{saving ? 'Đang lưu...' : 'Lưu'}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}

function renderField(f: CrudField, value: unknown, onChange: (v: unknown) => void) {
  switch (f.type) {
    case 'textarea':
      return <Textarea rows={3} value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} placeholder={f.placeholder} />
    case 'number':
      return <Input type="number" value={(value as number) ?? 0} onChange={(e) => onChange(Number(e.target.value))} />
    case 'switch':
      return (
        <div>
          <Switch checked={!!value} onCheckedChange={onChange} />
        </div>
      )
    case 'image':
      return <ImageField value={value as string} onChange={onChange} folder={f.folder} />
    case 'richtext':
      return <RichTextEditor value={(value as string) ?? ''} onChange={onChange} />
    case 'datetime':
      return <Input type="datetime-local" value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} />
    case 'select': {
      const isNum = typeof f.options?.[0]?.value === 'number'
      return (
        <Select value={value !== undefined && value !== null ? String(value) : undefined} onValueChange={(v) => onChange(isNum ? Number(v) : v)}>
          <SelectTrigger>
            <SelectValue placeholder="-- Chọn --" />
          </SelectTrigger>
          <SelectContent>
            {f.options?.map((o) => (
              <SelectItem key={String(o.value)} value={String(o.value)}>
                {o.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      )
    }
    default:
      return <Input value={(value as string) ?? ''} onChange={(e) => onChange(e.target.value)} placeholder={f.placeholder} />
  }
}
