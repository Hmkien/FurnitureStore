import { useState } from 'react'
import { ShieldCheck } from 'lucide-react'
import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { Button } from '@/components/ui/button'
import PermissionPickerDialog from '@/components/PermissionPickerDialog'
import { RbacSyncButton } from '@/components/seed-buttons'
import { rolesApi } from '@/api/services'
import type { Role } from '@/types'

const columns: Column<Role>[] = [
  { header: 'Tên vai trò', cell: (r) => <span className="font-medium">{r.name}</span> },
  {
    header: 'Mã vai trò',
    cell: (r) => <code className="rounded bg-muted px-1.5 py-0.5 text-xs">{r.roleCode}</code>,
  },
]

const fields: CrudField[] = [
  { name: 'name', label: 'Tên vai trò', required: true, full: true, placeholder: 'VD: Quản lý cửa hàng' },
  { name: 'roleCode', label: 'Mã vai trò', required: true, placeholder: 'VD: MANAGER' },
]

export default function RolesPage() {
  const [picking, setPicking] = useState<Role | null>(null)

  return (
    <>
      <ResourceCrud<Role>
        title="Vai trò"
        api={rolesApi}
        columns={columns}
        fields={fields}
        searchIn={['Name', 'RoleCode']}
        headerActions={(reload) => <RbacSyncButton onDone={reload} />}
        rowActions={(r) => (
          <Button variant="ghost" size="icon" title="Phân quyền" className="text-indigo-600" onClick={() => setPicking(r)}>
            <ShieldCheck className="h-4 w-4" />
          </Button>
        )}
      />
      <PermissionPickerDialog
        open={!!picking}
        onOpenChange={(v) => !v && setPicking(null)}
        roleId={picking?.id}
        roleName={picking?.name}
      />
    </>
  )
}
