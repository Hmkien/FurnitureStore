import { Navigate } from 'react-router-dom'

// Trang sửa sản phẩm cũ đã thay bằng modal trong ProductsPage.
export default function ProductEditPage() {
  return <Navigate to="/products" replace />
}
