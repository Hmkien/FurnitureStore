import { api } from './client'
import type {
  BaseQuery,
  PagedResult,
  Role,
  Permission,
  UserAccount,
  Category,
  Product,
  ProductDetail,
  ProductVariant,
  ProductImage,
  Order,
  OrderDetail,
  Post,
  Banner,
  Slide,
  Coupon,
  MembershipTier,
  MediaFile,
  StockMovement,
  LowStockVariant,
  RevenueSummary,
  RevenuePoint,
  TopProduct,
  OrderStatusCount,
  OrderStatus,
  PaymentStatus,
  StockMovementType,
} from '../types'

export const defaultQuery = (over: Partial<BaseQuery> = {}): BaseQuery => ({
  pageNumber: 1,
  pageSize: 10,
  sortDesc: true,
  ...over,
})

// ---- Factory CRUD cho các resource theo cùng convention backend ----
function crud<TEntity, TForm>(resource: string) {
  return {
    paged: (q: BaseQuery) =>
      api.post<PagedResult<TEntity>>(`/${resource}/GetPaged`, q).then((r) => r.data),
    get: (id: string) => api.get<TEntity>(`/${resource}/${id}`).then((r) => r.data),
    create: (form: TForm) => api.post(`/${resource}`, form).then((r) => r.data),
    update: (id: string, form: TForm) => api.put(`/${resource}/${id}`, form).then((r) => r.data),
    approve: (id: string) => api.put(`/${resource}/${id}/Approved`),
    reject: (id: string) => api.put(`/${resource}/${id}/Reject`),
    remove: (id: string) => api.delete(`/${resource}/${id}`),
  }
}

// ===== Hệ thống: Vai trò & Quyền (theo CrudService chuẩn) =====
export const rolesApi = crud<Role, Partial<Role>>('Role')
// Lưu ý: route backend là /Permision (giữ đúng chính tả của API).
export const permissionsApi = crud<Permission, Partial<Permission>>('Permision')

// ===== Hệ thống: Tài khoản người dùng =====
// User dùng multipart/form-data (có avatar) + approve/reject qua POST, nên tách riêng.
export interface UserQuery extends BaseQuery {
  status?: number
}

export const usersApi = {
  paged: (q: UserQuery) =>
    api.post<PagedResult<UserAccount>>('/User/GetPaged', q).then((r) => r.data),
  get: (id: string) => api.get<UserAccount>(`/User/${id}`).then((r) => r.data),
  create: (form: FormData) =>
    api
      .post('/User', form, { headers: { 'Content-Type': 'multipart/form-data' } })
      .then((r) => r.data),
  update: (id: string, form: FormData) =>
    api
      .put(`/User/${id}`, form, { headers: { 'Content-Type': 'multipart/form-data' } })
      .then((r) => r.data),
  remove: (id: string) => api.delete(`/User/${id}`),
  approve: (id: string) => api.post(`/User/${id}/approve`),
  reject: (id: string) => api.post(`/User/${id}/reject`),
  resetPassword: (id: string, newPassword: string, confirmPassword: string) =>
    api.post(`/User/${id}/reset-password`, { newPassword, confirmPassword }),
}

// ===== Catalog =====
export const categoriesApi = crud<Category, Partial<Category>>('Category')

export const productsApi = {
  ...crud<Product, Partial<Product>>('Product'),
  paged: (q: BaseQuery & Record<string, unknown>) =>
    api.post<PagedResult<Product>>('/Product/GetPaged', q).then((r) => r.data),
  detail: (id: string) => api.get<ProductDetail>(`/Product/${id}/detail`).then((r) => r.data),
}

export const variantsApi = {
  ...crud<ProductVariant, Partial<ProductVariant>>('ProductVariant'),
  byProduct: (productId: string) =>
    api.get<ProductVariant[]>(`/ProductVariant/by-product/${productId}`).then((r) => r.data),
}

export const imagesApi = {
  create: (form: Partial<ProductImage>) => api.post('/ProductImage', form).then((r) => r.data),
  update: (id: string, form: Partial<ProductImage>) => api.put(`/ProductImage/${id}`, form),
  remove: (id: string) => api.delete(`/ProductImage/${id}`),
  byProduct: (productId: string) =>
    api.get<ProductImage[]>(`/ProductImage/by-product/${productId}`).then((r) => r.data),
}

// ===== Orders =====
export const ordersApi = {
  all: (q: BaseQuery) => api.post<PagedResult<Order>>('/Order/GetPaged', q).then((r) => r.data),
  detail: (id: string) => api.get<OrderDetail>(`/Order/${id}`).then((r) => r.data),
  updateStatus: (id: string, orderStatus: OrderStatus) =>
    api.put(`/Order/${id}/status`, { orderStatus }),
  updatePaymentStatus: (id: string, paymentStatus: PaymentStatus) =>
    api.put(`/Order/${id}/payment-status`, { paymentStatus }),
  cancel: (id: string) => api.put(`/Order/${id}/cancel`),
}

// ===== Content =====
export const postsApi = crud<Post, Partial<Post>>('Post')
export const bannersApi = crud<Banner, Partial<Banner>>('Banner')
export const slidesApi = crud<Slide, Partial<Slide>>('Slide')

// ===== Commerce =====
export const couponsApi = crud<Coupon, Partial<Coupon>>('Coupon')
export const tiersApi = {
  ...crud<MembershipTier, Partial<MembershipTier>>('MembershipTier'),
}

