import type { ReactNode } from 'react'
import { Card, CardContent } from '@/components/ui/card'

interface Props {
  title: string
  value: ReactNode
  icon: ReactNode
  color: string
  hint?: string
}

export default function StatCard({ title, value, icon, color, hint }: Props) {
  return (
    <Card>
      <CardContent className="flex items-center gap-4 p-5">
        <div
          className="flex h-12 w-12 flex-none items-center justify-center rounded-xl text-white"
          style={{ background: color, boxShadow: `0 6px 16px ${color}40` }}
        >
          {icon}
        </div>
        <div className="min-w-0">
          <p className="text-sm text-muted-foreground">{title}</p>
          <p className="text-2xl font-bold leading-tight">{value}</p>
          {hint && <p className="text-xs text-muted-foreground">{hint}</p>}
        </div>
      </CardContent>
    </Card>
  )
}
