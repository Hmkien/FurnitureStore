import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { bannersApi } from '@/api/services'
import { mediaUrl } from '@/lib/format'
import type { Banner } from '@/types'

const columns: Column<Banner>[] = [
  { header: 'Tiêu đề', cell: (b) => <span className="font-medium">{b.title}</span> },
  {
    header: 'Ảnh',
    className: 'w-24',
    cell: (b) => (b.imageUrl ? <img src={mediaUrl(b.imageUrl)} className="h-10 w-20 rounded-md border object-cover" /> : null),
  },
  { header: 'Vị trí', cell: (b) => b.position ?? '—' },
  { header: 'Thứ tự', cell: (b) => b.sortOrder },
]

const fields: CrudField[] = [
  { name: 'title', label: 'Tiêu đề', required: true, full: true },
  { name: 'imageUrl', label: 'Ảnh', type: 'image', folder: 'banners', required: true },
  { name: 'position', label: 'Vị trí', placeholder: 'home_top, sidebar...' },
  { name: 'sortOrder', label: 'Thứ tự', type: 'number' },
  { name: 'linkUrl', label: 'Link', full: true },
  { name: 'startDate', label: 'Bắt đầu', type: 'datetime' },
  { name: 'endDate', label: 'Kết thúc', type: 'datetime' },
]

export default function BannersPage() {
  return <ResourceCrud<Banner> title="Banner" api={bannersApi} columns={columns} fields={fields} searchIn={['Title', 'Position']} syncResource="banner" defaults={{ sortOrder: 0 }} />
}
