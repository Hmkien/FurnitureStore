import { Link } from 'react-router-dom'
import { Package, ArrowUpRight } from 'lucide-react'
import { currency, img } from '../lib/format'
import type { Product } from '../types'

export default function ProductCard({ p }: { p: Product }) {
  return (
    <Link to={`/products/${p.id}`} className="group block">
      <div className="relative aspect-[4/5] overflow-hidden rounded-2xl bg-[#ece4d8]">
        {p.thumbnailUrl ? (
          <img
            src={img(p.thumbnailUrl)}
            alt={p.name}
            loading="lazy"
            className="h-full w-full object-cover transition-transform duration-[800ms] ease-[cubic-bezier(.22,1,.36,1)] group-hover:scale-[1.06]"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-ink-muted/40"><Package className="h-10 w-10" /></div>
        )}

        {p.style && (
          <span className="absolute left-3 top-3 rounded-full bg-paper/85 px-2.5 py-1 text-[11px] font-medium text-ink backdrop-blur">
            {p.style}
          </span>
        )}

        <span className="absolute bottom-3 right-3 grid h-10 w-10 translate-y-3 place-items-center rounded-full bg-ink text-paper opacity-0 transition-all duration-300 group-hover:translate-y-0 group-hover:opacity-100">
          <ArrowUpRight className="h-5 w-5" />
        </span>
      </div>

      <div className="mt-3.5 px-0.5">
        <h3 className="line-clamp-1 text-[15px] font-medium text-ink transition-colors group-hover:text-clay-600">{p.name}</h3>
        <div className="mt-1 flex items-baseline gap-2">
          <span className="font-display text-lg text-ink">{p.minPrice != null ? currency(p.minPrice) : 'Liên hệ'}</span>
        </div>
      </div>
    </Link>
  )
}
