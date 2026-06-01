import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, Truck, ShieldCheck, Leaf, Headphones, Ticket, Copy } from 'lucide-react'
import { catalogApi } from '../api/services'
import { img, currency } from '../lib/format'
import { useToast } from '../context/Toast'
import ProductCard from '../components/ProductCard'
import { StatusEntity, DiscountType, type Product, type Category, type Slide, type Post, type Coupon } from '../types'

const values = [
  { icon: Truck, title: 'Giao hàng tận nơi', desc: 'Miễn phí cho đơn từ 5 triệu' },
  { icon: ShieldCheck, title: 'Bảo hành 24 tháng', desc: 'An tâm dài lâu' },
  { icon: Leaf, title: 'Vật liệu bền vững', desc: 'Gỗ tự nhiên tuyển chọn' },
  { icon: Headphones, title: 'Tư vấn tận tâm', desc: 'Hỗ trợ 7 ngày/tuần' },
]

export default function HomePage() {
  const toast = useToast()
  const [slides, setSlides] = useState<Slide[]>([])
  const [cats, setCats] = useState<Category[]>([])
  const [products, setProducts] = useState<Product[]>([])
  const [posts, setPosts] = useState<Post[]>([])
  const [coupons, setCoupons] = useState<Coupon[]>([])

  useEffect(() => {
    catalogApi.slides().then(setSlides).catch(() => {})
    catalogApi.categories().then((c) => setCats(c.filter((x) => x.status === StatusEntity.Approved))).catch(() => {})
    catalogApi
      .products({ pageNumber: 1, pageSize: 8, sortBy: 'Created', sortDesc: true })
      .then((r) => setProducts(r.data.filter((p) => p.status === StatusEntity.Approved)))
      .catch(() => {})
    catalogApi.posts(6).then(setPosts).catch(() => {})
    catalogApi.coupons().then(setCoupons).catch(() => {})
  }, [])

  const copyCode = async (code: string) => {
    try {
      await navigator.clipboard.writeText(code)
      toast.success(`Đã sao chép mã ${code}`)
    } catch {
      toast.info(`Mã: ${code}`)
    }
  }

  const hero = slides[0]
  const heroImg = hero?.imageUrl ?? products[0]?.thumbnailUrl

  return (
    <div className="pb-4">
      {/* ===== Hero ===== */}
      <section className="u-container pt-8 sm:pt-12">
        <div className="grid items-stretch gap-6 lg:grid-cols-[1.05fr_.95fr]">
          <div className="flex flex-col justify-center animate-fade-up">
            <span className="eyebrow">Bộ sưu tập 2026</span>
            <h1 className="display mt-5 text-[2.7rem] text-ink sm:text-6xl">
              Nội thất kể câu chuyện
              <br />
              <span className="italic text-clay-600">tổ ấm</span> của bạn
            </h1>
            <p className="mt-6 max-w-md text-[15px] leading-relaxed text-ink-soft">
              Những thiết kế tinh giản, vật liệu tự nhiên và đường nét bền bỉ theo năm tháng —
              {hero?.caption ? ` ${hero.caption}` : ' kiến tạo không gian sống an yên.'}
            </p>
            <div className="mt-8 flex flex-wrap items-center gap-3">
              <Link to="/products" className="btn-primary">
                Khám phá bộ sưu tập <ArrowRight className="h-4 w-4" />
              </Link>
              <Link to={hero?.linkUrl ?? '/products'} className="btn-outline">Ưu đãi mùa này</Link>
            </div>
            <div className="mt-10 flex items-center gap-8">
              <Stat value="500+" label="Mẫu thiết kế" />
              <span className="h-10 w-px bg-line" />
              <Stat value="12K+" label="Khách hài lòng" />
              <span className="h-10 w-px bg-line" />
              <Stat value="24th" label="Bảo hành" />
            </div>
          </div>

          <div className="relative animate-fade-in">
            <div className="aspect-[4/5] overflow-hidden rounded-4xl bg-[#ece4d8] shadow-soft lg:aspect-auto lg:h-full">
              {heroImg ? (
                <img src={img(heroImg)} alt={hero?.title ?? 'Furnitura'} className="h-full w-full object-cover" />
              ) : (
                <div className="grid h-full place-items-center font-display text-2xl text-ink-muted">Furnitura</div>
              )}
            </div>
            <div className="absolute -bottom-5 -left-5 hidden rounded-2xl border border-line bg-paper px-5 py-4 shadow-soft sm:block">
              <div className="font-display text-xl text-clay-600">Thiết kế thủ công</div>
              <div className="text-xs text-ink-muted">Tinh xảo trong từng chi tiết</div>
            </div>
          </div>
        </div>
      </section>

      {/* ===== Giá trị ===== */}
      <section className="u-container mt-16">
        <div className="grid grid-cols-2 gap-px overflow-hidden rounded-3xl border border-line bg-line lg:grid-cols-4">
          {values.map((v) => (
            <div key={v.title} className="flex items-start gap-3 bg-paper p-5">
              <span className="grid h-10 w-10 shrink-0 place-items-center rounded-full bg-clay-50 text-clay-600">
                <v.icon className="h-5 w-5" />
              </span>
              <div>
                <div className="text-sm font-semibold text-ink">{v.title}</div>
                <div className="text-xs text-ink-muted">{v.desc}</div>
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* ===== Mã giảm giá ===== */}
      {coupons.length > 0 && (
        <section className="u-container mt-20">
          <SectionHeader eyebrow="Ưu đãi" title="Mã giảm giá hôm nay" />
          <div className="mt-7 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {coupons.map((c) => {
              const value = c.discountType === DiscountType.Percentage ? `-${c.discountValue}%` : `-${currency(c.discountValue)}`
              return (
                <div key={c.code} className="relative flex items-stretch overflow-hidden rounded-2xl border border-clay-200 bg-clay-50">
                  {/* Cuống vé */}
                  <div className="flex w-28 shrink-0 flex-col items-center justify-center border-r border-dashed border-clay-300 bg-clay-600 px-3 py-5 text-paper">
                    <Ticket className="h-5 w-5 opacity-80" />
                    <div className="mt-1 font-display text-2xl leading-none">{value}</div>
                  </div>
                  <div className="flex flex-1 flex-col justify-center p-4">
                    <div className="font-mono text-sm font-bold tracking-wider text-ink">{c.code}</div>
                    <p className="mt-0.5 line-clamp-1 text-xs text-ink-soft">
                      {c.description || (c.minOrderAmount > 0 ? `Đơn từ ${currency(c.minOrderAmount)}` : 'Áp dụng mọi đơn')}
                      {c.discountType === DiscountType.Percentage && c.maxDiscount > 0 && ` · tối đa ${currency(c.maxDiscount)}`}
                    </p>
                    <button
                      onClick={() => copyCode(c.code)}
                      className="mt-2 inline-flex w-fit items-center gap-1.5 rounded-full bg-ink/5 px-3 py-1 text-xs font-medium text-ink transition hover:bg-clay-600 hover:text-paper"
                    >
                      <Copy className="h-3.5 w-3.5" /> Sao chép mã
                    </button>
                  </div>
                </div>
              )
            })}
          </div>
        </section>
      )}

      {/* ===== Danh mục ===== */}
      {cats.length > 0 && (
        <section className="u-container mt-20">
          <SectionHeader eyebrow="Mua theo không gian" title="Danh mục nổi bật" to="/products" />
          <div className="mt-7 grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
            {cats.slice(0, 8).map((c) => (
              <Link
                key={c.id}
                to={`/products?category=${c.id}`}
                className="group relative flex aspect-[3/2] items-end overflow-hidden rounded-2xl bg-[#ece4d8] p-4"
              >
                {c.imageUrl && (
                  <img
                    src={img(c.imageUrl)}
                    alt={c.name}
                    className="absolute inset-0 h-full w-full object-cover transition duration-700 group-hover:scale-105"
                  />
                )}
                <span className="absolute inset-0 bg-gradient-to-t from-ink/70 via-ink/10 to-transparent" />
                <span className="relative font-medium text-paper">{c.name}</span>
              </Link>
            ))}
          </div>
        </section>
      )}

      {/* ===== Sản phẩm mới ===== */}
      <section className="u-container mt-20">
        <SectionHeader eyebrow="Vừa ra mắt" title="Sản phẩm mới" to="/products" />
        {products.length === 0 ? (
          <p className="mt-7 text-ink-muted">Chưa có sản phẩm.</p>
        ) : (
          <div className="mt-7 grid grid-cols-2 gap-5 sm:grid-cols-3 lg:grid-cols-4">
            {products.map((p) => (
              <ProductCard key={p.id} p={p} />
            ))}
          </div>
        )}
      </section>

      {/* ===== Băng CTA ===== */}
      <section className="u-container mt-20">
        <div className="relative overflow-hidden rounded-4xl bg-ink px-8 py-14 text-center text-paper sm:px-16 sm:py-20">
          <div className="absolute -right-16 -top-16 h-64 w-64 rounded-full bg-clay-600/30 blur-3xl" />
          <div className="absolute -bottom-20 -left-10 h-64 w-64 rounded-full bg-clay-500/20 blur-3xl" />
          <span className="eyebrow text-clay-300">Furnitura Studio</span>
          <h2 className="display mx-auto mt-4 max-w-2xl text-3xl sm:text-5xl">
            Biến ngôi nhà thành nơi đáng để trở về
          </h2>
          <p className="mx-auto mt-4 max-w-md text-sm text-paper/70">
            Khám phá những thiết kế được tuyển chọn kỹ lưỡng cho không gian sống của bạn.
          </p>
          <Link to="/products" className="btn mt-8 inline-flex bg-paper text-ink hover:bg-clay-50">
            Bắt đầu mua sắm <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      </section>

      {/* ===== Tin nổi bật ===== */}
      {posts.length > 0 && (
        <section className="u-container mt-20">
          <SectionHeader eyebrow="Cảm hứng" title="Tin nổi bật" to="/tin-tuc" />
          <div className="mt-7 grid gap-6 sm:grid-cols-3">
            {posts.slice(0, 3).map((post) => (
              <Link key={post.id} to={`/tin-tuc/${post.slug}`} className="group overflow-hidden rounded-2xl border border-line bg-paper">
                <div className="aspect-[16/10] overflow-hidden bg-[#ece4d8]">
                  {post.thumbnailUrl && (
                    <img src={img(post.thumbnailUrl)} alt={post.title} className="h-full w-full object-cover transition duration-700 group-hover:scale-105" />
                  )}
                </div>
                <div className="p-5">
                  {post.category && <span className="eyebrow">{post.category}</span>}
                  <h3 className="mt-2 line-clamp-2 font-display text-lg text-ink transition group-hover:text-clay-600">{post.title}</h3>
                  {post.summary && <p className="mt-2 line-clamp-2 text-sm text-ink-soft">{post.summary}</p>}
                </div>
              </Link>
            ))}
          </div>
        </section>
      )}
    </div>
  )
}

function Stat({ value, label }: { value: string; label: string }) {
  return (
    <div>
      <div className="font-display text-2xl text-ink">{value}</div>
      <div className="text-xs uppercase tracking-wide text-ink-muted">{label}</div>
    </div>
  )
}

function SectionHeader({ eyebrow, title, to }: { eyebrow: string; title: string; to?: string }) {
  return (
    <div className="flex items-end justify-between gap-4">
      <div>
        <span className="eyebrow">{eyebrow}</span>
        <h2 className="display mt-2 text-3xl text-ink sm:text-4xl">{title}</h2>
      </div>
      {to && (
        <Link to={to} className="group hidden shrink-0 items-center gap-1.5 text-sm font-medium text-ink-soft hover:text-clay-600 sm:flex">
          Xem tất cả <ArrowRight className="h-4 w-4 transition group-hover:translate-x-1" />
        </Link>
      )}
    </div>
  )
}
