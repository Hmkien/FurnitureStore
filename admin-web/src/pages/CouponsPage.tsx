import ResourceCrud, { type CrudField } from '@/components/ResourceCrud'
import type { Column } from '@/components/DataTable'
import { couponsApi } from '@/api/services'
import { currency, discountTypeLabel, enumOptions } from '@/lib/format'
import { DiscountType, type Coupon } from '@/types'

const columns: Column<Coupon>[] = [
  { header: 'Mã', cell: (c) => <span className="font-medium">{c.code}</span> },
  { header: 'Loại', cell: (c) => discountTypeLabel[c.discountType] },
  { header: 'Giá trị', cell: (c) => (c.discountType === DiscountType.Percentage ? `${c.discountValue}%` : currency(c.discountValue)) },
  { header: 'Đơn tối thiểu', cell: (c) => currency(c.minOrderAmount) },
  { header: 'Lượt dùng', cell: (c) => `${c.usedCount}/${c.usageLimit || '∞'}` },
]

const fields: CrudField[] = [
  { name: 'code', label: 'Mã giảm giá', required: true },
  { name: 'discountType', label: 'Loại giảm', type: 'select', options: enumOptions(discountTypeLabel) },
  { name: 'discountValue', label: 'Giá trị giảm', type: 'number' },
  { name: 'maxDiscount', label: 'Giảm tối đa (0 = ∞)', type: 'number' },
  { name: 'minOrderAmount', label: 'Đơn tối thiểu', type: 'number' },
  { name: 'usageLimit', label: 'Giới hạn lượt (0 = ∞)', type: 'number' },
  { name: 'startDate', label: 'Bắt đầu', type: 'datetime' },
  { name: 'endDate', label: 'Kết thúc', type: 'datetime' },
  { name: 'description', label: 'Mô tả', type: 'textarea' },
]

export default function CouponsPage() {
  return (
    <ResourceCrud<Coupon>
      title="Mã giảm giá"
      api={couponsApi}
      columns={columns}
      fields={fields}
      searchIn={['Code', 'Description']}
      syncResource="coupon"
      defaults={{ discountType: DiscountType.Percentage, discountValue: 0, minOrderAmount: 0, maxDiscount: 0, usageLimit: 0 }}
    />
  )
}
