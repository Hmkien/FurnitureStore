import { useEffect, useState } from 'react'
import { ordersApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from '@/components/Toast'
import DataTable, { type Column } from '@/components/DataTable'
import { OrderSimButton } from '@/components/seed-buttons'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { cn } from '@/lib/utils'
import { currency, dateTime, orderStatusLabel, paymentStatusLabel, paymentMethodLabel, enumOptions } from '@/lib/format'
import { OrderStatus, PaymentStatus, type Order, type OrderDetail } from '@/types'

const orderStatusCls: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'bg-amber-100 text-amber-700',
  [OrderStatus.Confirmed]: 'bg-blue-100 text-blue-700',
  [OrderStatus.Preparing]: 'bg-indigo-100 text-indigo-700',
  [OrderStatus.Shipping]: 'bg-cyan-100 text-cyan-700',
  [OrderStatus.Completed]: 'bg-emerald-100 text-emerald-700',
  [OrderStatus.Cancelled]: 'bg-red-100 text-red-700',
}

export default function OrdersPage() {
  const toast = useToast()
  const [data, setData] = useState<Order[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [order, setOrder] = useState<OrderDetail | null>(null)
  const [open, setOpen] = useState(false)
  const pageSize = 10

  const load = async () => {
    setLoading(true)
    try {
      const res = await ordersApi.all({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn: ['OrderCode', 'ReceiverName', 'ReceiverPhone'], sortBy: 'Created', sortDesc: true })
      setData(res.data ?? [])
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

  const openDetail = async (o: Order) => {
    setOpen(true)
    setOrder(null)
    try {
      setOrder(await ordersApi.detail(o.id))
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const act = async (fn: () => Promise<unknown>, ok: string) => {
    try {
      await fn()
      toast.success(ok)
      if (order) setOrder(await ordersApi.detail(order.id))
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const columns: Column<Order>[] = [
    { header: 'STT', className: 'w-14 text-center', cell: (_o, i) => <span className="text-muted-foreground">{(page - 1) * pageSize + i + 1}</span> },
    { header: 'Mã đơn', cell: (o) => <span className="font-medium">{o.orderCode}</span> },
    { header: 'Tổng tiền', className: 'text-right', cell: (o) => <span className="font-semibold">{currency(o.totalAmount)}</span> },
    { header: 'Thanh toán', cell: (o) => paymentMethodLabel[o.paymentMethod] },
    { header: 'TT thanh toán', cell: (o) => paymentStatusLabel[o.paymentStatus] },
    { header: 'Trạng thái', cell: (o) => <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', orderStatusCls[o.orderStatus])}>{orderStatusLabel[o.orderStatus]}</span> },
    { header: 'Ngày tạo', cell: (o) => dateTime(o.created) },
  ]

  const canCancel = order && order.orderStatus !== OrderStatus.Cancelled && order.orderStatus !== OrderStatus.Completed

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Đơn hàng</h1>
        <div className="flex items-center gap-2">
          <Input placeholder="Tìm mã đơn / người nhận..." value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && (setPage(1), setKeyword(search))} className="w-64" />
          <OrderSimButton onDone={load} />
        </div>
      </div>

      <DataTable columns={columns} data={data} rowKey={(o) => o.id} loading={loading} page={page} pageSize={pageSize} total={total} onPage={setPage} onRowClick={openDetail} />

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-h-[88vh] overflow-y-auto sm:max-w-3xl">
          <DialogHeader>
            <DialogTitle>{order ? `Đơn ${order.orderCode}` : 'Đang tải...'}</DialogTitle>
          </DialogHeader>
          {order && (
            <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
              <div className="lg:col-span-2">
                <Table>
                  <TableHeader><TableRow><TableHead>Sản phẩm</TableHead><TableHead>Thuộc tính</TableHead><TableHead className="text-right">Giá</TableHead><TableHead className="text-right">SL</TableHead><TableHead className="text-right">Thành tiền</TableHead></TableRow></TableHeader>
                  <TableBody>
                    {order.items.map((i) => (
                      <TableRow key={i.id}>
                        <TableCell className="font-medium">{i.productName}</TableCell>
                        <TableCell className="text-muted-foreground">{i.variantInfo}</TableCell>
                        <TableCell className="text-right">{currency(i.price)}</TableCell>
                        <TableCell className="text-right">{i.quantity}</TableCell>
                        <TableCell className="text-right font-semibold">{currency(i.lineTotal)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
                <div className="ml-auto mt-4 max-w-xs space-y-1 text-sm">
                  <Row label="Tạm tính" value={currency(order.subTotal)} />
                  <Row label="Phí vận chuyển" value={currency(order.shippingFee)} />
                  {order.discountAmount > 0 && <Row label={`Giảm giá ${order.couponCode ? `(${order.couponCode})` : ''}`} value={`- ${currency(order.discountAmount)}`} />}
                  <div className="border-t pt-1"><Row label="Tổng cộng" value={currency(order.totalAmount)} bold /></div>
                </div>
              </div>

              <div className="space-y-4">
                <div className="rounded-lg border p-3 text-sm">
                  <div className="mb-1 font-semibold">Người nhận</div>
                  <div>{order.receiverName}</div>
                  <div className="text-muted-foreground">{order.receiverPhone}</div>
                  <div className="text-muted-foreground">{order.shippingAddress}</div>
                  {order.note && <div className="mt-1 italic text-muted-foreground">Ghi chú: {order.note}</div>}
                </div>
                <div className="space-y-3">
                  <div className="space-y-1">
                    <Label>Trạng thái đơn</Label>
                    <Select value={String(order.orderStatus)} onValueChange={(v) => act(() => ordersApi.updateStatus(order.id, Number(v) as OrderStatus), 'Đã cập nhật')}>
                      <SelectTrigger><SelectValue /></SelectTrigger>
                      <SelectContent>{enumOptions(orderStatusLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
                    </Select>
                  </div>
                  <div className="space-y-1">
                    <Label>Trạng thái thanh toán</Label>
                    <Select value={String(order.paymentStatus)} onValueChange={(v) => act(() => ordersApi.updatePaymentStatus(order.id, Number(v) as PaymentStatus), 'Đã cập nhật')}>
                      <SelectTrigger><SelectValue /></SelectTrigger>
                      <SelectContent>{enumOptions(paymentStatusLabel).map((o) => <SelectItem key={o.value} value={String(o.value)}>{o.label}</SelectItem>)}</SelectContent>
                    </Select>
                  </div>
                  {canCancel && (
                    <Button variant="destructive" className="w-full" onClick={() => window.confirm('Hủy đơn? Tồn kho sẽ hoàn lại.') && act(() => ordersApi.cancel(order.id), 'Đã hủy đơn')}>
                      Hủy đơn
                    </Button>
                  )}
                </div>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  )
}

function Row({ label, value, bold }: { label: string; value: string; bold?: boolean }) {
  return (
    <div className="flex justify-between">
      <span className="text-muted-foreground">{label}</span>
      <span className={bold ? 'font-bold' : ''}>{value}</span>
    </div>
  )
}
