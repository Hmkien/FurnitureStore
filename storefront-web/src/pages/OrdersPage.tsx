import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Package, ChevronDown, Search } from 'lucide-react'
import { orderApi } from '../api/services'
import { currency, dateTime, img } from '../lib/format'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import { OrderStatus, PaymentStatus, type OrderListItem, type OrderDetail } from '../types'

const orderStatusLabel: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'Chờ xác nhận',
  [OrderStatus.Confirmed]: 'Đã xác nhận',
  [OrderStatus.Preparing]: 'Đang chuẩn bị',
  [OrderStatus.Shipping]: 'Đang giao',
  [OrderStatus.Completed]: 'Hoàn thành',
  [OrderStatus.Cancelled]: 'Đã hủy',
}
const orderStatusCls: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'bg-amber-100 text-amber-700',
  [OrderStatus.Confirmed]: 'bg-blue-100 text-blue-700',
  [OrderStatus.Preparing]: 'bg-indigo-100 text-indigo-700',
  [OrderStatus.Shipping]: 'bg-cyan-100 text-cyan-700',
  [OrderStatus.Completed]: 'bg-emerald-100 text-emerald-700',
  [OrderStatus.Cancelled]: 'bg-red-100 text-red-700',
}
const paymentStatusLabel: Record<PaymentStatus, string> = {
  [PaymentStatus.Unpaid]: 'Chưa thanh toán',
  [PaymentStatus.Paid]: 'Đã thanh toán',
  [PaymentStatus.Refunded]: 'Đã hoàn tiền',
}

const statusTabs: { value?: OrderStatus; label: string }[] = [
  { value: undefined, label: 'Tất cả' },
  { value: OrderStatus.Pending, label: 'Chờ xác nhận' },
  { value: OrderStatus.Shipping, label: 'Đang giao' },
  { value: OrderStatus.Completed, label: 'Hoàn thành' },
  { value: OrderStatus.Cancelled, label: 'Đã hủy' },
]

