import { useEffect, useState } from 'react'
import { Boxes, History } from 'lucide-react'
import { inventoryApi } from '@/api/services'
import { useToast } from '@/components/Toast'
import { getErrorMessage } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Card } from '@/components/ui/card'
import { movementTypeLabel, enumOptions, dateTime } from '@/lib/format'
import { StockMovementType, type LowStockVariant, type StockMovement } from '@/types'

export default function InventoryPage() {
  const toast = useToast()
  const [threshold, setThreshold] = useState(5)
  const [rows, setRows] = useState<LowStockVariant[]>([])
  const [loading, setLoading] = useState(false)

  const [adjustOpen, setAdjustOpen] = useState(false)
  const [current, setCurrent] = useState<LowStockVariant | null>(null)
  const [form, setForm] = useState<{ type: StockMovementType; quantity: number; note: string }>({ type: StockMovementType.Import, quantity: 1, note: '' })

  const [histOpen, setHistOpen] = useState(false)
  const [history, setHistory] = useState<StockMovement[]>([])

  const load = () => {
    setLoading(true)
    inventoryApi.lowStock(threshold).then(setRows).catch((e) => toast.error(getErrorMessage(e))).finally(() => setLoading(false))
  }
  useEffect(load, [threshold]) // eslint-disable-line react-hooks/exhaustive-deps

  const openAdjust = (r: LowStockVariant) => {
    setCurrent(r)
    setForm({ type: StockMovementType.Import, quantity: 1, note: '' })
    setAdjustOpen(true)
  }
  const saveAdjust = async () => {
    try {
      await inventoryApi.adjust({ variantId: current!.variantId, type: form.type, quantity: form.quantity, note: form.note })
      toast.success('Đã cập nhật kho')
      setAdjustOpen(false)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }
  const openHistory = async (r: LowStockVariant) => {
    try {
      const res = await inventoryApi.history(r.variantId, { pageNumber: 1, pageSize: 50, sortBy: 'Created', sortDesc: true })
      setHistory(res.data ?? [])
      setHistOpen(true)
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="flex items-center gap-2 text-xl font-semibold"><Boxes className="h-5 w-5" /> Quản lý kho</h1>
        <div className="flex items-center gap-2 text-sm">
          <span className="text-muted-foreground">Ngưỡng cảnh báo</span>
          <Input type="number" value={threshold} onChange={(e) => setThreshold(Number(e.target.value))} className="w-24" />
        </div>
      </div>

      <Card className="overflow-hidden">
        <Table>
          <TableHeader><TableRow><TableHead className="w-14 text-center">STT</TableHead><TableHead>Sản phẩm</TableHead><TableHead>SKU</TableHead><TableHead className="text-right">Tồn</TableHead><TableHead className="text-right">Thao tác</TableHead></TableRow></TableHeader>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={5} className="h-24 text-center text-muted-foreground">Đang tải...</TableCell></TableRow>
            ) : rows.length === 0 ? (
              <TableRow><TableCell colSpan={5} className="h-24 text-center text-muted-foreground">Không có biến thể dưới ngưỡng</TableCell></TableRow>
            ) : rows.map((r, i) => (
              <TableRow key={r.variantId}>
                <TableCell className="text-center text-muted-foreground">{i + 1}</TableCell>
                <TableCell className="font-medium">{r.productName}</TableCell>
                <TableCell className="text-muted-foreground">{r.skuVariant}</TableCell>
                <TableCell className="text-right"><span className="rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-700">{r.stockQuantity}</span></TableCell>
                <TableCell className="text-right">
                  <Button variant="outline" size="sm" onClick={() => openAdjust(r)}>Nhập/Xuất</Button>
                  <Button variant="ghost" size="sm" className="ml-2" onClick={() => openHistory(r)}><History className="mr-1 h-4 w-4" /> Lịch sử</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Card>

      <Dialog open={adjustOpen} onOpenChange={setAdjustOpen}>
        <DialogContent>
          <DialogHeader><DialogTitle>Điều chỉnh kho</DialogTitle></DialogHeader>
          <p className="text-sm text-muted-foreground">{current?.productName} · {current?.skuVariant}</p>
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1.5">
              <Label>Loại</Label>
              <Select value={String(form.type)} onValueChange={(v) => setForm({ ...form, type: Number(v) as StockMovementType })}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>{enumOptions(movementTypeLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
              </Select>
            </div>
            <div className="space-y-1.5">
              <Label>Số lượng</Label>
              <Input type="number" value={form.quantity} onChange={(e) => setForm({ ...form, quantity: Number(e.target.value) })} />
            </div>
          </div>
          <div className="space-y-1.5">
            <Label>Ghi chú</Label>
            <Input value={form.note} onChange={(e) => setForm({ ...form, note: e.target.value })} />
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setAdjustOpen(false)}>Hủy</Button>
            <Button onClick={saveAdjust}>Lưu</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={histOpen} onOpenChange={setHistOpen}>
        <DialogContent className="sm:max-w-2xl">
          <DialogHeader><DialogTitle>Lịch sử biến động kho</DialogTitle></DialogHeader>
          <Table>
            <TableHeader><TableRow><TableHead>Thời gian</TableHead><TableHead>Loại</TableHead><TableHead className="text-right">SL</TableHead><TableHead className="text-right">Tồn sau</TableHead><TableHead>Ghi chú</TableHead></TableRow></TableHeader>
            <TableBody>
              {history.length === 0 ? (
                <TableRow><TableCell colSpan={5} className="text-center text-muted-foreground">Chưa có biến động</TableCell></TableRow>
              ) : history.map((h) => (
                <TableRow key={h.id}>
                  <TableCell className="text-muted-foreground">{dateTime(h.created)}</TableCell>
                  <TableCell>{movementTypeLabel[h.type]}</TableCell>
                  <TableCell className="text-right">{h.quantity}</TableCell>
                  <TableCell className="text-right font-medium">{h.quantityAfter}</TableCell>
                  <TableCell className="text-muted-foreground">{h.note}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </DialogContent>
      </Dialog>
    </div>
  )
}
