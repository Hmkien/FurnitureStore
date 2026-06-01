import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { RbacSyncButton } from '@/components/seed-buttons'
import { permissionsApi } from '@/api/services'
import type { Permission } from '@/types'

const columns: Column<Permission>[] = [
  { header: 'Tên quyền', cell: (p) => <span className="font-medium">{p.permisionName}</span> },
  {
    header: 'Mã quyền',
    cell: (p) => <code className="rounded bg-muted px-1.5 py-0.5 text-xs">{p.permisionCode}</code>,
  },
  { header: 'Mô tả', cell: (p) => <span className="text-muted-foreground">{p.description}</span> },
]

const fields: CrudField[] = [
  { name: 'permisionName', label: 'Tên quyền', required: true, full: true, placeholder: 'VD: Xem sản phẩm' },
  { name: 'permisionCode', label: 'Mã quyền', required: true, placeholder: 'VD: PRODUCT_VIEW' },
  { name: 'description', label: 'Mô tả', type: 'textarea' },
]

export default function PermissionsPage() {
  return (
    <ResourceCrud<Permission>
      title="Quyền"
      api={permissionsApi}
      columns={columns}
      fields={fields}
      searchIn={['PermisionName', 'PermisionCode']}
      headerActions={(reload) => <RbacSyncButton onDone={reload} />}
    />
  )
}
