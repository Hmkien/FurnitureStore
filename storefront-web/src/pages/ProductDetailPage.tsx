import { useEffect, useState } from 'react'
import { Link, useParams, useNavigate } from 'react-router-dom'
import { Star, ShoppingBag, Minus, Plus, Truck, ShieldCheck, RotateCcw, ChevronRight } from 'lucide-react'
import { catalogApi, reviewApi } from '../api/services'
import { currency, img, dateOnly } from '../lib/format'
import { useCart } from '../context/CartContext'
import { useAuth } from '../context/AuthContext'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import type { ProductDetail, ProductReviewSummary, ProductVariant } from '../types'

export default function ProductDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const toast = useToast()
  const { add } = useCart()
  const { user } = useAuth()

  const [p, setP] = useState<ProductDetail | null>(null)
  const [reviews, setReviews] = useState<ProductReviewSummary | null>(null)
  const [activeImg, setActiveImg] = useState('')
  const [variant, setVariant] = useState<ProductVariant | null>(null)
  const [qty, setQty] = useState(1)
  const [adding, setAdding] = useState(false)

  const [rating, setRating] = useState(5)
  const [comment, setComment] = useState('')
  const [otp, setOtp] = useState('')

  useEffect(() => {
    if (!id) return
    setQty(1)
    catalogApi.productDetail(id).then((d) => {
      setP(d)
      setActiveImg(d.images.find((i) => i.isPrimary)?.imageUrl ?? d.images[0]?.imageUrl ?? '')
      setVariant(d.variants[0] ?? null)
    }).catch((e) => toast.error(getErrorMessage(e)))
    reviewApi.byProduct(id).then(setReviews).catch(() => {})
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id])

  if (!p) return <div className="u-container py-24 text-center text-ink-muted">Đang tải...</div>

  const addToCart = async () => {
    if (!variant) return toast.error('Vui lòng chọn phiên bản')
    setAdding(true)
    try {
      await add(variant.id, qty)
      toast.success('Đã thêm vào giỏ hàng')
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setAdding(false)
    }
  }

  const getOtp = async () => {
    if (!user) return navigate('/login')
    try {
      const res = await reviewApi.requestOtp(p.id)
      setOtp(res.otp)
      toast.info('Mã OTP: ' + res.otp)
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }
  const submitReview = async () => {
    if (!user) return navigate('/login')
    if (!otp) return toast.error('Vui lòng lấy mã OTP')
    try {
      await reviewApi.create({ productId: p.id, rating, comment, otp })
      toast.success('Cảm ơn bạn đã đánh giá')
      setComment(''); setOtp('')
      setReviews(await reviewApi.byProduct(p.id))
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const outOfStock = (variant?.stockQuantity ?? 0) <= 0

  return (
    <div className="u-container py-8">
      {/* Breadcrumb */}
      <nav className="flex items-center gap-1.5 text-sm text-ink-muted">
        <Link to="/" className="hover:text-ink">Trang chủ</Link>
        <ChevronRight className="h-3.5 w-3.5" />
        <Link to="/products" className="hover:text-ink">Sản phẩm</Link>
        <ChevronRight className="h-3.5 w-3.5" />
        <span className="truncate text-ink">{p.name}</span>
      </nav>

      <div className="mt-6 grid grid-cols-1 gap-10 lg:grid-cols-2">
        {/* Gallery */}
        <div className="flex flex-col-reverse gap-4 sm:flex-row">
          {p.images.length > 1 && (
            <div className="flex gap-3 sm:flex-col">
              {p.images.map((im) => (
                <button
                  key={im.id}
                  onClick={() => setActiveImg(im.imageUrl)}
                  className={`h-16 w-16 shrink-0 overflow-hidden rounded-xl border-2 transition ${activeImg === im.imageUrl ? 'border-clay-500' : 'border-transparent opacity-70 hover:opacity-100'}`}
                >
                  <img src={img(im.imageUrl)} className="h-full w-full object-cover" />
                </button>
              ))}
            </div>
          )}
          <div className="aspect-square flex-1 overflow-hidden rounded-4xl bg-[#ece4d8]">
            {activeImg && <img src={img(activeImg)} alt={p.name} className="h-full w-full object-cover" />}
          </div>
        </div>

        {/* Info */}
        <div className="lg:py-2">
          <div className="flex flex-wrap items-center gap-2">
            {p.categoryName && <span className="chip">{p.categoryName}</span>}
            {p.style && <span className="chip">{p.style}</span>}
          </div>
          <h1 className="display mt-4 text-3xl text-ink sm:text-4xl">{p.name}</h1>

          {reviews && reviews.totalReviews > 0 && (
            <div className="mt-3 flex items-center gap-2 text-sm text-ink-soft">
              <span className="flex text-clay-500">
                {Array.from({ length: 5 }).map((_, i) => (
                  <Star key={i} className={`h-4 w-4 ${i < Math.round(reviews.averageRating) ? 'fill-current' : 'text-line'}`} />
                ))}
              </span>
              {reviews.averageRating.toFixed(1)} · {reviews.totalReviews} đánh giá
            </div>
          )}

          <div className="mt-5 font-display text-4xl text-clay-600">{currency(variant?.price ?? p.minPrice)}</div>
          {p.shortDescription && <p className="mt-4 leading-relaxed text-ink-soft">{p.shortDescription}</p>}

          {p.variants.length > 0 && (
            <div className="mt-7">
              <div className="eyebrow mb-3">Phiên bản</div>
              <div className="flex flex-wrap gap-2.5">
                {p.variants.map((v) => (
                  <button
                    key={v.id}
                    onClick={() => setVariant(v)}
                    className={`rounded-xl border px-4 py-2.5 text-sm transition ${
                      variant?.id === v.id ? 'border-ink bg-ink text-paper' : 'border-line bg-paper text-ink hover:border-ink/40'
                    }`}
                  >
                    {[v.size, v.material, v.color].filter(Boolean).join(' / ') || v.skuVariant}
                  </button>
                ))}
              </div>
              <p className={`mt-3 text-sm ${outOfStock ? 'text-red-600' : 'text-ink-muted'}`}>
                {outOfStock ? 'Tạm hết hàng' : `Còn ${variant?.stockQuantity} sản phẩm`}
              </p>
            </div>
          )}

          <div className="mt-7 flex flex-wrap items-center gap-3">
            <div className="flex items-center rounded-full border border-line bg-paper">
              <button onClick={() => setQty((q) => Math.max(1, q - 1))} className="grid h-11 w-11 place-items-center text-ink-soft hover:text-ink"><Minus className="h-4 w-4" /></button>
              <span className="w-10 text-center font-medium">{qty}</span>
              <button onClick={() => setQty((q) => q + 1)} className="grid h-11 w-11 place-items-center text-ink-soft hover:text-ink"><Plus className="h-4 w-4" /></button>
            </div>
            <button onClick={addToCart} disabled={adding || outOfStock} className="btn-dark flex-1 min-w-[200px]">
              <ShoppingBag className="h-5 w-5" /> {adding ? 'Đang thêm...' : outOfStock ? 'Hết hàng' : 'Thêm vào giỏ'}
            </button>
          </div>

          <div className="mt-8 grid grid-cols-3 gap-3 border-t border-line pt-6 text-center">
            <Trust icon={Truck} label="Giao tận nơi" />
            <Trust icon={ShieldCheck} label="BH 24 tháng" />
            <Trust icon={RotateCcw} label="Đổi trả 7 ngày" />
          </div>
        </div>
      </div>

      {/* Mô tả */}
      {p.longDescription && (
        <section className="mt-16 grid gap-8 lg:grid-cols-[200px_1fr]">
          <h2 className="display text-2xl text-ink">Mô tả chi tiết</h2>
          <div className="prose-content max-w-3xl" dangerouslySetInnerHTML={{ __html: p.longDescription }} />
        </section>
      )}

      {/* Đánh giá */}
      <section className="mt-16 grid gap-8 border-t border-line pt-12 lg:grid-cols-[200px_1fr]">
        <h2 className="display text-2xl text-ink">
          Đánh giá
          {reviews && reviews.totalReviews > 0 && (
            <span className="mt-1 block text-sm font-normal text-ink-muted">{reviews.averageRating.toFixed(1)}★ · {reviews.totalReviews} lượt</span>
          )}
        </h2>

        <div className="max-w-3xl">
          <div className="space-y-3">
            {reviews?.reviews.map((r) => (
              <div key={r.id} className="surface p-5">
                <div className="flex items-center gap-3">
                  <span className="grid h-9 w-9 place-items-center rounded-full bg-clay-50 font-semibold text-clay-600">{r.reviewerName.charAt(0)}</span>
                  <div>
                    <div className="text-sm font-medium text-ink">{r.reviewerName}</div>
                    <div className="flex items-center gap-2 text-xs text-ink-muted">
                      <span className="flex text-clay-500">{Array.from({ length: r.rating }).map((_, i) => <Star key={i} className="h-3 w-3 fill-current" />)}</span>
                      {dateOnly(r.created)}
                    </div>
                  </div>
                </div>
                {r.comment && <p className="mt-3 text-sm leading-relaxed text-ink-soft">{r.comment}</p>}
              </div>
            ))}
            {reviews && reviews.reviews.length === 0 && <p className="text-ink-muted">Chưa có đánh giá. Hãy là người đầu tiên!</p>}
          </div>

          <div className="surface mt-6 p-6">
            <h3 className="font-display text-lg text-ink">Viết đánh giá</h3>
            <p className="mt-1 text-xs text-ink-muted">Chỉ khách đã mua &amp; nhận hàng mới đánh giá được. Mã OTP demo sẽ tự điền.</p>
            <div className="mt-4 flex items-center gap-1">
              {[1, 2, 3, 4, 5].map((n) => (
                <button key={n} onClick={() => setRating(n)} aria-label={`${n} sao`}>
                  <Star className={`h-7 w-7 transition ${n <= rating ? 'fill-clay-500 text-clay-500' : 'text-line'}`} />
                </button>
              ))}
            </div>
            <textarea value={comment} onChange={(e) => setComment(e.target.value)} rows={3} placeholder="Cảm nhận của bạn về sản phẩm..." className="field mt-3 resize-none" />
            <div className="mt-3 flex flex-wrap items-center gap-2">
              <input value={otp} onChange={(e) => setOtp(e.target.value)} placeholder="Nhập mã OTP" className="field w-40" />
              <button onClick={getOtp} className="btn-outline">Lấy mã OTP</button>
              <button onClick={submitReview} className="btn-primary">Gửi đánh giá</button>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}

function Trust({ icon: Icon, label }: { icon: typeof Truck; label: string }) {
  return (
    <div className="flex flex-col items-center gap-1.5 text-xs text-ink-soft">
      <Icon className="h-5 w-5 text-clay-600" />
      {label}
    </div>
  )
}
