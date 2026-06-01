import { useEffect, useRef, useState } from 'react'
import { Upload, Loader2, Check } from 'lucide-react'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { mediaApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from './Toast'
import { cn } from '@/lib/utils'
import { mediaUrl } from '@/lib/format'
import type { MediaFile } from '@/types'

interface Props {
  open: boolean
  onOpenChange: (v: boolean) => void
  onSelect: (url: string) => void
}

export default function MediaPicker({ open, onOpenChange, onSelect }: Props) {
  const toast = useToast()
  const ref = useRef<HTMLInputElement>(null)
  const [items, setItems] = useState<MediaFile[]>([])
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [uploading, setUploading] = useState(false)
  const [picked, setPicked] = useState<string | null>(null)

  const load = () => {
    setLoading(true)
    mediaApi
      .paged({ pageNumber: 1, pageSize: 60, keyword: keyword || undefined, searchIn: ['FileName', 'Folder'], sortBy: 'Created', sortDesc: true })
      .then((r) => setItems(r.data ?? []))
      .catch((e) => toast.error(getErrorMessage(e)))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    if (open) {
      setPicked(null)
      load()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, keyword])

  const uploadMany = async (files: FileList) => {
    setUploading(true)
    try {
      for (const f of Array.from(files)) await mediaApi.upload(f, 'general')
      toast.success(`Đã tải ${files.length} ảnh`)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setUploading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[88vh] overflow-hidden sm:max-w-3xl">
        <DialogHeader>
          <DialogTitle>Chọn ảnh từ kho</DialogTitle>
        </DialogHeader>
        <div className="flex items-center gap-2">
          <Input placeholder="Tìm ảnh..." value={keyword} onChange={(e) => setKeyword(e.target.value)} className="max-w-xs" />
          <Button variant="outline" onClick={() => ref.current?.click()} disabled={uploading}>
            {uploading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Upload className="mr-2 h-4 w-4" />}
            Tải lên
          </Button>
          <input ref={ref} type="file" accept="image/*" multiple className="hidden" onChange={(e) => e.target.files?.length && uploadMany(e.target.files)} />
        </div>

        <div className="grid max-h-[55vh] grid-cols-3 gap-3 overflow-y-auto p-1 sm:grid-cols-5">
          {loading ? (
            <div className="col-span-full flex h-40 items-center justify-center">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : items.length === 0 ? (
            <div className="col-span-full py-10 text-center text-sm text-muted-foreground">Kho ảnh trống</div>
          ) : (
            items.map((m) => (
              <button
                key={m.id}
                type="button"
                onClick={() => setPicked(m.url)}
                className={cn('relative aspect-square overflow-hidden rounded-lg border-2', picked === m.url ? 'border-primary' : 'border-transparent hover:border-muted')}
              >
                <img src={mediaUrl(m.url)} alt={m.fileName} className="h-full w-full object-cover" />
                {picked === m.url && (
                  <span className="absolute right-1 top-1 flex h-5 w-5 items-center justify-center rounded-full bg-primary text-primary-foreground">
                    <Check className="h-3 w-3" />
                  </span>
                )}
              </button>
            ))
          )}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button
            disabled={!picked}
            onClick={() => {
              if (picked) {
                onSelect(picked)
                onOpenChange(false)
              }
            }}
          >
            Chọn ảnh
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
