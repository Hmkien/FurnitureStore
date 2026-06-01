import { useEffect, useState } from 'react'
import { warrantyApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from '@/components/Toast'
import DataTable, { type Column } from '@/components/DataTable'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { cn } from '@/lib/utils'
import { dateTime, warrantyStatusLabel, enumOptions } from '@/lib/format'
import { WarrantyStatus, type WarrantyClaim } from '@/types'

const statusCls: Record<WarrantyStatus, string> = {
  [WarrantyStatus.Received]: 'bg-amber-100 text-amber-700',
  [WarrantyStatus.Processing]: 'bg-blue-100 text-blue-700',
  [WarrantyStatus.Completed]: 'bg-emerald-100 text-emerald-700',
  [WarrantyStatus.Rejected]: 'bg-red-100 text-red-700',
}

export default function WarrantyPage() {
  const toast = useToast()
  const [data, setData] = useState<WarrantyClaim[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [current, setCurrent] = useState<WarrantyClaim | null>(null)
  const [status, setStatus] = useState<WarrantyStatus>(WarrantyStatus.Received)
  const [note, setNote] = useState('')
  const pageSize = 10

  const load = async () => {
    setLoading(true)
    try {
      const res = await warrantyApi.all({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn: ['Code', 'CustomerName', 'ProductName'], sortBy: 'Created', sortDesc: true })
      setData((res.data as WarrantyClaim[]) ?? [])
      setTotal(res.recordsFiltered ?? 0)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setLoading(false)
    }
  }
  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, keyword])

  const open = (w: WarrantyClaim) => {
    setCurrent(w)
    setStatus(w.claimStatus)
    setNote(w.note ?? '')
  }
  const save = async () => {
    if (!current) return
    try {
      await warrantyApi.updateStatus(current.id, status, note)
      toast.success('Đã cập nhật')
      setCurrent(null)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const columns: Column<WarrantyClaim>[] = [
    { header: 'STT', className: 'w-14 text-center', cell: (_w, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span> },
    { header: 'Mã', cell: (w) => <span className="font-medium">{w.code}</span> },
    { header: 'Đơn', cell: (w) => w.orderCode },
    { header: 'Sản phẩm', cell: (w) => w.productName },
    { header: 'Khách', cell: (w) => `${w.customerName} · ${w.customerPhone}` },
    { header: 'Trạng thái', cell: (w) => <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', statusCls[w.claimStatus])}>{warrantyStatusLabel[w.claimStatus]}</span> },
    { header: 'Ngày tạo', cell: (w) => dateTime(w.created) },
  ]

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Bảo hành</h1>
        <Input placeholder="Tìm mã / khách / sản phẩm..." value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && (setPage(1), setKeyword(search))} className="w-64" />
      </div>

      <DataTable columns={columns} data={data} rowKey={(w) => w.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} onRowClick={open} />

      <Dialog open={!!current} onOpenChange={(v) => !v && setCurrent(null)}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>Phiếu {current?.code}</DialogTitle></DialogHeader>
          {current && (
            <div className="space-y-3 text-sm">
              <div className="text-muted-foreground">Đơn {current.orderCode} · {current.productName}</div>
              <div>{current.customerName} · {current.customerPhone}</div>
              <div className="rounded-lg border p-3"><span className="font-medium">Lỗi:</span> {current.issueDescription}</div>
              <div className="space-y-1.5">
                <Label>Trạng thái</Label>
                <Select value={String(status)} onValueChange={(v) => setStatus(Number(v) as WarrantyStatus)}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>{enumOptions(warrantyStatusLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
                </Select>
              </div>
              <div className="space-y-1.5">
                <Label>Ghi chú xử lý</Label>
                <Input value={note} onChange={(e) => setNote(e.target.value)} />
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setCurrent(null)}>Đóng</Button>
            <Button onClick={save}>Lưu</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
