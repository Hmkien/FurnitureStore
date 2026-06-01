import { useEffect, useState } from 'react'
import { PackagePlus, ReceiptText, ShieldPlus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { syncApi, rbacApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from './Toast'
import { useAuth } from '@/context/AuthContext'

// Cache trạng thái EnableSync để không gọi lại nhiều lần.
let cachedEnabled: boolean | null = null
let inflight: Promise<boolean> | null = null
function loadEnabled(): Promise<boolean> {
  if (cachedEnabled !== null) return Promise.resolve(cachedEnabled)
  if (!inflight) inflight = syncApi.enabled().then((v) => (cachedEnabled = v)).catch(() => (cachedEnabled = false))
  return inflight
}

function useSyncEnabled() {
  const [enabled, setEnabled] = useState(cachedEnabled ?? false)
  useEffect(() => {
    loadEnabled().then(setEnabled)
  }, [])
  return enabled
}

/** Seed 5 sản phẩm Moho khác nhau mỗi lần — đặt ở trang Sản phẩm. */
export function MohoSeedButton({ onDone }: { onDone?: () => void }) {
  const toast = useToast()
  const enabled = useSyncEnabled()
  const [busy, setBusy] = useState(false)
  if (!enabled) return null

  const run = async () => {
    setBusy(true)
    try {
      const res = await syncApi.moho(5)
      toast.success(res.message)
      onDone?.()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Button variant="outline" onClick={run} disabled={busy}>
      <PackagePlus className="mr-2 h-4 w-4" />
      {busy ? 'Đang tạo...' : 'Lấy 5 SP Moho'}
    </Button>
  )
}

/** Tạo 10 đơn hàng giả lập nhiều trạng thái — đặt ở trang Đơn hàng. */
export function OrderSimButton({ onDone }: { onDone?: () => void }) {
  const toast = useToast()
  const enabled = useSyncEnabled()
  const [busy, setBusy] = useState(false)
  if (!enabled) return null

  const run = async () => {
    setBusy(true)
    try {
      const res = await syncApi.orders(10)
      toast.success(res.message)
      onDone?.()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Button variant="outline" onClick={run} disabled={busy}>
      <ReceiptText className="mr-2 h-4 w-4" />
      {busy ? 'Đang tạo...' : 'Tạo 10 đơn giả lập'}
    </Button>
  )
}

/** Đồng bộ quyền & vai trò từ enum — chỉ SuperUser, đặt ở trang Vai trò / Quyền. */
export function RbacSyncButton({ onDone }: { onDone?: () => void }) {
  const toast = useToast()
  const { user } = useAuth()
  const [busy, setBusy] = useState(false)
  if (!user?.isSuperUser) return null

  const run = async () => {
    setBusy(true)
    try {
      const res = await rbacApi.sync()
      toast.success(res.message)
      onDone?.()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Button variant="outline" onClick={run} disabled={busy}>
      <ShieldPlus className="mr-2 h-4 w-4" />
      {busy ? 'Đang đồng bộ...' : 'Đồng bộ quyền & vai trò'}
    </Button>
  )
}
