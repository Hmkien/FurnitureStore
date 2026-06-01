import { useEffect, useState } from 'react'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { useToast } from './Toast'
import { getErrorMessage } from '@/api/client'
import { rbacApi, type RoleOption } from '@/api/services'

interface Props {
  open: boolean
  onOpenChange: (v: boolean) => void
  userId?: string
  userName?: string
}

/** Gán vai trò cho một người dùng. */
export default function RolePickerDialog({ open, onOpenChange, userId, userName }: Props) {
  const toast = useToast()
  const [roles, setRoles] = useState<RoleOption[]>([])
  const [selected, setSelected] = useState<Set<string>>(new Set())
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (!open || !userId) return
    setLoading(true)
    Promise.all([rbacApi.roles(), rbacApi.userRoles(userId)])
      .then(([all, ids]) => {
        setRoles(all)
        setSelected(new Set(ids))
      })
      .catch((e) => toast.error(getErrorMessage(e)))
      .finally(() => setLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, userId])

  const toggle = (id: string) =>
    setSelected((s) => {
      const next = new Set(s)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })

  const save = async () => {
    if (!userId) return
    setSaving(true)
    try {
      await rbacApi.setUserRoles(userId, [...selected])
      toast.success('Đã lưu vai trò')
      onOpenChange(false)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[88vh] overflow-y-auto sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Phân vai trò {userName ? `– ${userName}` : ''}</DialogTitle>
        </DialogHeader>

        {loading ? (
          <div className="py-10 text-center text-muted-foreground">Đang tải...</div>
        ) : roles.length === 0 ? (
          <div className="py-10 text-center text-sm text-muted-foreground">
            Chưa có vai trò nào. Hãy "Đồng bộ phân quyền" ở trang Tổng quan trước.
          </div>
        ) : (
          <div className="space-y-2">
            {roles.map((r) => (
              <label key={r.id} className="flex cursor-pointer items-center gap-3 rounded-lg border p-2.5 text-sm">
                <input
                  type="checkbox"
                  className="h-4 w-4 accent-primary"
                  checked={selected.has(r.id)}
                  onChange={() => toggle(r.id)}
                />
                <span className="font-medium">{r.name}</span>
                <code className="ml-auto text-[10px] text-muted-foreground">{r.roleCode}</code>
              </label>
            ))}
          </div>
        )}

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button onClick={save} disabled={saving || loading}>{saving ? 'Đang lưu...' : 'Lưu'}</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
