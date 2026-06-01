import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ChevronRight, ArrowLeft } from 'lucide-react'
import { catalogApi } from '../api/services'
import { img, dateOnly } from '../lib/format'
import { useToast } from '../context/Toast'
import { getErrorMessage } from '../api/client'
import type { Post } from '../types'

export default function PostDetailPage() {
  const { slug } = useParams()
  const toast = useToast()
  const [post, setPost] = useState<Post | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!slug) return
    setLoading(true)
    catalogApi.postBySlug(slug)
      .then(setPost)
      .catch((e) => toast.error(getErrorMessage(e)))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [slug])

  if (loading) return <div className="u-container py-24 text-center text-ink-muted">Đang tải...</div>
  if (!post) return (
    <div className="u-container py-24 text-center">
      <p className="text-ink-soft">Không tìm thấy bài viết.</p>
      <Link to="/tin-tuc" className="btn-primary mt-6">Về trang tin tức</Link>
    </div>
  )

  return (
    <article className="u-container py-8">
      <nav className="flex items-center gap-1.5 text-sm text-ink-muted">
        <Link to="/" className="hover:text-ink">Trang chủ</Link>
        <ChevronRight className="h-3.5 w-3.5" />
        <Link to="/tin-tuc" className="hover:text-ink">Tin tức</Link>
        <ChevronRight className="h-3.5 w-3.5" />
        <span className="truncate text-ink">{post.title}</span>
      </nav>

      <header className="mx-auto mt-8 max-w-3xl text-center">
        {post.category && <span className="eyebrow">{post.category}</span>}
        <h1 className="display mt-3 text-4xl text-ink sm:text-5xl">{post.title}</h1>
        {post.created && <p className="mt-4 text-sm text-ink-muted">{dateOnly(post.created)}</p>}
      </header>

      {post.thumbnailUrl && (
        <div className="mx-auto mt-8 max-w-4xl overflow-hidden rounded-4xl bg-[#ece4d8]">
          <img src={img(post.thumbnailUrl)} alt={post.title} className="h-full w-full object-cover" />
        </div>
      )}

      <div className="mx-auto mt-10 max-w-3xl">
        {post.summary && <p className="mb-6 border-l-2 border-clay-400 pl-4 text-lg italic text-ink-soft">{post.summary}</p>}
        {post.content ? (
          <div className="prose-content text-[15px]" dangerouslySetInnerHTML={{ __html: post.content }} />
        ) : (
          <p className="text-ink-muted">Nội dung đang được cập nhật.</p>
        )}

        <div className="mt-12 border-t border-line pt-6">
          <Link to="/tin-tuc" className="inline-flex items-center gap-1.5 text-sm font-medium text-clay-600 hover:underline">
            <ArrowLeft className="h-4 w-4" /> Tất cả bài viết
          </Link>
        </div>
      </div>
    </article>
  )
}
