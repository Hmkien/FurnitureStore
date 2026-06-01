import { useEffect, useState } from 'react'
import { returnsApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from '@/components/Toast'
import DataTable, { type Column } from '@/components/DataTable'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { cn } from '@/lib/utils'
import { dateTime, returnTypeLabel, returnStatusLabel, enumOptions } from '@/lib/format'
import { ReturnStatus, type ReturnRequest } from '@/types'

const statusCls: Record<ReturnStatus, string> = {
  [ReturnStatus.Requested]: 'bg-amber-100 text-amber-700',
  [ReturnStatus.Approved]: 'bg-blue-100 text-blue-700',
  [ReturnStatus.Rejected]: 'bg-red-100 text-red-700',
  [ReturnStatus.Completed]: 'bg-emerald-100 text-emerald-700',
}

export default function ReturnsPage() {
  const toast = useToast()
  const [data, setData] = useState<ReturnRequest[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [current, setCurrent] = useState<ReturnRequest | null>(null)
  const [status, setStatus] = useState<ReturnStatus>(ReturnStatus.Requested)
  const [note, setNote] = useState('')
  const pageSize = 10

  const load = async () => {
    setLoading(true)
    try {
      const res = await returnsApi.all({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn: ['Code', 'Reason'], sortBy: 'Created', sortDesc: true })
      setData((res.data as ReturnRequest[]) ?? [])
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

  const open = (r: ReturnRequest) => {
    setCurrent(r)
    setStatus(r.requestStatus)
    setNote(r.note ?? '')
  }

  const save = async () => {
    if (!current) return
    try {
      await returnsApi.updateStatus(current.id, status, note)
      toast.success('Đã cập nhật')
      setCurrent(null)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const columns: Column<ReturnRequest>[] = [
    { header: 'STT', className: 'w-14 text-center', cell: (_r, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span> },
    { header: 'Mã', cell: (r) => <span className="font-medium">{r.code}</span> },
    { header: 'Đơn hàng', cell: (r) => r.orderCode },
    { header: 'Khách', cell: (r) => r.customerName },
    { header: 'Loại', cell: (r) => returnTypeLabel[r.type] },
    { header: 'Trạng thái', cell: (r) => <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', statusCls[r.requestStatus])}>{returnStatusLabel[r.requestStatus]}</span> },
    { header: 'Ngày tạo', cell: (r) => dateTime(r.created) },
  ]

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Đổi / Trả hàng</h1>
        <Input placeholder="Tìm mã / lý do..." value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && (setPage(1), setKeyword(search))} className="w-64" />
      </div>

      <DataTable columns={columns} data={data} rowKey={(r) => r.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} onRowClick={open} />

      <Dialog open={!!current} onOpenChange={(v) => !v && setCurrent(null)}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader><DialogTitle>Yêu cầu {current?.code}</DialogTitle></DialogHeader>
          {current && (
            <div className="space-y-3 text-sm">
              <div className="text-muted-foreground">Đơn {current.orderCode} · {returnTypeLabel[current.type]}</div>
              <div><span className="font-medium">Lý do:</span> {current.reason}</div>
              <div className="rounded-lg border p-3">
                <div className="mb-1 font-medium">Sản phẩm</div>
                {current.items.map((i, idx) => (
                  <div key={idx} className="flex justify-between text-muted-foreground"><span>{i.productName}</span><span>x{i.quantity}</span></div>
                ))}
              </div>
              <div className="space-y-1.5">
                <Label>Trạng thái</Label>
                <Select value={String(status)} onValueChange={(v) => setStatus(Number(v) as ReturnStatus)}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>{enumOptions(returnStatusLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
                </Select>
              </div>
              <div className="space-y-1.5">
                <Label>Ghi chú xử lý</Label>
                <Input value={note} onChange={(e) => setNote(e.target.value)} />
              </div>
              <p className="text-xs text-muted-foreground">* Hoàn tất "Trả hàng"/"Hoàn tiền" sẽ tự cộng lại tồn kho.</p>
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
