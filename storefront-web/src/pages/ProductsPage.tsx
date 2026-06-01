import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Search, SlidersHorizontal } from 'lucide-react'
import { catalogApi, type ProductQuery } from '../api/services'
import ProductCard from '../components/ProductCard'
import { StatusEntity, type Product, type Category } from '../types'

type Sort = 'newest' | 'price-asc' | 'price-desc'
const sortMap: Record<Sort, Pick<ProductQuery, 'sortBy' | 'sortDesc'>> = {
  newest: { sortBy: 'Created', sortDesc: true },
  'price-asc': { sortBy: 'price', sortDesc: false },
  'price-desc': { sortBy: 'price', sortDesc: true },
}

export default function ProductsPage() {
  const [params, setParams] = useSearchParams()
  const [cats, setCats] = useState<Category[]>([])
  const [products, setProducts] = useState<Product[]>([])
  const [loading, setLoading] = useState(false)
  const [sort, setSort] = useState<Sort>('newest')

  const categoryId = params.get('category') ?? undefined
  const keyword = params.get('keyword') ?? ''
  const [search, setSearch] = useState(keyword)
  useEffect(() => setSearch(keyword), [keyword])

  const activeCat = useMemo(() => cats.find((c) => c.id === categoryId), [cats, categoryId])

  useEffect(() => {
    catalogApi.categories().then((c) => setCats(c.filter((x) => x.status === StatusEntity.Approved))).catch(() => {})
  }, [])

  useEffect(() => {
    setLoading(true)
    catalogApi
      .products({ pageNumber: 1, pageSize: 48, keyword: keyword || undefined, categoryId, ...sortMap[sort] })
      .then((r) => setProducts(r.data.filter((p) => p.status === StatusEntity.Approved)))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [categoryId, keyword, sort])

  const setParam = (key: string, value?: string) => {
    const next = new URLSearchParams(params)
    if (value) next.set(key, value)
    else next.delete(key)
    setParams(next)
  }

  return (
    <div>
      {/* Tiêu đề trang */}
      <div className="border-b border-line bg-paper">
        <div className="u-container py-10">
          <span className="eyebrow">Bộ sưu tập</span>
          <h1 className="display mt-2 text-4xl text-ink">{activeCat ? activeCat.name : keyword ? `Kết quả cho “${keyword}”` : 'Tất cả sản phẩm'}</h1>
        </div>
      </div>

      <div className="u-container grid grid-cols-1 gap-10 py-10 lg:grid-cols-[230px_1fr]">
        {/* Sidebar */}
        <aside className="space-y-7 lg:sticky lg:top-28 lg:h-fit">
          <form
            onSubmit={(e) => { e.preventDefault(); setParam('keyword', search.trim() || undefined) }}
            className="flex items-center rounded-full border border-line bg-paper px-3.5 py-2.5"
          >
            <Search className="h-4 w-4 text-ink-muted" />
            <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Tìm sản phẩm..." className="w-full bg-transparent px-2 text-sm outline-none" />
          </form>

          <div>
            <h3 className="eyebrow mb-3">Danh mục</h3>
            <div className="space-y-0.5">
              <CatLink active={!categoryId} onClick={() => setParam('category', undefined)}>Tất cả</CatLink>
              {cats.map((c) => (
                <CatLink key={c.id} active={categoryId === c.id} onClick={() => setParam('category', c.id)}>
                  {c.name}
                </CatLink>
              ))}
            </div>
          </div>
        </aside>

        {/* Lưới sản phẩm */}
        <div>
          <div className="mb-6 flex items-center justify-between gap-3">
            <span className="text-sm text-ink-muted">{loading ? 'Đang tải...' : `${products.length} sản phẩm`}</span>
            <label className="flex items-center gap-2 text-sm">
              <SlidersHorizontal className="h-4 w-4 text-ink-muted" />
              <select
                value={sort}
                onChange={(e) => setSort(e.target.value as Sort)}
                className="rounded-full border border-line bg-paper px-3 py-2 text-sm outline-none focus:border-clay-400"
              >
                <option value="newest">Mới nhất</option>
                <option value="price-asc">Giá: thấp → cao</option>
                <option value="price-desc">Giá: cao → thấp</option>
              </select>
            </label>
          </div>

          {loading ? (
            <div className="grid grid-cols-2 gap-5 sm:grid-cols-3">
              {Array.from({ length: 6 }).map((_, i) => (
                <div key={i} className="aspect-[4/5] animate-pulse rounded-2xl bg-[#ece4d8]" />
              ))}
            </div>
          ) : products.length === 0 ? (
            <div className="rounded-2xl border border-dashed border-line py-20 text-center text-ink-muted">
              Không tìm thấy sản phẩm phù hợp.
            </div>
          ) : (
            <div className="grid grid-cols-2 gap-5 sm:grid-cols-3">
              {products.map((p) => (
                <ProductCard key={p.id} p={p} />
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

function CatLink({ active, onClick, children }: { active: boolean; onClick: () => void; children: React.ReactNode }) {
  return (
    <button
      onClick={onClick}
      className={`block w-full rounded-lg px-3 py-2 text-left text-sm transition ${
        active ? 'bg-clay-50 font-medium text-clay-700' : 'text-ink-soft hover:bg-ink/5 hover:text-ink'
      }`}
    >
      {children}
    </button>
  )
}
