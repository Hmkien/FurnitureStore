import { useEffect, useMemo, useState } from 'react'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { useToast } from './Toast'
import { getErrorMessage } from '@/api/client'
import { rbacApi, type PermissionOption } from '@/api/services'

interface Props {
  open: boolean
  onOpenChange: (v: boolean) => void
  roleId?: string
  roleName?: string
}

/** Gán quyền cho một vai trò — checkbox gom nhóm theo module. */
export default function PermissionPickerDialog({ open, onOpenChange, roleId, roleName }: Props) {
  const toast = useToast()
  const [all, setAll] = useState<PermissionOption[]>([])
  const [selected, setSelected] = useState<Set<string>>(new Set())
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!open || !roleId) return
    setLoading(true)
    Promise.all([rbacApi.permissions(), rbacApi.rolePermissions(roleId)])
      .then(([perms, ids]) => {
        setAll(perms)
        setSelected(new Set(ids))
      })
      .catch((e) => toast.error(getErrorMessage(e)))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, roleId])

  const groups = useMemo(() => {
    const m = new Map<string, PermissionOption[]>()
    all.forEach((p) => {
      if (!m.has(p.module)) m.set(p.module, [])
      m.get(p.module)!.push(p)
    })
    return [...m.entries()]
  }, [all])

  const toggle = (id: string) =>
    setSelected((s) => {
      const next = new Set(s)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })

  const toggleGroup = (items: PermissionOption[], on: boolean) =>
    setSelected((s) => {
      const next = new Set(s)
      items.forEach((p) => (on ? next.add(p.id) : next.delete(p.id)))
      return next
    })

  const save = async () => {
    if (!roleId) return
    setSaving(true)
    try {
      await rbacApi.setRolePermissions(roleId, [...selected])
      toast.success('Đã lưu phân quyền')
      onOpenChange(false)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[88vh] overflow-y-auto sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>Phân quyền {roleName ? `– ${roleName}` : ''}</DialogTitle>
        </DialogHeader>

        {loading ? (
          <div className="py-10 text-center text-muted-foreground">Đang tải...</div>
        ) : (
          <div className="space-y-3">
            {groups.map(([module, items]) => {
              const allOn = items.every((p) => selected.has(p.id))
              return (
                <div key={module} className="rounded-lg border p-3">
                  <div className="mb-2 flex items-center justify-between">
                    <span className="text-sm font-semibold">{module}</span>
                    <button
                      type="button"
                      className="text-xs font-medium text-primary hover:underline"
                      onClick={() => toggleGroup(items, !allOn)}
                    >
                      {allOn ? 'Bỏ chọn nhóm' : 'Chọn cả nhóm'}
                    </button>
                  </div>
                  <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                    {items.map((p) => (
                      <label key={p.id} className="flex cursor-pointer items-center gap-2 text-sm">
                        <input
                          type="checkbox"
                          className="h-4 w-4 accent-primary"
                          checked={selected.has(p.id)}
                          onChange={() => toggle(p.id)}
                        />
                        <span>{p.name}</span>
                        <code className="ml-auto text-[10px] text-muted-foreground">{p.code}</code>
                      </label>
                    ))}
                  </div>
                </div>
              )
            })}
          </div>
        )}

        <DialogFooter>
          <span className="mr-auto self-center text-sm text-muted-foreground">Đã chọn {selected.size} quyền</span>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button onClick={save} disabled={saving || loading}>{saving ? 'Đang lưu...' : 'Lưu'}</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
