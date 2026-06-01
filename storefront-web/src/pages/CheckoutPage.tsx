import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Truck, Wallet, Landmark, CreditCard, Check, MapPin, Plus } from 'lucide-react'
import { orderApi, couponApi, paymentApi, addressApi } from '../api/services'
import { useCart } from '../context/CartContext'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import { currency } from '../lib/format'
import { PaymentMethod, type Address } from '../types'

// Khớp cấu hình Shipping của API (FlatFee / FreeShippingThreshold).
const FLAT_FEE = 50000
const FREE_THRESHOLD = 5000000

const methods = [
  { v: PaymentMethod.COD, label: 'Thanh toán khi nhận hàng', desc: 'COD — trả tiền mặt khi nhận', icon: Truck },
  { v: PaymentMethod.VNPAY, label: 'VNPAY', desc: 'Thẻ ATM / QR ngân hàng', icon: CreditCard },
  { v: PaymentMethod.Momo, label: 'Ví Momo', desc: 'Thanh toán qua ví Momo', icon: Wallet },
  { v: PaymentMethod.BankTransfer, label: 'Chuyển khoản', desc: 'Chuyển khoản ngân hàng', icon: Landmark },
]

export default function CheckoutPage() {
  const { cart, reload } = useCart()
  const navigate = useNavigate()
  const toast = useToast()
  const [form, setForm] = useState({ receiverName: '', receiverPhone: '', shippingAddress: '', note: '' })
  const [couponCode, setCouponCode] = useState('')
  const [discount, setDiscount] = useState(0)
  const [method, setMethod] = useState<PaymentMethod>(PaymentMethod.COD)
  const [placing, setPlacing] = useState(false)

  const [addresses, setAddresses] = useState<Address[]>([])
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [manual, setManual] = useState(false)

  const fillFrom = (a: Address) =>
    setForm((f) => ({ ...f, receiverName: a.receiverName, receiverPhone: a.receiverPhone, shippingAddress: a.addressLine }))

  useEffect(() => {
    addressApi
      .list()
      .then((list) => {
        setAddresses(list)
        const def = list.find((a) => a.isDefault) ?? list[0]
        if (def) {
          setSelectedId(def.id)
          fillFrom(def)
        } else {
          setManual(true)
        }
      })
      .catch(() => setManual(true))
  }, [])

  const selectAddress = (a: Address) => {
    setSelectedId(a.id)
    setManual(false)
    fillFrom(a)
  }

  const subTotal = cart?.subTotal ?? 0
  const shippingFee = subTotal <= 0 ? 0 : subTotal >= FREE_THRESHOLD ? 0 : FLAT_FEE
  const total = Math.max(0, subTotal - discount) + shippingFee

  const applyCoupon = async () => {
    if (!couponCode) return
    try {
      const res = await couponApi.preview(couponCode, subTotal)
      setDiscount(res.discountAmount)
      toast.success(`Áp dụng mã: -${currency(res.discountAmount)}`)
    } catch (e) {
      setDiscount(0)
      toast.error(getErrorMessage(e))
    }
  }

  const place = async () => {
    if (!form.receiverName || !form.receiverPhone || !form.shippingAddress) return toast.error('Vui lòng nhập đầy đủ thông tin nhận hàng')
    setPlacing(true)
    try {
      const order = await orderApi.checkout({ ...form, couponCode: couponCode || undefined, shippingFee, paymentMethod: method })
      await reload()
      if (method === PaymentMethod.VNPAY) {
        window.location.href = (await paymentApi.vnpay(order.orderCode)).paymentUrl
        return
      }
      if (method === PaymentMethod.Momo) {
        window.location.href = (await paymentApi.momo(order.orderCode)).paymentUrl
        return
      }
      toast.success('Đặt hàng thành công!')
      navigate('/tai-khoan/don-hang')
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setPlacing(false)
    }
  }

  return (
    <div className="u-container py-10">
      <span className="eyebrow">Thanh toán</span>
      <h1 className="display mt-2 text-4xl text-ink">Hoàn tất đơn hàng</h1>

      <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-[1fr_380px]">
        <div className="space-y-8">
          <section className="surface p-6">
            <div className="flex items-center justify-between">
              <h2 className="font-display text-xl text-ink">Thông tin nhận hàng</h2>
              <Link to="/tai-khoan/dia-chi" className="inline-flex items-center gap-1 text-sm font-medium text-clay-600 hover:underline">
                <Plus className="h-4 w-4" /> Thêm địa chỉ
              </Link>
            </div>

            {/* Chọn từ sổ địa chỉ */}
            {addresses.length > 0 && !manual && (
              <div className="mt-4 space-y-2.5">
                {addresses.map((a) => {
                  const active = selectedId === a.id
                  return (
                    <button
                      key={a.id}
                      onClick={() => selectAddress(a)}
                      className={`flex w-full items-start gap-3 rounded-2xl border p-4 text-left transition ${active ? 'border-clay-500 bg-clay-50' : 'border-line hover:border-ink/30'}`}
                    >
                      <MapPin className={`mt-0.5 h-5 w-5 shrink-0 ${active ? 'text-clay-600' : 'text-ink-muted'}`} />
                      <span className="flex-1">
                        <span className="flex items-center gap-2">
                          <span className="font-medium text-ink">{a.receiverName}</span>
                          <span className="text-sm text-ink-muted">{a.receiverPhone}</span>
                          {a.isDefault && <span className="rounded-full bg-clay-100 px-2 py-0.5 text-[11px] font-medium text-clay-700">Mặc định</span>}
                        </span>
                        <span className="mt-0.5 block text-sm text-ink-soft">{a.addressLine}</span>
                      </span>
                      {active && <Check className="h-5 w-5 text-clay-600" />}
                    </button>
                  )
                })}
                <button onClick={() => setManual(true)} className="text-sm font-medium text-ink-soft hover:text-clay-600">
                  + Nhập địa chỉ khác
                </button>
              </div>
            )}

            {/* Nhập tay */}
            {(manual || addresses.length === 0) && (
              <div className="mt-4 grid grid-cols-1 gap-3 sm:grid-cols-2">
                <input className="field" placeholder="Họ tên người nhận" value={form.receiverName} onChange={(e) => setForm({ ...form, receiverName: e.target.value })} />
                <input className="field" placeholder="Số điện thoại" value={form.receiverPhone} onChange={(e) => setForm({ ...form, receiverPhone: e.target.value })} />
                <input className="field sm:col-span-2" placeholder="Địa chỉ giao hàng" value={form.shippingAddress} onChange={(e) => setForm({ ...form, shippingAddress: e.target.value })} />
                {addresses.length > 0 && (
                  <button onClick={() => { setManual(false); const a = addresses.find((x) => x.id === selectedId) ?? addresses[0]; if (a) selectAddress(a) }} className="w-fit text-sm font-medium text-ink-soft hover:text-clay-600">
                    ← Chọn địa chỉ đã lưu
                  </button>
                )}
              </div>
            )}

            <textarea className="field mt-3 resize-none" rows={2} placeholder="Ghi chú (tuỳ chọn)" value={form.note} onChange={(e) => setForm({ ...form, note: e.target.value })} />
          </section>

          <section className="surface p-6">
            <h2 className="font-display text-xl text-ink">Phương thức thanh toán</h2>
            <div className="mt-4 grid gap-3 sm:grid-cols-2">
              {methods.map((m) => {
                const active = method === m.v
                return (
                  <button
                    key={m.v}
                    onClick={() => setMethod(m.v)}
                    className={`flex items-center gap-3 rounded-2xl border p-4 text-left transition ${active ? 'border-clay-500 bg-clay-50' : 'border-line hover:border-ink/30'}`}
                  >
                    <span className={`grid h-10 w-10 place-items-center rounded-full ${active ? 'bg-clay-600 text-paper' : 'bg-ink/5 text-ink-soft'}`}>
                      <m.icon className="h-5 w-5" />
                    </span>
                    <span className="flex-1">
                      <span className="block text-sm font-medium text-ink">{m.label}</span>
                      <span className="block text-xs text-ink-muted">{m.desc}</span>
                    </span>
                    {active && <Check className="h-5 w-5 text-clay-600" />}
                  </button>
                )
              })}
            </div>
          </section>
        </div>

        <div className="h-fit lg:sticky lg:top-28">
          <div className="surface p-6">
            <h2 className="font-display text-xl text-ink">Đơn hàng</h2>
            <div className="mt-4 flex gap-2">
              <input className="field" placeholder="Mã giảm giá" value={couponCode} onChange={(e) => setCouponCode(e.target.value)} />
              <button onClick={applyCoupon} className="btn-outline shrink-0 px-4 py-2">Áp dụng</button>
            </div>
            <div className="mt-5 space-y-2.5 text-sm">
              <Row label="Tạm tính" value={currency(subTotal)} />
              {discount > 0 && <Row label="Giảm giá" value={`- ${currency(discount)}`} accent />}
              <Row label="Phí vận chuyển" value={shippingFee === 0 ? 'Miễn phí' : currency(shippingFee)} />
              <div className="mt-2 border-t border-line pt-3">
                <div className="flex items-baseline justify-between">
                  <span className="font-medium text-ink">Tổng cộng</span>
                  <span className="font-display text-2xl text-clay-600">{currency(total)}</span>
                </div>
              </div>
            </div>
            <button onClick={place} disabled={placing || subTotal === 0} className="btn-dark mt-6 w-full">
              {placing ? 'Đang xử lý...' : 'Đặt hàng'}
            </button>
            <p className="mt-3 text-center text-xs text-ink-muted">Miễn phí vận chuyển cho đơn từ {currency(FREE_THRESHOLD)}</p>
          </div>
        </div>
      </div>
    </div>
  )
}

function Row({ label, value, accent }: { label: string; value: string; accent?: boolean }) {
  return (
    <div className="flex justify-between">
      <span className="text-ink-muted">{label}</span>
      <span className={accent ? 'font-medium text-clay-600' : 'text-ink'}>{value}</span>
    </div>
  )
}