export default function OrdersPage() {
  const toast = useToast()
  const [orders, setOrders] = useState<OrderListItem[]>([])
  const [detail, setDetail] = useState<OrderDetail | null>(null)
  const [openId, setOpenId] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [status, setStatus] = useState<OrderStatus | undefined>(undefined)
  const [keyword, setKeyword] = useState('')
  const [search, setSearch] = useState('')

  const load = useCallback(() => {
    setLoading(true)
    orderApi.my({ keyword: keyword || undefined, orderStatus: status })
      .then((r) => setOrders(r.data ?? []))
      .catch((e) => toast.error(getErrorMessage(e)))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [keyword, status])

  useEffect(load, [load])

  const toggle = async (id: string) => {
    if (openId === id) return setOpenId(null)
    setOpenId(id)
    setDetail(null)
    try {
      setDetail(await orderApi.detail(id))
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  return (
    <div>
      <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
        <h2 className="font-display text-2xl text-ink">Đơn hàng của tôi</h2>
        <form
          onSubmit={(e) => { e.preventDefault(); setKeyword(search.trim()) }}
          className="flex items-center rounded-full border border-line bg-paper px-3.5 py-2"
        >
          <Search className="h-4 w-4 text-ink-muted" />
          <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Tìm mã đơn..." className="w-44 bg-transparent px-2 text-sm outline-none" />
        </form>
      </div>

      {/* Lọc trạng thái */}
      <div className="mb-6 flex flex-wrap gap-2">
        {statusTabs.map((t) => {
          const active = status === t.value
          return (
            <button
              key={t.label}
              onClick={() => setStatus(t.value)}
              className={`rounded-full border px-4 py-1.5 text-sm font-medium transition ${
                active ? 'border-ink bg-ink text-paper' : 'border-line bg-paper text-ink-soft hover:border-ink/40'
              }`}
            >
              {t.label}
            </button>
          )
        })}
      </div>

      {loading ? (
        <p className="text-ink-muted">Đang tải...</p>
      ) : orders.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-line py-16 text-center">
          <Package className="mx-auto h-8 w-8 text-ink-muted opacity-50" />
          <p className="mt-3 text-ink-soft">Không có đơn hàng phù hợp.</p>
          <Link to="/products" className="btn-primary mt-5">Mua sắm ngay</Link>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((o) => {
            const open = openId === o.id
            return (
              <div key={o.id} className="surface overflow-hidden">
                <button onClick={() => toggle(o.id)} className="flex w-full items-center gap-4 p-4 text-left">
                  <div className="h-16 w-16 shrink-0 overflow-hidden rounded-xl bg-[#ece4d8]">
                    {o.thumbnailUrl ? (
                      <img src={img(o.thumbnailUrl)} alt="" className="h-full w-full object-cover" />
                    ) : (
                      <div className="grid h-full place-items-center text-ink-muted/40"><Package className="h-6 w-6" /></div>
                    )}
                  </div>
                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2">
                      <span className="font-medium text-ink">{o.orderCode}</span>
                      <span className={`rounded-full px-2 py-0.5 text-[11px] font-medium ${orderStatusCls[o.orderStatus]}`}>{orderStatusLabel[o.orderStatus]}</span>
                    </div>
                    <div className="mt-0.5 truncate text-sm text-ink-soft">
                      {o.firstProductName ?? 'Đơn hàng'}
                      {o.itemCount > 1 && <span className="text-ink-muted"> và {o.itemCount - 1} sản phẩm khác…</span>}
                    </div>
                    <div className="mt-0.5 text-xs text-ink-muted">{dateTime(o.created)} · {paymentStatusLabel[o.paymentStatus]}</div>
                  </div>
                  <div className="text-right">
                    <div className="font-display text-lg text-clay-600">{currency(o.totalAmount)}</div>
                    <div className="text-xs text-ink-muted">{o.itemCount} SP</div>
                  </div>
                  <ChevronDown className={`h-5 w-5 shrink-0 text-ink-muted transition ${open ? 'rotate-180' : ''}`} />
                </button>

                {open && (
                  <div className="border-t border-line p-5">
                    {!detail ? (
                      <p className="text-sm text-ink-muted">Đang tải...</p>
                    ) : (
                      <>
                        <div className="space-y-2.5">
                          {detail.items.map((it) => (
                            <div key={it.id} className="flex items-center justify-between gap-3 text-sm">
                              <span className="text-ink-soft">
                                {it.productName}
                                {it.variantInfo && <span className="text-ink-muted"> · {it.variantInfo}</span>}
                                <span className="text-ink-muted"> × {it.quantity}</span>
                              </span>
                              <span className="shrink-0 font-medium text-ink">{currency(it.lineTotal)}</span>
                            </div>
                          ))}
                        </div>
                        <div className="mt-4 space-y-1 border-t border-line pt-3 text-sm">
                          <Row label="Tạm tính" value={currency(detail.subTotal)} />
                          {detail.discountAmount > 0 && <Row label={`Giảm giá ${detail.couponCode ? `(${detail.couponCode})` : ''}`} value={`- ${currency(detail.discountAmount)}`} />}
                          <Row label="Phí vận chuyển" value={detail.shippingFee === 0 ? 'Miễn phí' : currency(detail.shippingFee)} />
                          <div className="flex justify-between pt-1 font-medium text-ink">
                            <span>Tổng cộng</span><span className="text-clay-600">{currency(detail.totalAmount)}</span>
                          </div>
                        </div>
                        <div className="mt-4 rounded-2xl bg-cream p-4 text-sm text-ink-soft">
                          <span className="font-medium text-ink">Giao tới: </span>
                          {detail.receiverName} · {detail.receiverPhone}
                          <br />
                          {detail.shippingAddress}
                          {detail.note && <div className="mt-1 italic">Ghi chú: {detail.note}</div>}
                        </div>
                      </>
                    )}
                  </div>
                )}
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between text-ink-soft">
      <span>{label}</span>
      <span>{value}</span>
    </div>
  )
}
