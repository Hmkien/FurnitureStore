import { Routes, Route, Navigate } from 'react-router-dom'
import ProtectedRoute from './components/ProtectedRoute'
import Layout from './components/Layout'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import CategoriesPage from './pages/CategoriesPage'
import ProductsPage from './pages/ProductsPage'
import OrdersPage from './pages/OrdersPage'
import OrderDetailPage from './pages/OrderDetailPage'
import ReturnsPage from './pages/ReturnsPage'
import WarrantyPage from './pages/WarrantyPage'
import InventoryPage from './pages/InventoryPage'
import MediaPage from './pages/MediaPage'
import CouponsPage from './pages/CouponsPage'
import MembershipTiersPage from './pages/MembershipTiersPage'
import PostsPage from './pages/PostsPage'
import BannersPage from './pages/BannersPage'
import SlidesPage from './pages/SlidesPage'
import UsersPage from './pages/UsersPage'
import RolesPage from './pages/RolesPage'
import PermissionsPage from './pages/PermissionsPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardPage />} />
        <Route path="categories" element={<CategoriesPage />} />
        <Route path="products" element={<ProductsPage />} />
        <Route path="orders" element={<OrdersPage />} />
        <Route path="orders/:id" element={<OrderDetailPage />} />
        <Route path="returns" element={<ReturnsPage />} />
        <Route path="warranty" element={<WarrantyPage />} />
        <Route path="inventory" element={<InventoryPage />} />
        <Route path="media" element={<MediaPage />} />
        <Route path="coupons" element={<CouponsPage />} />
        <Route path="membership-tiers" element={<MembershipTiersPage />} />
        <Route path="posts" element={<PostsPage />} />
        <Route path="banners" element={<BannersPage />} />
        <Route path="slides" element={<SlidesPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="roles" element={<RolesPage />} />
        <Route path="permissions" element={<PermissionsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