// ===== Reviews (đánh giá sản phẩm, có OTP) =====
export const reviewsApi = {
  byProduct: (productId: string) => api.get(`/Review/by-product/${productId}`).then((r) => r.data),
  paged: (q: BaseQuery) => api.post<PagedResult>('/Review/GetPaged', q).then((r) => r.data),
  requestOtp: (productId: string) =>
    api.post<{ otp: string; message: string }>(`/Review/request-otp/${productId}`).then((r) => r.data),
  create: (form: { productId: string; rating: number; comment?: string; imageUrl?: string; otp: string }) =>
    api.post('/Review', form).then((r) => r.data),
  approve: (id: string) => api.put(`/Review/${id}/Approved`),
  reject: (id: string) => api.put(`/Review/${id}/Reject`),
}

// ===== Inventory =====
export const inventoryApi = {
  adjust: (form: { variantId: string; type: StockMovementType; quantity: number; note?: string }) =>
    api.post('/Inventory/adjust', form).then((r) => r.data),
  history: (variantId: string, q: BaseQuery) =>
    api.post<PagedResult<StockMovement>>(`/Inventory/history/${variantId}`, q).then((r) => r.data),
  lowStock: (threshold = 5) =>
    api.get<LowStockVariant[]>(`/Inventory/low-stock?threshold=${threshold}`).then((r) => r.data),
}

// ===== Media =====
export const mediaApi = {
  paged: (q: BaseQuery) => api.post<PagedResult<MediaFile>>('/Media/GetPaged', q).then((r) => r.data),
  upload: (file: File, folder?: string) => {
    const fd = new FormData()
    fd.append('file', file)
    if (folder) fd.append('folder', folder)
    return api
      .post<MediaFile>('/Media/upload', fd, { headers: { 'Content-Type': 'multipart/form-data' } })
      .then((r) => r.data)
  },
  remove: (id: string) => api.delete(`/Media/${id}`),
}

// ===== Returns & Warranty =====
export const returnsApi = {
  all: (q: BaseQuery) => api.post<PagedResult>('/Return/GetPaged', q).then((r) => r.data),
  my: (q: BaseQuery) => api.post<PagedResult>('/Return/my/GetPaged', q).then((r) => r.data),
  create: (form: unknown) => api.post('/Return', form).then((r) => r.data),
  updateStatus: (id: string, status: number, note?: string) => api.put(`/Return/${id}/status`, { status, note }),
}

export const warrantyApi = {
  all: (q: BaseQuery) => api.post<PagedResult>('/Warranty/GetPaged', q).then((r) => r.data),
  my: (q: BaseQuery) => api.post<PagedResult>('/Warranty/my/GetPaged', q).then((r) => r.data),
  create: (form: unknown) => api.post('/Warranty', form).then((r) => r.data),
  updateStatus: (id: string, status: number, note?: string) => api.put(`/Warranty/${id}/status`, { status, note }),
}

// ===== Notifications =====
export interface NotificationItem {
  type: string
  title: string
  description?: string
  createdAt: string
  link?: string
}
export const notificationsApi = {
  recent: (take = 20) => api.get<NotificationItem[]>(`/Notification?take=${take}`).then((r) => r.data),
}

// ===== Sync (giả lập dữ liệu mẫu) =====
export const syncApi = {
  enabled: () => api.get<{ enabled: boolean }>('/Sync/enabled').then((r) => r.data.enabled),
  generate: (resource: string, count = 10) =>
    api.post<{ created: number; message: string }>(`/Sync/${resource}?count=${count}`).then((r) => r.data),
  moho: (count = 5) =>
    api.post<{ created: number; message: string }>(`/Sync/moho?count=${count}`).then((r) => r.data),
  orders: (count = 10) =>
    api.post<{ created: number; message: string }>(`/Sync/orders?count=${count}`).then((r) => r.data),
}

// ===== RBAC: phân quyền cho vai trò & phân vai trò cho người dùng =====
export interface PermissionOption {
  id: string
  code: string
  name: string
  module: string
}
export interface RoleOption {
  id: string
  name: string
  roleCode: string
}

export const rbacApi = {
  sync: () =>
    api
      .post<{ permissionsCreated: number; rolesCreated: number; mappingsCreated: number; message: string }>(
        '/Rbac/sync',
      )
      .then((r) => r.data),
  permissions: () => api.get<PermissionOption[]>('/Rbac/permissions').then((r) => r.data),
  roles: () => api.get<RoleOption[]>('/Rbac/roles').then((r) => r.data),
  rolePermissions: (roleId: string) => api.get<string[]>(`/Rbac/roles/${roleId}/permissions`).then((r) => r.data),
  setRolePermissions: (roleId: string, permissionIds: string[]) =>
    api.put(`/Rbac/roles/${roleId}/permissions`, { permissionIds }),
  userRoles: (userId: string) => api.get<string[]>(`/Rbac/users/${userId}/roles`).then((r) => r.data),
  setUserRoles: (userId: string, roleIds: string[]) => api.put(`/Rbac/users/${userId}/roles`, { roleIds }),
}

// ===== Reports =====
export const reportsApi = {
  summary: (from?: string, to?: string) =>
    api
      .get<RevenueSummary>('/Report/revenue-summary', { params: { from, to } })
      .then((r) => r.data),
  byDay: (from: string, to: string) =>
    api
      .get<RevenuePoint[]>('/Report/revenue-by-day', { params: { from, to } })
      .then((r) => r.data),
  topProducts: (from?: string, to?: string, top = 10) =>
    api
      .get<TopProduct[]>('/Report/top-products', { params: { from, to, top } })
      .then((r) => r.data),
  statusBreakdown: () =>
    api.get<OrderStatusCount[]>('/Report/order-status-breakdown').then((r) => r.data),
  lowStock: (threshold = 5) =>
    api.get<LowStockVariant[]>(`/Report/low-stock?threshold=${threshold}`).then((r) => r.data),
}
