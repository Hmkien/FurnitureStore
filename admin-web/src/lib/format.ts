import {
  StatusEntity,
  OrderStatus,
  PaymentStatus,
  PaymentMethod,
  ProductCondition,
  DiscountType,
  StockMovementType,
  ReturnType,
  ReturnStatus,
  WarrantyStatus,
} from '../types'

import { API_URL } from '../api/client'

/** Trả URL ảnh dùng được: giữ nguyên nếu là URL tuyệt đối (http), ghép API_URL nếu là đường dẫn tương đối. */
export const mediaUrl = (u?: string) => (!u ? '' : /^https?:\/\//i.test(u) ? u : `${API_URL}${u}`)

export const currency = (v?: number) =>
  (v ?? 0).toLocaleString('vi-VN', { maximumFractionDigits: 0 }) + ' đ'

export const dateTime = (v?: string) => (v ? new Date(v).toLocaleString('vi-VN') : '')
export const dateOnly = (v?: string) => (v ? new Date(v).toLocaleDateString('vi-VN') : '')

export const statusLabel: Record<StatusEntity, string> = {
  [StatusEntity.Approved]: 'Hiển thị',
  [StatusEntity.Pending]: 'Chờ duyệt',
  [StatusEntity.Rejected]: 'Đã ẩn',
  [StatusEntity.Draft]: 'Bản nháp',
}

export const orderStatusLabel: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'Chờ xác nhận',
  [OrderStatus.Confirmed]: 'Đã xác nhận',
  [OrderStatus.Preparing]: 'Đang chuẩn bị',
  [OrderStatus.Shipping]: 'Đang giao',
  [OrderStatus.Completed]: 'Hoàn thành',
  [OrderStatus.Cancelled]: 'Đã hủy',
}

export const paymentStatusLabel: Record<PaymentStatus, string> = {
  [PaymentStatus.Unpaid]: 'Chưa thanh toán',
  [PaymentStatus.Paid]: 'Đã thanh toán',
  [PaymentStatus.Refunded]: 'Đã hoàn tiền',
}

export const paymentMethodLabel: Record<PaymentMethod, string> = {
  [PaymentMethod.COD]: 'COD',
  [PaymentMethod.BankTransfer]: 'Chuyển khoản',
  [PaymentMethod.VNPAY]: 'VNPAY',
  [PaymentMethod.Momo]: 'Momo',
}

export const conditionLabel: Record<ProductCondition, string> = {
  [ProductCondition.New]: 'Mới 100%',
  [ProductCondition.LikeNew99]: 'Như mới 99%',
  [ProductCondition.LikeNew]: 'Like new',
  [ProductCondition.Refurbished]: 'Tân trang',
  [ProductCondition.Returned]: 'Hàng trả lại',
}

export const discountTypeLabel: Record<DiscountType, string> = {
  [DiscountType.Percentage]: 'Theo %',
  [DiscountType.Fixed]: 'Số tiền',
}

export const movementTypeLabel: Record<StockMovementType, string> = {
  [StockMovementType.Import]: 'Nhập kho',
  [StockMovementType.Export]: 'Xuất kho',
  [StockMovementType.Adjust]: 'Điều chỉnh',
}

export const returnTypeLabel: Record<ReturnType, string> = {
  [ReturnType.Exchange]: 'Đổi hàng',
  [ReturnType.Return]: 'Trả hàng',
  [ReturnType.Refund]: 'Hoàn tiền',
}

export const returnStatusLabel: Record<ReturnStatus, string> = {
  [ReturnStatus.Requested]: 'Chờ xử lý',
  [ReturnStatus.Approved]: 'Đã duyệt',
  [ReturnStatus.Rejected]: 'Từ chối',
  [ReturnStatus.Completed]: 'Hoàn tất',
}

export const warrantyStatusLabel: Record<WarrantyStatus, string> = {
  [WarrantyStatus.Received]: 'Tiếp nhận',
  [WarrantyStatus.Processing]: 'Đang xử lý',
  [WarrantyStatus.Completed]: 'Hoàn tất',
  [WarrantyStatus.Rejected]: 'Từ chối',
}

export const orderStatusBadge: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'bg-amber-100 text-amber-700',
  [OrderStatus.Confirmed]: 'bg-blue-100 text-blue-700',
  [OrderStatus.Preparing]: 'bg-indigo-100 text-indigo-700',
  [OrderStatus.Shipping]: 'bg-cyan-100 text-cyan-700',
  [OrderStatus.Completed]: 'bg-emerald-100 text-emerald-700',
  [OrderStatus.Cancelled]: 'bg-red-100 text-red-700',
}

export const statusBadge: Record<StatusEntity, string> = {
  [StatusEntity.Approved]: 'bg-emerald-100 text-emerald-700',
  [StatusEntity.Pending]: 'bg-amber-100 text-amber-700',
  [StatusEntity.Rejected]: 'bg-red-100 text-red-700',
  [StatusEntity.Draft]: 'bg-slate-100 text-slate-600',
}

/** Helper liệt kê option cho <select> từ một bản ghi label. */
export function enumOptions<T extends number>(labels: Record<T, string>) {
  return (Object.entries(labels) as [string, string][]).map(([value, label]) => ({
    value: Number(value) as T,
    label,
  }))
}
