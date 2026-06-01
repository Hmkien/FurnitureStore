import { useEffect, useRef, useState } from 'react'
import { Upload, Trash2, Copy, Loader2, ChevronLeft, ChevronRight } from 'lucide-react'
import { mediaApi } from '@/api/services'
import { useToast } from '@/components/Toast'
import { getErrorMessage } from '@/api/client'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card } from '@/components/ui/card'
import { dateTime, mediaUrl } from '@/lib/format'
import type { MediaFile } from '@/types'

export default function MediaPage() {
  const toast = useToast()
  const ref = useRef<HTMLInputElement>(null)
  const [data, setData] = useState<MediaFile[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [keyword, setKeyword] = useState('')
  const [loading, setLoading] = useState(false)
  const [uploading, setUploading] = useState(false)
  const pageSize = 24
  const totalPages = Math.max(1, Math.ceil(total / pageSize))

  const load = async () => {
    setLoading(true)
    try {
      const res = await mediaApi.paged({ pageNumber: page, pageSize, keyword: keyword || undefined, searchIn: ['FileName', 'Folder'], sortBy: 'Created', sortDesc: true })
      setData(res.data ?? [])
      setTotal(res.recordsFiltered ?? 0)
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setLoading(false)
    }
  }
  useEffect(() => {
    load()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, keyword])

  const uploadMany = async (files: FileList) => {
    setUploading(true)
    try {
      for (const f of Array.from(files)) await mediaApi.upload(f, 'general')
      toast.success(`Đã tải lên ${files.length} ảnh`)
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    } finally {
      setUploading(false)
    }
  }

  const remove = async (m: MediaFile) => {
    if (!window.confirm('Xóa file này?')) return
    try {
      await mediaApi.remove(m.id)
      toast.success('Đã xóa')
      load()
    } catch (e) {
      toast.error(getErrorMessage(e))
    }
  }

  const copy = (url: string) => {
    navigator.clipboard?.writeText(mediaUrl(url))
    toast.info('Đã copy URL')
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-xl font-semibold">Kho media</h1>
        <div className="flex items-center gap-2">
          <Input
            placeholder="Tìm theo tên / thư mục..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                setPage(1)
                setKeyword(search)
              }
            }}
            className="w-56"
          />
          <Button onClick={() => ref.current?.click()} disabled={uploading}>
            {uploading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Upload className="mr-2 h-4 w-4" />}
            Tải ảnh lên
          </Button>
          <input ref={ref} type="file" accept="image/*" multiple className="hidden" onChange={(e) => e.target.files?.length && uploadMany(e.target.files)} />
        </div>
      </div>

      {loading ? (
        <div className="flex h-40 items-center justify-center">
          <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
        </div>
      ) : data.length === 0 ? (
        <Card className="p-10 text-center text-sm text-muted-foreground">Chưa có file nào</Card>
      ) : (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-6">
          {data.map((m) => (
            <Card key={m.id} className="group overflow-hidden">
              <div className="relative">
                <img src={mediaUrl(m.url)} alt={m.fileName} className="h-28 w-full object-cover" />
                <div className="absolute right-1 top-1 hidden gap-1 group-hover:flex">
                  <Button size="icon" variant="secondary" className="h-7 w-7" onClick={() => copy(m.url)}>
                    <Copy className="h-3.5 w-3.5" />
                  </Button>
                  <Button size="icon" variant="destructive" className="h-7 w-7" onClick={() => remove(m)}>
                    <Trash2 className="h-3.5 w-3.5" />
                  </Button>
                </div>
              </div>
              <div className="p-2">
                <p className="truncate text-xs font-medium" title={m.fileName}>{m.fileName}</p>
                <p className="text-[11px] text-muted-foreground">{dateTime(m.created)}</p>
              </div>
            </Card>
          ))}
        </div>
      )}

      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>{total} file · Trang {page}/{totalPages}</span>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(page - 1)}>
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(page + 1)}>
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}
