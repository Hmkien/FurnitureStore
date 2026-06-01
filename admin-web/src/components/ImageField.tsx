import { useState } from 'react'
import { X, Images } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { mediaUrl } from '@/lib/format'
import MediaPicker from './MediaPicker'

interface Props {
  value?: string
  onChange?: (url: string | undefined) => void
  folder?: string
}

export default function ImageField({ value, onChange }: Props) {
  const [pickerOpen, setPickerOpen] = useState(false)

  return (
    <div className="flex items-center gap-3">
      {value && <img src={mediaUrl(value)} alt="" className="h-16 w-16 rounded-lg border object-cover" />}
      <Button type="button" variant="outline" size="sm" onClick={() => setPickerOpen(true)}>
        <Images className="mr-2 h-4 w-4" /> {value ? 'Đổi ảnh' : 'Chọn ảnh'}
      </Button>
      {value && (
        <Button type="button" variant="ghost" size="icon" onClick={() => onChange?.(undefined)}>
          <X className="h-4 w-4" />
        </Button>
      )}
      <MediaPicker open={pickerOpen} onOpenChange={setPickerOpen} onSelect={(url) => onChange?.(url)} />
    </div>
  )
}
