import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { categoriesApi } from '@/api/services'
import { dateOnly, mediaUrl } from '@/lib/format'
import type { Category } from '@/types'

const columns: Column<Category>[] = [
  { header: 'Tên', cell: (c) => <span className="font-medium">{c.name}</span> },
  {
    header: 'Ảnh',
    className: 'w-20',
    cell: (c) =>
      c.imageUrl ? (
        <img src={mediaUrl(c.imageUrl)} className="h-10 w-10 rounded-md border object-cover" />
      ) : (
        <div className="h-10 w-10 rounded-md bg-muted" />
      ),
  },
  { header: 'Slug', cell: (c) => <span className="text-muted-foreground">{c.slug}</span> },
  { header: 'Ngày tạo', cell: (c) => dateOnly(c.created) },
]

const fields: CrudField[] = [
  { name: 'name', label: 'Tên danh mục', required: true },
  { name: 'slug', label: 'Slug (để trống tự sinh)' },
  { name: 'imageUrl', label: 'Ảnh', type: 'image', folder: 'categories' },
  { name: 'description', label: 'Mô tả', type: 'textarea' },
]

export default function CategoriesPage() {
  return <ResourceCrud<Category> title="Danh mục" api={categoriesApi} columns={columns} fields={fields} searchIn={['Name', 'Slug']} syncResource="category" />
}
