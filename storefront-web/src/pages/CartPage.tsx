import { Link, useNavigate } from 'react-router-dom'
import { Minus, Plus, Trash2, ShoppingBag, ArrowRight } from 'lucide-react'
import { useCart } from '../context/CartContext'
import { currency, img } from '../lib/format'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'

export default function CartPage() {
  const { cart, update, remove } = useCart()
  const navigate = useNavigate()
  const toast = useToast()

  const items = cart?.items ?? []
  const change = async (fn: () => Promise<void>) => {
    try {
      await fn()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  if (items.length === 0) return <EmptyState text="Giỏ hàng của bạn đang trống." cta="Khám phá sản phẩm" to="/products" />

  return (
    <div className="u-container py-10">
      <span className="eyebrow">Giỏ hàng</span>
      <h1 className="display mt-2 text-4xl text-ink">Giỏ của bạn</h1>

      <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-[1fr_360px]">
        <div className="divide-y divide-line overflow-hidden rounded-3xl border border-line bg-paper">
          {items.map((i) => (
            <div key={i.id} className="flex items-center gap-4 p-4 sm:p-5">
              <div className="h-24 w-24 shrink-0 overflow-hidden rounded-2xl bg-[#ece4d8]">
                {i.imageUrl && <img src={img(i.imageUrl)} className="h-full w-full object-cover" />}
              </div>
              <div className="min-w-0 flex-1">
                <div className="truncate font-medium text-ink">{i.productName}</div>
                <div className="text-sm text-ink-muted">{[i.size, i.material, i.color].filter(Boolean).join(' / ')}</div>
                <div className="mt-1.5 font-display text-lg text-clay-600">{currency(i.price)}</div>
              </div>
              <div className="flex flex-col items-end gap-2.5">
                <button onClick={() => change(() => remove(i.id))} className="text-ink-muted transition hover:text-red-600" title="Xóa">
                  <Trash2 className="h-4.5 w-4.5" />
                </button>
                <div className="flex items-center rounded-full border border-line">
                  <button onClick={() => change(() => update(i.id, Math.max(1, i.quantity - 1)))} className="grid h-9 w-9 place-items-center text-ink-soft hover:text-ink"><Minus className="h-3.5 w-3.5" /></button>
                  <span className="w-8 text-center text-sm font-medium">{i.quantity}</span>
                  <button onClick={() => change(() => update(i.id, i.quantity + 1))} className="grid h-9 w-9 place-items-center text-ink-soft hover:text-ink"><Plus className="h-3.5 w-3.5" /></button>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="h-fit lg:sticky lg:top-28">
          <div className="surface p-6">
            <h2 className="font-display text-xl text-ink">Tóm tắt đơn</h2>
            <div className="mt-4 flex justify-between text-sm">
              <span className="text-ink-muted">Tạm tính ({cart?.totalItems} sản phẩm)</span>
              <span className="font-semibold text-ink">{currency(cart?.subTotal)}</span>
            </div>
            <div className="mt-2 flex justify-between text-sm">
              <span className="text-ink-muted">Phí vận chuyển</span>
              <span className="text-ink-soft">Tính khi thanh toán</span>
            </div>
            <button onClick={() => navigate('/checkout')} className="btn-dark mt-6 w-full">
              Tiến hành thanh toán <ArrowRight className="h-4 w-4" />
            </button>
            <Link to="/products" className="mt-3 block text-center text-sm text-ink-muted hover:text-clay-600">Tiếp tục mua sắm</Link>
          </div>
        </div>
      </div>
    </div>
  )
}

function EmptyState({ text, cta, to }: { text: string; cta: string; to: string }) {
  return (
    <div className="u-container py-24 text-center">
      <span className="mx-auto grid h-16 w-16 place-items-center rounded-full bg-paper text-ink-muted shadow-soft">
        <ShoppingBag className="h-7 w-7" />
      </span>
      <p className="mt-5 text-ink-soft">{text}</p>
      <Link to={to} className="btn-primary mt-6">{cta}</Link>
    </div>
  )
}
