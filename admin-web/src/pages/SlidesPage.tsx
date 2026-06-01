import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { slidesApi } from '@/api/services'
import { mediaUrl } from '@/lib/format'
import type { Slide } from '@/types'

const columns: Column<Slide>[] = [
  { header: 'Tiêu đề', cell: (s) => <span className="font-medium">{s.title}</span> },
  {
    header: 'Ảnh',
    className: 'w-24',
    cell: (s) => (s.imageUrl ? <img src={mediaUrl(s.imageUrl)} className="h-10 w-20 rounded-md border object-cover" /> : null),
  },
  { header: 'Caption', cell: (s) => <span className="text-muted-foreground">{s.caption}</span> },
  { header: 'Thứ tự', cell: (s) => s.sortOrder },
]

const fields: CrudField[] = [
  { name: 'title', label: 'Tiêu đề', required: true, full: true },
  { name: 'imageUrl', label: 'Ảnh', type: 'image', folder: 'slides', required: true },
  { name: 'caption', label: 'Caption', full: true },
  { name: 'linkUrl', label: 'Link' },
  { name: 'sortOrder', label: 'Thứ tự', type: 'number' },
]

export default function SlidesPage() {
  return <ResourceCrud<Slide> title="Slide" api={slidesApi} columns={columns} fields={fields} searchIn={['Title']} syncResource="slide" defaults={{ sortOrder: 0 }} />
}
