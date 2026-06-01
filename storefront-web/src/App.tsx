import { Routes, Route, Navigate } from 'react-router-dom'
import Layout from './components/Layout'
import HomePage from './pages/HomePage'
import ProductsPage from './pages/ProductsPage'
import ProductDetailPage from './pages/ProductDetailPage'
import CartPage from './pages/CartPage'
import CheckoutPage from './pages/CheckoutPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import OrdersPage from './pages/OrdersPage'
import NewsPage from './pages/NewsPage'
import PostDetailPage from './pages/PostDetailPage'
import AboutPage from './pages/AboutPage'
import AccountLayout from './components/AccountLayout'
import ProfilePage from './pages/account/ProfilePage'
import AddressesPage from './pages/account/AddressesPage'
import ChangePasswordPage from './pages/account/ChangePasswordPage'
import { useAuth } from './context/AuthContext'
import type { ReactNode } from 'react'

function RequireAuth({ children }: { children: ReactNode }) {
  const { user, loading } = useAuth()
  if (loading) return <div className="p-10 text-center text-zinc-500">Đang tải...</div>
  if (!user) return <Navigate to="/login" replace />
  return <>{children}</>
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="products" element={<ProductsPage />} />
        <Route path="products/:id" element={<ProductDetailPage />} />
        <Route path="tin-tuc" element={<NewsPage />} />
        <Route path="tin-tuc/:slug" element={<PostDetailPage />} />
        <Route path="ve-chung-toi" element={<AboutPage />} />
        <Route path="cart" element={<CartPage />} />
        <Route path="checkout" element={<RequireAuth><CheckoutPage /></RequireAuth>} />
        <Route path="orders" element={<Navigate to="/tai-khoan/don-hang" replace />} />
        <Route path="tai-khoan" element={<RequireAuth><AccountLayout /></RequireAuth>}>
          <Route index element={<ProfilePage />} />
          <Route path="dia-chi" element={<AddressesPage />} />
          <Route path="don-hang" element={<OrdersPage />} />
          <Route path="doi-mat-khau" element={<ChangePasswordPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  )
}
