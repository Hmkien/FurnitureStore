// ===== Enums (khớp giá trị số với backend) =====
export enum StatusEntity {
  Approved = 1,
  Pending = 2,
  Rejected = 3,
  Draft = 4,
}

export enum ProductCondition {
  New = 1,
  LikeNew99 = 2,
  LikeNew = 3,
  Refurbished = 4,
  Returned = 5,
}

export enum PaymentMethod {
  COD = 1,
  BankTransfer = 2,
  VNPAY = 3,
  Momo = 4,
}

export enum PaymentStatus {
  Unpaid = 1,
  Paid = 2,
  Refunded = 3,
}

export enum OrderStatus {
  Pending = 1,
  Confirmed = 2,
  Preparing = 3,
  Shipping = 4,
  Completed = 5,
  Cancelled = 6,
}

export enum DiscountType {
  Percentage = 1,
  Fixed = 2,
}

export enum StockMovementType {
  Import = 1,
  Export = 2,
  Adjust = 3,
}

export enum ReturnType {
  Exchange = 1,
  Return = 2,
  Refund = 3,
}

export enum ReturnStatus {
  Requested = 1,
  Approved = 2,
  Rejected = 3,
  Completed = 4,
}

export enum WarrantyStatus {
  Received = 1,
  Processing = 2,
  Completed = 3,
  Rejected = 4,
}

// ===== Query & paging =====
export interface BaseQuery {
  pageNumber: number
  pageSize: number
  keyword?: string
  sortBy?: string
  sortDesc?: boolean
  fromDate?: string
  toDate?: string
  searchIn?: string[]
}

/** DataTableJson trả về từ backend. */
export interface PagedResult<T = unknown> {
  recordsTotal: number
  recordsFiltered: number
  data: T[]
}

// ===== Auth =====
export interface AuthResponse {
  username: string
  accessToken: string
  refreshToken: string
  accessTokenExpires: string
  refreshTokenExpires: string
}

export interface CurrentUser {
  userId: string
  userName: string
  email: string
  fullName: string
  phoneNumber: string
  address?: string
  isSuperUser: boolean
  birthDay?: string
  roles: string[]
  permissions: string[]
}

// ===== Hệ thống: tài khoản / vai trò / quyền =====
export interface Role {
  id: string
  name: string
  roleCode: string
  status: StatusEntity
  created: string
}

export interface Permission {
  id: string
  permisionName: string
  permisionCode: string
  description?: string
  status: StatusEntity
  created: string
}

export interface UserAccount {
  id: string
  userName: string
  email: string
  firstName?: string
  lastName?: string
  birthday?: string
  phoneNumber?: string
  address?: string
  isSuperUser: boolean
  imageAvatar?: string
  status: StatusEntity
  created: string
  lastModified: string
}

// ===== Catalog =====
export interface Category {
  id: string
  name: string
  slug: string
  description?: string
  imageUrl?: string
  parentId?: string | null
  status: StatusEntity
  created: string
}

export interface Product {
  id: string
  categoryId: string
  name: string
  slug: string
  sku: string
  shortDescription?: string
  longDescription?: string
  style?: string
  status: StatusEntity
  created: string
  thumbnailUrl?: string
  minPrice?: number
}

export interface ProductVariant {
  id: string
  productId: string
  size?: string
  material?: string
  color?: string
  condition: ProductCondition
  price: number
  stockQuantity: number
  skuVariant: string
}

export interface ProductImage {
  id: string
  productId: string
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
  categoryId: string
  categoryName?: string
  minPrice?: number
  maxPrice?: number
  variants: ProductVariant[]
  images: ProductImage[]
}

// ===== Orders =====
export interface OrderItem {
  id: string
  variantId: string
  productName: string
  variantInfo?: string
  price: number
  quantity: number
  lineTotal: number
}

export interface Order {
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
  itemCount: number
  created: string
}

export interface OrderDetail extends Order {
  userId: string
  receiverName: string
  receiverPhone: string
  shippingAddress: string
  note?: string
  items: OrderItem[]
}

// ===== Content =====
export interface Post {
  id: string
  title: string
  slug: string
  summary?: string
  content?: string
  thumbnailUrl?: string
  category?: string
  author?: string
  isPublished: boolean
  publishedAt?: string
  status: StatusEntity
  created: string
}

export interface Banner {
  id: string
  title: string
  imageUrl: string
  linkUrl?: string
  position?: string
  sortOrder: number
  startDate?: string
  endDate?: string
  status: StatusEntity
}

export interface Slide {
  id: string
  title: string
  imageUrl: string
  linkUrl?: string
  caption?: string
  sortOrder: number
  status: StatusEntity
}

// ===== Commerce =====
export interface Coupon {
  id: string
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minOrderAmount: number
  maxDiscount: number
  startDate?: string
  endDate?: string
  usageLimit: number
  usedCount: number
  status: StatusEntity
}

export interface MembershipTier {
  id: string
  name: string
  minSpending: number
  discountPercent: number
  description?: string
  status: StatusEntity
}

export interface MediaFile {
  id: string
  fileName: string
  url: string
  contentType?: string
  size: number
  folder?: string
  created: string
}

export interface StockMovement {
  id: string
  variantId: string
  skuVariant: string
  productName: string
  type: StockMovementType
  quantity: number
  quantityAfter: number
  note?: string
  created: string
}

export interface LowStockVariant {
  variantId: string
  productId: string
  productName: string
  skuVariant: string
  stockQuantity: number
}

// ===== Reports =====
export interface RevenueSummary {
  totalRevenue: number
  completedOrders: number
  itemsSold: number
  averageOrderValue: number
}

export interface RevenuePoint {
  date: string
  revenue: number
  orders: number
}

export interface TopProduct {
  productId: string
  productName: string
  quantitySold: number
  revenue: number
}

export interface OrderStatusCount {
  orderStatus: OrderStatus
  count: number
}

// ===== Returns & Warranty =====
export interface ReturnItem {
  variantId: string
  productName: string
  quantity: number
}

export interface ReturnRequest {
  id: string
  code: string
  orderCode: string
  customerName: string
  type: ReturnType
  requestStatus: ReturnStatus
  reason: string
  note?: string
  created: string
  items: ReturnItem[]
}

export interface WarrantyClaim {
  id: string
  code: string
  orderCode: string
  productName: string
  customerName: string
  customerPhone: string
  issueDescription: string
  claimStatus: WarrantyStatus
  note?: string
  created: string
  completedAt?: string
}
