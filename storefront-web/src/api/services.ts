import { api, tokenStore } from './client'
import type {
  PagedResult,
  Product,
  ProductDetail,
  Category,
  Slide,
  Banner,
  Post,
  Coupon,
  Address,
  Cart,
  ProductReviewSummary,
  OrderDetail,
  OrderListItem,
  CurrentUser,
  PaymentMethod,
} from '../types'

// ===== Auth =====
export const authApi = {
  login: (username: string, password: string) =>
    api.post('/Auth/login', { username, password }).then((r) => {
      tokenStore.set(r.data.accessToken, r.data.refreshToken)
      return r.data
    }),
  register: (userName: string, email: string, password: string) =>
    api.post('/Auth/register', { userName, email, password }).then((r) => r.data),
  me: () => api.get<CurrentUser>('/User/current').then((r) => r.data),
  logout: async () => {
    try {
      await api.delete('/Auth/logout')
    } catch {
      /* ignore */
    }
    tokenStore.clear()
  },
}

// ===== Catalog (public) =====
export interface ProductQuery {
  pageNumber: number
  pageSize: number
  keyword?: string
  sortBy?: string
  sortDesc?: boolean
  categoryId?: string
  minPrice?: number
  maxPrice?: number
  style?: string
}

export const catalogApi = {
  products: (q: ProductQuery) => api.post<PagedResult<Product>>('/Product/GetPaged', q).then((r) => r.data),
  productDetail: (id: string) => api.get<ProductDetail>(`/Product/${id}/detail`).then((r) => r.data),
  categories: () =>
    api.post<PagedResult<Category>>('/Category/GetPaged', { pageNumber: 1, pageSize: 100, sortDesc: false }).then((r) => r.data.data),
  slides: () => api.get<Slide[]>('/Slide/active').then((r) => r.data),
  banners: (position?: string) => api.get<Banner[]>('/Banner/active', { params: { position } }).then((r) => r.data),
  posts: (pageSize = 8) =>
    api.post<PagedResult<Post>>('/Post/GetPaged', { pageNumber: 1, pageSize, sortDesc: true }).then((r) => r.data.data),
  postBySlug: (slug: string) => api.get<Post>(`/Post/by-slug/${slug}`).then((r) => r.data),
  coupons: () => api.get<Coupon[]>('/Coupon/active').then((r) => r.data),
}

// ===== Reviews (OTP) =====
export const reviewApi = {
  byProduct: (productId: string) => api.get<ProductReviewSummary>(`/Review/by-product/${productId}`).then((r) => r.data),
  requestOtp: (productId: string) =>
    api.post<{ otp: string; message: string }>(`/Review/request-otp/${productId}`).then((r) => r.data),
  create: (form: { productId: string; rating: number; comment?: string; otp: string }) => api.post('/Review', form).then((r) => r.data),
}

// ===== Cart (auth) =====
export const cartApi = {
  get: () => api.get<Cart>('/Cart').then((r) => r.data),
  add: (variantId: string, quantity: number) => api.post<Cart>('/Cart/items', { variantId, quantity }).then((r) => r.data),
  update: (cartItemId: string, quantity: number) => api.put<Cart>(`/Cart/items/${cartItemId}`, { quantity }).then((r) => r.data),
  remove: (cartItemId: string) => api.delete<Cart>(`/Cart/items/${cartItemId}`).then((r) => r.data),
  clear: () => api.delete('/Cart'),
  merge: (guestToken: string) => api.post('/Cart/merge', { guestToken }),
}

// ===== Coupons =====
export const couponApi = {
  preview: (code: string, subTotal: number) =>
    api.get<{ code: string; subTotal: number; discountAmount: number; amountAfterDiscount: number }>('/Coupon/preview', { params: { code, subTotal } }).then((r) => r.data),
}

// ===== Orders =====
export const orderApi = {
  checkout: (form: {
    receiverName: string
    receiverPhone: string
    shippingAddress: string
    note?: string
    couponCode?: string
    shippingFee: number
    paymentMethod: PaymentMethod
  }) => api.post<OrderDetail>('/Order/checkout', form).then((r) => r.data),
  my: (q?: { keyword?: string; orderStatus?: number }) =>
    api
      .post<PagedResult<OrderListItem>>('/Order/my/GetPaged', {
        pageNumber: 1,
        pageSize: 50,
        sortDesc: true,
        keyword: q?.keyword || undefined,
        searchIn: q?.keyword ? ['OrderCode', 'ReceiverName', 'ReceiverPhone'] : undefined,
        orderStatus: q?.orderStatus,
      })
      .then((r) => r.data),
  detail: (id: string) => api.get<OrderDetail>(`/Order/${id}`).then((r) => r.data),
}

// ===== Sổ địa chỉ =====
export const addressApi = {
  list: () => api.get<Address[]>('/Address').then((r) => r.data),
  create: (form: Omit<Address, 'id'>) => api.post<Address>('/Address', form).then((r) => r.data),
  update: (id: string, form: Omit<Address, 'id'>) => api.put(`/Address/${id}`, form),
  remove: (id: string) => api.delete(`/Address/${id}`),
  setDefault: (id: string) => api.put(`/Address/${id}/default`),
}

// ===== Hồ sơ người dùng =====
export const userApi = {
  updateProfile: (
    userId: string,
    data: { email: string; firstName?: string; lastName?: string; phoneNumber?: string; address?: string; birthday?: string },
  ) => {
    const fd = new FormData()
    fd.append('Email', data.email)
    if (data.firstName) fd.append('FirstName', data.firstName)
    if (data.lastName) fd.append('LastName', data.lastName)
    if (data.phoneNumber) fd.append('PhoneNumber', data.phoneNumber)
    if (data.address) fd.append('Address', data.address)
    if (data.birthday) fd.append('Birthday', data.birthday)
    fd.append('RemoveImage', 'false')
    return api.put(`/User/${userId}`, fd, { headers: { 'Content-Type': 'multipart/form-data' } })
  },
  changePassword: (currentPassword: string, newPassword: string) =>
    api.post('/User/change-password', { currentPassword, newPassword }),
}

// ===== Payment =====
export const paymentApi = {
  vnpay: (orderCode: string) => api.get<{ paymentUrl: string }>(`/Payment/vnpay/create/${orderCode}`).then((r) => r.data),
  momo: (orderCode: string) => api.get<{ paymentUrl: string }>(`/Payment/momo/create/${orderCode}`).then((r) => r.data),
}

// ===== Returns & Warranty =====
export const supportApi = {
  createReturn: (form: unknown) => api.post('/Return', form).then((r) => r.data),
  myReturns: () => api.post<PagedResult>('/Return/my/GetPaged', { pageNumber: 1, pageSize: 50, sortDesc: true }).then((r) => r.data),
  createWarranty: (form: unknown) => api.post('/Warranty', form).then((r) => r.data),
}
