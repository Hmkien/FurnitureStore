import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowUpRight } from 'lucide-react'
import { catalogApi } from '../api/services'
import { img, dateOnly } from '../lib/format'
import type { Post } from '../types'

export default function NewsPage() {
  const [posts, setPosts] = useState<Post[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    catalogApi.posts(24).then(setPosts).catch(() => {}).finally(() => setLoading(false))
  }, [])

  const [featured, ...rest] = posts

  return (
    <div>
      <div className="border-b border-line bg-paper">
        <div className="u-container py-10">
          <span className="eyebrow">Tạp chí</span>
          <h1 className="display mt-2 text-4xl text-ink">Tin tức &amp; cảm hứng</h1>
          <p className="mt-2 max-w-xl text-ink-soft">Xu hướng nội thất, mẹo bài trí và câu chuyện đằng sau từng thiết kế.</p>
        </div>
      </div>

      <div className="u-container py-10">
        {loading ? (
          <p className="text-ink-muted">Đang tải...</p>
        ) : posts.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-line py-20 text-center text-ink-muted">Chưa có bài viết.</div>
        ) : (
          <>
            {featured && (
              <Link to={`/tin-tuc/${featured.slug}`} className="group mb-10 grid gap-6 overflow-hidden rounded-4xl border border-line bg-paper md:grid-cols-2">
                <div className="aspect-[16/11] overflow-hidden bg-[#ece4d8]">
                  {featured.thumbnailUrl && <img src={img(featured.thumbnailUrl)} alt={featured.title} className="h-full w-full object-cover transition duration-700 group-hover:scale-105" />}
                </div>
                <div className="flex flex-col justify-center p-7">
                  {featured.category && <span className="eyebrow">{featured.category}</span>}
                  <h2 className="display mt-3 text-3xl text-ink">{featured.title}</h2>
                  {featured.summary && <p className="mt-3 line-clamp-3 text-ink-soft">{featured.summary}</p>}
                  <span className="mt-5 inline-flex items-center gap-1.5 text-sm font-medium text-clay-600">
                    Đọc tiếp <ArrowUpRight className="h-4 w-4 transition group-hover:translate-x-0.5" />
                  </span>
                </div>
              </Link>
            )}

            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {rest.map((post) => (
                <Link key={post.id} to={`/tin-tuc/${post.slug}`} className="group overflow-hidden rounded-2xl border border-line bg-paper">
                  <div className="aspect-[16/10] overflow-hidden bg-[#ece4d8]">
                    {post.thumbnailUrl && <img src={img(post.thumbnailUrl)} alt={post.title} className="h-full w-full object-cover transition duration-700 group-hover:scale-105" />}
                  </div>
                  <div className="p-5">
                    <div className="flex items-center gap-2 text-xs text-ink-muted">
                      {post.category && <span className="eyebrow">{post.category}</span>}
                      {post.created && <span>· {dateOnly(post.created)}</span>}
                    </div>
                    <h3 className="mt-2 line-clamp-2 font-display text-lg text-ink transition group-hover:text-clay-600">{post.title}</h3>
                    {post.summary && <p className="mt-2 line-clamp-2 text-sm text-ink-soft">{post.summary}</p>}
                  </div>
                </Link>
              ))}
            </div>
          </>
        )}
      </div>
    </div>
  )
}
