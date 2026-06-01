import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { postsApi } from '@/api/services'
import { dateOnly } from '@/lib/format'
import type { Post } from '@/types'

const columns: Column<Post>[] = [
  { header: 'Tiêu đề', cell: (p) => <span className="font-medium">{p.title}</span> },
  { header: 'Danh mục', cell: (p) => p.category ?? '—' },
  {
    header: 'Xuất bản',
    cell: (p) =>
      p.isPublished ? (
        <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs text-emerald-700">Đã đăng</span>
      ) : (
        <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600">Nháp</span>
      ),
  },
  { header: 'Ngày tạo', cell: (p) => dateOnly(p.created) },
]

const fields: CrudField[] = [
  { name: 'title', label: 'Tiêu đề', required: true, full: true },
  { name: 'category', label: 'Danh mục' },
  { name: 'author', label: 'Tác giả' },
  { name: 'thumbnailUrl', label: 'Ảnh đại diện', type: 'image', folder: 'posts' },
  { name: 'summary', label: 'Tóm tắt', type: 'textarea' },
  { name: 'content', label: 'Nội dung', type: 'richtext' },
  { name: 'isPublished', label: 'Xuất bản', type: 'switch' },
]

export default function PostsPage() {
  return <ResourceCrud<Post> title="Tin bài" api={postsApi} columns={columns} fields={fields} searchIn={['Title', 'Category']} syncResource="post" dialogWide />
}
