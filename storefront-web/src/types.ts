export enum StatusEntity { Approved = 1, Pending = 2, Rejected = 3, Draft = 4 }
export enum PaymentMethod { COD = 1, BankTransfer = 2, VNPAY = 3, Momo = 4 }
export enum OrderStatus { Pending = 1, Confirmed = 2, Preparing = 3, Shipping = 4, Completed = 5, Cancelled = 6 }
export enum PaymentStatus { Unpaid = 1, Paid = 2, Refunded = 3 }
export enum DiscountType { Percentage = 1, Fixed = 2 }

export interface Coupon {
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minOrderAmount: number
  maxDiscount: number
  endDate?: string
}

export interface Address {
  id: string
  receiverName: string
  receiverPhone: string
  addressLine: string
  label?: string
  isDefault: boolean
}

export interface PagedResult<T = unknown> {
  recordsTotal: number
  recordsFiltered: number
  data: T[]
}

export interface Category {
  id: string
  name: string
  slug: string
  imageUrl?: string
  status: StatusEntity
}

export interface Product {
  id: string
  name: string
  slug: string
  sku: string
  style?: string
  categoryId: string
  thumbnailUrl?: string
  minPrice?: number
  status: StatusEntity
}

export interface ProductVariant {
  id: string
  size?: string
  material?: string
  color?: string
  condition: number
  price: number
  stockQuantity: number
  skuVariant: string
}

export interface ProductImage {
  id: string
  imageUrl: string
  isPrimary: boolean
}

export interface ProductDetail {
  id: string
  name: string
  slug: string
  sku: string
  shortDescription?: string
  longDescription?: string
  style?: string
  categoryName?: string
  minPrice?: number
  maxPrice?: number
  variants: ProductVariant[]
  images: ProductImage[]
}

export interface Slide { id: string; title: string; imageUrl: string; linkUrl?: string; caption?: string }
export interface Banner { id: string; title: string; imageUrl: string; linkUrl?: string; position?: string }
export interface Post { id: string; title: string; slug: string; summary?: string; content?: string; thumbnailUrl?: string; category?: string; created: string }

export interface CartItem {
  id: string
  variantId: string
  productId: string
  productName: string
  size?: string
  material?: string
  color?: string
  imageUrl?: string
  price: number
  quantity: number
  lineTotal: number
  stockQuantity: number
}
export interface Cart { id: string; items: CartItem[]; totalItems: number; subTotal: number }

export interface ReviewVM { id: string; reviewerName: string; rating: number; comment?: string; imageUrl?: string; created: string }
export interface ProductReviewSummary { productId: string; averageRating: number; totalReviews: number; reviews: ReviewVM[] }

export interface OrderItem { id: string; productName: string; variantInfo?: string; price: number; quantity: number; lineTotal: number }
export interface OrderDetail {
  id: string
  orderCode: string
  subTotal: number
  shippingFee: number
  discountAmount: number
  couponCode?: string
  totalAmount: number
  paymentMethod: PaymentMethod
  paymentStatus: PaymentStatus
  orderStatus: OrderStatus
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  note?: string
  created: string
  items: OrderItem[]
}
export interface OrderListItem {
  id: string
  orderCode: string
  totalAmount: number
  orderStatus: OrderStatus
  paymentStatus: PaymentStatus
  created: string
  itemCount: number
  firstProductName?: string
  thumbnailUrl?: string
}

export interface CurrentUser {
  userId: string
  userName: string
  email: string
  fullName: string
  firstName?: string
  lastName?: string
  phoneNumber: string
  address?: string
  birthDay?: string
}
