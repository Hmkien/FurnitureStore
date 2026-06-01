import { useEffect, useState } from 'react'
import { DollarSign, ShoppingBag, Boxes, TrendingUp } from 'lucide-react'
import {
  ResponsiveContainer,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as RTooltip,
  PieChart,
  Pie,
  Cell,
  Legend,
} from 'recharts'
import dayjs from 'dayjs'
import { reportsApi } from '@/api/services'
import { getErrorMessage } from '@/api/client'
import { useToast } from '@/components/Toast'
import StatCard from '@/components/StatCard'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { currency, orderStatusLabel } from '@/lib/format'
import { OrderStatus } from '@/types'
import type { RevenueSummary, RevenuePoint, TopProduct, OrderStatusCount, LowStockVariant } from '@/types'

const shortCurrency = (v: number) => (v >= 1_000_000 ? `${(v / 1_000_000).toFixed(1)}tr` : v >= 1000 ? `${Math.round(v / 1000)}k` : `${v}`)

const statusColor: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: '#f59e0b',
  [OrderStatus.Confirmed]: '#3b82f6',
  [OrderStatus.Preparing]: '#6366f1',
  [OrderStatus.Shipping]: '#06b6d4',
  [OrderStatus.Completed]: '#16a34a',
  [OrderStatus.Cancelled]: '#ef4444',
}

export default function DashboardPage() {
  const toast = useToast()
  const [summary, setSummary] = useState<RevenueSummary | null>(null)
  const [series, setSeries] = useState<RevenuePoint[]>([])
  const [top, setTop] = useState<TopProduct[]>([])
  const [statuses, setStatuses] = useState<OrderStatusCount[]>([])
  const [low, setLow] = useState<LowStockVariant[]>([])

  useEffect(() => {
    const to = dayjs().endOf('day').toISOString()
    const from = dayjs().subtract(29, 'day').startOf('day').toISOString()
    Promise.all([
      reportsApi.summary(),
      reportsApi.byDay(from, to),
      reportsApi.topProducts(undefined, undefined, 5),
      reportsApi.statusBreakdown(),
      reportsApi.lowStock(5),
    ])
      .then(([s, d, t, st, l]) => {
        setSummary(s)
        setSeries(d)
        setTop(t)
        setStatuses(st)
        setLow(l)
      })
      .catch((e) => toast.error(getErrorMessage(e, 'Không tải được báo cáo')))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const chartData = series.map((p) => ({ label: dayjs(p.date).format('DD/MM'), revenue: p.revenue }))
  const pieData = statuses.map((s) => ({ name: orderStatusLabel[s.orderStatus as OrderStatus], value: s.count, color: statusColor[s.orderStatus as OrderStatus] }))

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Tổng quan</h1>
        <p className="text-sm text-muted-foreground">Số liệu kinh doanh 30 ngày gần nhất</p>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard title="Doanh thu" value={currency(summary?.totalRevenue)} icon={<DollarSign className="h-5 w-5" />} color="#16a34a" hint="Đơn đã hoàn thành" />
        <StatCard title="Đơn hoàn thành" value={summary?.completedOrders ?? 0} icon={<ShoppingBag className="h-5 w-5" />} color="#2563eb" />
        <StatCard title="Sản phẩm đã bán" value={summary?.itemsSold ?? 0} icon={<Boxes className="h-5 w-5" />} color="#7c3aed" />
        <StatCard title="Giá trị đơn TB" value={currency(summary?.averageOrderValue)} icon={<TrendingUp className="h-5 w-5" />} color="#ea580c" />
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Biểu đồ doanh thu</CardTitle>
          </CardHeader>
          <CardContent>
            {chartData.length === 0 ? (
              <div className="flex h-[300px] items-center justify-center text-sm text-muted-foreground">Chưa có dữ liệu</div>
            ) : (
              <ResponsiveContainer width="100%" height={300}>
                <AreaChart data={chartData} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
                  <defs>
                    <linearGradient id="rev" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#6d4bf0" stopOpacity={0.35} />
                      <stop offset="95%" stopColor="#6d4bf0" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#eef0f5" />
                  <XAxis dataKey="label" tick={{ fontSize: 12, fill: '#94a3b8' }} interval="preserveStartEnd" />
                  <YAxis tick={{ fontSize: 12, fill: '#94a3b8' }} tickFormatter={shortCurrency} width={50} />
                  <RTooltip formatter={(v: number) => currency(v)} />
                  <Area type="monotone" dataKey="revenue" stroke="#6d4bf0" strokeWidth={2.5} fill="url(#rev)" name="Doanh thu" />
                </AreaChart>
              </ResponsiveContainer>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Đơn theo trạng thái</CardTitle>
          </CardHeader>
          <CardContent>
            {pieData.length === 0 ? (
              <div className="flex h-[300px] items-center justify-center text-sm text-muted-foreground">Chưa có đơn</div>
            ) : (
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie data={pieData} dataKey="value" nameKey="name" innerRadius={55} outerRadius={95} paddingAngle={2}>
                    {pieData.map((d) => (
                      <Cell key={d.name} fill={d.color} />
                    ))}
                  </Pie>
                  <Legend />
                  <RTooltip />
                </PieChart>
              </ResponsiveContainer>
            )}
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Top sản phẩm bán chạy</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1">
            {top.length === 0 && <p className="py-6 text-center text-sm text-muted-foreground">Chưa có dữ liệu</p>}
            {top.map((p, i) => (
              <div key={p.productId} className="flex items-center gap-3 py-2">
                <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary/10 font-bold text-primary">{i + 1}</div>
                <div className="flex-1">
                  <div className="text-sm font-medium">{p.productName}</div>
                  <div className="text-xs text-muted-foreground">Đã bán {p.quantitySold}</div>
                </div>
                <div className="font-semibold">{currency(p.revenue)}</div>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Hàng sắp hết (≤ 5)</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1">
            {low.length === 0 && <p className="py-6 text-center text-sm text-muted-foreground">Không có</p>}
            {low.map((v) => (
              <div key={v.variantId} className="flex items-center justify-between py-2 text-sm">
                <div>
                  <div className="font-medium">{v.productName}</div>
                  <div className="text-xs text-muted-foreground">{v.skuVariant}</div>
                </div>
                <span className="rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-700">{v.stockQuantity}</span>
              </div>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
