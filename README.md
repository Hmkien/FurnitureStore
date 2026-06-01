# FurnitureStore

Nền tảng thương mại điện tử bán nội thất gồm **API backend** (ASP.NET Core / .NET 10) và **hai ứng dụng web** (React + Vite): trang khách hàng (*storefront*) và trang quản trị (*admin*).

---

## Kiến trúc tổng quan

```
FurnitureStore/
├── FurnitureStore.API/      # Backend ASP.NET Core (.NET 10) — REST API
├── storefront-web/          # Trang khách hàng (React + Vite)  — cổng 3000
├── admin-web/               # Trang quản trị   (React + Vite)  — cổng 3100, vào qua /quan-tri
├── FurnitureStore.slnx      # Solution .NET (chỉ chứa API)
└── package.json             # Script chạy đồng thời 2 web app (concurrently)
```

Sơ đồ cổng khi chạy local:

| Thành phần      | Cổng | Ghi chú                                                        |
| --------------- | ---- | ------------------------------------------------------------- |
| API backend     | 3001 | Swagger UI ở `http://localhost:3001/swagger`                  |
| Storefront      | 3000 | Site mặc định cho khách hàng                                  |
| Admin           | 3100 | Truy cập qua `http://localhost:3000/quan-tri` (storefront proxy) |

Storefront proxy đường dẫn `/quan-tri` sang app admin (cổng 3100), nên người dùng chỉ cần mở **một** địa chỉ `http://localhost:3000`.

---

## Công nghệ sử dụng

**Backend (`FurnitureStore.API`)**
- ASP.NET Core / .NET 10 (Web API)
- Entity Framework Core 10 + SQL Server (LocalDB) — Code First + Migrations
- Xác thực JWT (access + refresh token), phân quyền RBAC (Role / Permission)
- Thanh toán: VNPay & MoMo (sandbox)
- BCrypt.Net cho băm mật khẩu
- Swagger / Swashbuckle để xem & thử API

**Frontend (`storefront-web` & `admin-web`)**
- React 18 + TypeScript + Vite 5
- React Router 6, Axios (có interceptor tự refresh token)
- Tailwind CSS
- Admin bổ sung: Radix UI, TanStack Table, TipTap (rich text), Recharts (biểu đồ)

---

## Tính năng chính

- **Danh mục & sản phẩm**: danh mục nhiều cấp, biến thể sản phẩm (variant), hình ảnh, tình trạng hàng.
- **Bán hàng**: giỏ hàng (cho cả khách chưa đăng nhập qua `X-Cart-Token`), đặt hàng, mã giảm giá (coupon), hạng thành viên (membership tier).
- **Thanh toán**: tích hợp VNPay và MoMo (môi trường sandbox).
- **Sau bán hàng**: yêu cầu đổi/trả (return), bảo hành (warranty), đánh giá sản phẩm (review).
- **Kho**: quản lý tồn kho, lịch sử nhập/xuất (stock movement).
- **Nội dung**: bài viết (post), banner, slide trang chủ.
- **Người dùng & phân quyền**: tài khoản, địa chỉ giao hàng, vai trò & quyền (RBAC), thông báo.
- **Quản trị**: dashboard, báo cáo, quản lý media, đồng bộ dữ liệu (sync), seed dữ liệu mẫu.

---

## Yêu cầu môi trường

- [.NET SDK 10](https://dotnet.microsoft.com/download) trở lên
- [Node.js 18+](https://nodejs.org/) và npm
- SQL Server / SQL Server LocalDB (mặc định dùng `(localdb)\mssqllocaldb`)

---

## Cài đặt & chạy

### 1. Backend (API)

```bash
cd FurnitureStore.API
dotnet restore
dotnet run
```

- API sẽ chạy ở `http://localhost:3001`, Swagger tại `http://localhost:3001/swagger`.
- **Migration tự áp dụng khi khởi động** (`db.Database.Migrate()` trong `Program.cs`), nên không cần chạy `dotnet ef database update` thủ công. Nếu muốn chạy tay:

  ```bash
  dotnet ef database update
  ```

- Chuỗi kết nối và các cấu hình khác nằm trong `FurnitureStore.API/appsettings.json` (xem mục [Cấu hình](#cấu-hình)).

### 2. Frontend (storefront + admin)

Từ thư mục gốc, cài và chạy đồng thời cả hai web app:

```bash
# Cài dependency cho cả 2 app
npm run install:all

# Chạy đồng thời storefront (3000) + admin (3100)
npm run dev
```

Sau đó mở:
- Trang khách hàng: `http://localhost:3000`
- Trang quản trị:   `http://localhost:3000/quan-tri`

> Chạy riêng từng app: `npm --prefix storefront-web run dev` hoặc `npm --prefix admin-web run dev`.

---

## Cấu hình

### Backend — `FurnitureStore.API/appsettings.json`

| Mục                 | Mô tả                                                          |
| ------------------- | ------------------------------------------------------------- |
| `ConnectionStrings` | Chuỗi kết nối SQL Server (mặc định LocalDB `FurnitureStoreDB`) |
| `Jwt`               | Khóa ký, Issuer, Audience, thời hạn token                    |
| `Vnpay` / `Momo`    | Thông tin tích hợp cổng thanh toán (sandbox)                 |
| `Shipping`          | Phí ship cố định và ngưỡng miễn phí vận chuyển               |
| `EnableSync`        | Bật/tắt chức năng đồng bộ dữ liệu                            |

> ⚠️ **Bảo mật:** Các khóa bí mật (JWT key, secret của VNPay/MoMo) đang để giá trị mẫu trong `appsettings.json`. Trước khi triển khai thực tế, hãy thay bằng giá trị thật và quản lý qua biến môi trường / user-secrets, **không commit lên Git**.

### Frontend — file `.env`

Mỗi web app có file `.env` riêng trỏ tới API:

```
VITE_API_URL=http://localhost:3001
```

---

## Xây dựng bản production (build)

```bash
# Backend
cd FurnitureStore.API
dotnet publish -c Release

# Frontend (chạy trong từng thư mục storefront-web / admin-web)
npm run build      # tạo thư mục dist/
npm run preview    # xem thử bản build
```

---

## Ghi chú

- Dự án chưa khởi tạo Git. Để bắt đầu quản lý phiên bản:

  ```bash
  git init
  git add .
  git commit -m "Initial commit"
  ```

  File `.gitignore` ở thư mục gốc đã loại trừ sẵn `bin/`, `obj/`, `node_modules/`, `dist/`, các file `.env` và secret.
- CORS phía API đã cho phép các origin `http://localhost:3000`, `3100`, `5173`.
