import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Bell, ShoppingCart, Star } from 'lucide-react'
import dayjs from 'dayjs'
import { Button } from '@/components/ui/button'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { notificationsApi, type NotificationItem } from '@/api/services'

const SEEN_KEY = 'fs_notif_seen'

export default function NotificationBell() {
  const navigate = useNavigate()
  const [items, setItems] = useState<NotificationItem[]>([])
  const [open, setOpen] = useState(false)
  const [seen, setSeen] = useState<number>(() => Number(localStorage.getItem(SEEN_KEY) ?? 0))

  const load = () => {
    notificationsApi.recent(20).then(setItems).catch(() => {})
  }

  useEffect(() => {
    load()
    const t = setInterval(load, 60000)
    return () => clearInterval(t)
  }, [])

  const unread = items.filter((i) => dayjs(i.createdAt).valueOf() > seen).length

  const onOpenChange = (v: boolean) => {
    setOpen(v)
    if (v) {
      const now = Date.now()
      localStorage.setItem(SEEN_KEY, String(now))
      setSeen(now)
    }
  }

  return (
    <Popover open={open} onOpenChange={onOpenChange}>
      <PopoverTrigger asChild>
        <Button variant="ghost" size="icon" className="relative">
          <Bell className="h-5 w-5 text-muted-foreground" />
          {unread > 0 && (
            <span className="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-destructive px-1 text-[10px] font-semibold text-destructive-foreground">
              {unread}
            </span>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent align="end" className="w-96 p-0">
        <div className="border-b px-4 py-3 font-semibold">Thông báo</div>
        <div className="max-h-96 overflow-y-auto">
          {items.length === 0 ? (
            <div className="py-10 text-center text-sm text-muted-foreground">Chưa có thông báo</div>
          ) : (
            items.map((n, i) => (
              <button
                key={i}
                className="flex w-full items-start gap-3 px-4 py-3 text-left hover:bg-accent"
                onClick={() => {
                  if (n.link) {
                    setOpen(false)
                    navigate(n.link)
                  }
                }}
              >
                <span
                  className={
                    'mt-0.5 flex h-9 w-9 flex-none items-center justify-center rounded-full ' +
                    (n.type === 'order' ? 'bg-blue-100 text-blue-600' : 'bg-amber-100 text-amber-600')
                  }
                >
                  {n.type === 'order' ? <ShoppingCart className="h-4 w-4" /> : <Star className="h-4 w-4" />}
                </span>
                <span className="min-w-0">
                  <span className="block text-sm font-medium">{n.title}</span>
                  <span className="block truncate text-xs text-muted-foreground">{n.description}</span>
                  <span className="block text-[11px] text-muted-foreground">{dayjs(n.createdAt).format('HH:mm DD/MM')}</span>
                </span>
              </button>
            ))
          )}
        </div>
      </PopoverContent>
    </Popover>
  )
}
