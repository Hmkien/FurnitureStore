import { useEffect, useState } from 'react'
import { Sparkles } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { syncApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from './Toast'

let cachedEnabled: boolean | null = null
let inflight: Promise<boolean> | null = null

function loadEnabled(): Promise<boolean> {
  if (cachedEnabled !== null) return Promise.resolve(cachedEnabled)
  if (!inflight) {
    inflight = syncApi
      .enabled()
      .then((v) => ((cachedEnabled = v), v))
      .catch(() => ((cachedEnabled = false), false))
  }
  return inflight
}

export default function SyncButton({ resource, count = 10, onDone }: { resource: string; count?: number; onDone?: () => void }) {
  const toast = useToast()
  const [enabled, setEnabled] = useState(cachedEnabled ?? false)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    loadEnabled().then(setEnabled)
  }, [])

  if (!enabled) return null

  const run = async () => {
    setLoading(true)
    try {
      const res = await syncApi.generate(resource, count)
      toast.success(res.message)
      onDone?.()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setLoading(false)
    }
  }

  return (
    <Button variant="outline" onClick={run} disabled={loading}>
      <Sparkles className="mr-2 h-4 w-4" />
      {loading ? 'Đang tạo...' : 'Tạo dữ liệu mẫu'}
    </Button>
  )
}
