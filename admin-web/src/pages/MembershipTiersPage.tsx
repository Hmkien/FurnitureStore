import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { tiersApi } from '@/api/services'
import { currency } from '@/lib/format'
import type { MembershipTier } from '@/types'

const columns: Column<MembershipTier>[] = [
  { header: 'Tên hạng', cell: (t) => <span className="font-medium">{t.name}</span> },
  { header: 'Chi tiêu tối thiểu', cell: (t) => currency(t.minSpending) },
  { header: 'Chiết khấu', cell: (t) => `${t.discountPercent}%` },
  { header: 'Mô tả', cell: (t) => <span className="text-muted-foreground">{t.description}</span> },
]

const fields: CrudField[] = [
  { name: 'name', label: 'Tên hạng', required: true, full: true },
  { name: 'minSpending', label: 'Chi tiêu tối thiểu', type: 'number' },
  { name: 'discountPercent', label: 'Chiết khấu (%)', type: 'number' },
  { name: 'description', label: 'Mô tả', type: 'textarea' },
]

export default function MembershipTiersPage() {
  return <ResourceCrud<MembershipTier> title="Hạng thành viên" api={tiersApi} columns={columns} fields={fields} searchIn={['Name']} syncResource="membershiptier" defaults={{ minSpending: 0, discountPercent: 0 }} />
}
