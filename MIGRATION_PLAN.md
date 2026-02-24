# Kế Hoạch Chuyển Đổi Hệ Thống: Razor sang React Modern (MVP Priority)

Bản kế hoạch này tập trung vào việc hoàn thiện các giao diện tối thiểu (MVP) để hệ thống có thể vận hành luồng mua hàng cơ bản trên dự án **React (Vite + TypeScript)**, kết nối API Backend .NET.

---

## 0. Kết quả kiểm tra giao diện (ASP.NET vs React)
- ASP.NET (Razor) đã có đầy đủ màn hình MVP: `Home/Index`, `Products/Index`, `Products/Details`, `Cart/Index`, `Cart/Checkout`, `Cart/Success`, `Auth/Login`. Có thêm bước `Cart/Confirmation` trong flow checkout.
- React hiện có: `pages/Home.tsx`, `pages/Products.tsx` (UI đã dựng nhưng đang dùng mock data).
- React còn thiếu: `pages/ProductDetails.tsx`, `pages/Cart.tsx`, `pages/Login.tsx`, `pages/Checkout.tsx`, `pages/OrderSuccess.tsx` + route tương ứng.
- Chưa có kết nối API thực cho Home/Products, Navbar (user/cart), và luồng checkout.
- Backend đã có các API cho Products/Categories/Auth/Orders; còn thiếu API Cart và danh sách cửa hàng để frontend dùng cho checkout.

---

## 1. Ưu Tiên MVP (Minimum Viable Product)
Mục tiêu là hoàn thành luồng: **Xem hàng -> Chi tiết -> Thêm giỏ -> Đăng nhập -> Thanh toán**.

### Giai đoạn 1: Foundation & Core UIs (UI đã có, dữ liệu đang mock)
- [x] Thiết lập Project React, Tailwind CSS (v3.4.17), Framer Motion.
- [x] Design System: Layout (Navbar, Footer), Color Palette (Refined Espresso).
- [x] Trang Chủ (`Home/Index` -> `pages/Home.tsx`).
- [x] Danh sách sản phẩm (`Products/Index` -> `pages/Products.tsx`).
- [x] Quản lý trạng thái giỏ hàng (`CartContext.tsx`).
- [ ] Kết nối dữ liệu thật cho Home (categories + featured products) qua API.
- [ ] Kết nối dữ liệu thật cho Products (search/filter/pagination) qua API.
- [ ] Thay mock user/cart ở Navbar bằng Auth state + CartContext.

### Giai đoạn 2: Chi tiết & Giỏ hàng (ƯU TIÊN 1 - MVP)
*Mục tiêu: Cho phép người dùng xem kỹ sản phẩm và chuẩn bị mua hàng.*
- [ ] **Chi tiết sản phẩm (`Products/Details` -> `pages/ProductDetails.tsx`):**
    - Gallery hình ảnh, lựa chọn size/topping.
    - Gọi API `GET /api/v1/products/{id}` để lấy dữ liệu thật và render.
    - Nút "Thêm vào giỏ hàng" kết nối `CartContext`.
    - Sản phẩm liên quan (Sử dụng `GET /api/v1/products?categoryId=...` hoặc `search`).
- [ ] **Trang Giỏ hàng (`Cart/Index` -> `pages/Cart.tsx`):**
    - Xem danh sách sản phẩm đã chọn.
    - Cập nhật số lượng, xóa sản phẩm.
    - Tính tổng tiền Real-time.
    - Đồng bộ dữ liệu sản phẩm/giá từ API để tránh lệch giá.
    - Nếu cần đồng bộ giỏ hàng server-side: bổ sung API Cart (Add/Update/Remove/Get/Count).

### Giai đoạn 3: Hội viên & Thanh toán (ƯU TIÊN 2 - MVP)
*Mục tiêu: Hoàn tất quy trình đặt hàng bảo mật.*
- [ ] **Hệ thống đăng nhập (`Auth/Login` -> `pages/Login.tsx`):**
    - Giao diện hiện đại, validate form an toàn.
    - Gọi API `POST /api/v1/auth/login`, xử lý JWT Token từ Backend .NET.
    - Lưu/refresh trạng thái đăng nhập qua `GET /api/v1/auth/me`.
- [ ] **Quy trình Thanh toán (`Cart/Checkout` -> `pages/Checkout.tsx`):**
    - Nhập thông tin giao hàng (Sử dụng API lấy địa chỉ nếu có).
    - Lựa chọn phương thức thanh toán.
    - Tóm tắt đơn hàng (Order Summary).
    - Gọi `GET /api/v1/auth/me` để prefill thông tin người dùng.
    - Gọi API lấy danh sách cửa hàng (hiện chưa có endpoint, cần bổ sung).
    - Tạo đơn qua `POST /api/v1/orders`, nhận response và hiển thị kết quả.
- [ ] **Thành công (`Cart/Success` -> `pages/OrderSuccess.tsx`):**
    - Hiển thị mã đơn hàng và thông báo cảm ơn.
    - Gọi `GET /api/v1/orders/{id}` để hiển thị thông tin đơn thật.

---

## 2. Giai đoạn Hậu MVP (Mở rộng)

### Giai đoạn 4: Tin tức & Thông tin bổ trợ
- [ ] Trang Tin tức/Blog (`News`).
- [ ] Trang Giới thiệu (`About`).
- [ ] Trang Tìm kiếm nâng cao (`Search`).

### Giai đoạn 5: Quản lý khách hàng (User Portal)
- [ ] Thông tin cá nhân (`Profile Edit`).
- [ ] Lịch sử đơn hàng & Trạng thái vận chuyển.

### Giai đoạn 6: Quản trị viên (Admin Dashboard)
- [ ] Toàn bộ Area/Admin (Sản phẩm, Đơn hàng, Nhân viên, Kho...) sẽ được xây dựng trên một layout Dashboard riêng biệt.

---

## 3. Bản Đồ Chuyển Đổi MVP (Razor vs React)

| Chức năng MVP | Razor View (Gốc) | Component React | Trạng thái |
| :--- | :--- | :--- | :--- |
| Xem thực đơn | `Products/Index` | `pages/Products.tsx` | UI xong, chưa nối API |
| Xem chi tiết | `Products/Details` | `pages/ProductDetails.tsx` | Chưa có |
| Quản lý giỏ | `Cart/Index` | `pages/Cart.tsx` | Chưa có |
| Đăng nhập | `Auth/Login` | `pages/Login.tsx` | Chưa có |
| Đặt hàng | `Cart/Checkout` | `pages/Checkout.tsx` | Chưa có |
| Hoàn tất đơn | `Cart/Success` | `pages/OrderSuccess.tsx` | Chưa có |

---

## 4. Kết nối API & Dữ liệu (BẮT BUỘC cho MVP)
### 4.1. Endpoint đã có (Backend .NET)
- `GET /api/v1/products` (list, search, filter, pagination).
- `GET /api/v1/products/{id}` (product detail).
- `GET /api/v1/categories` (category list cho filter/home).
- `POST /api/v1/auth/login`, `GET /api/v1/auth/me`, `POST /api/v1/auth/logout`.
- `POST /api/v1/orders`, `GET /api/v1/orders/{id}`.

### 4.2. Endpoint còn thiếu (cần bổ sung để MVP chạy trọn vẹn)
- API Cart: `GET/POST/PATCH/DELETE /api/v1/cart` + `GET /api/v1/cart/count` (add/update/remove/get/summary).
- API danh sách cửa hàng: `GET /api/v1/stores` (từ `GetCafeStoresQuery` trong Application).

### 4.3. Quy tắc hiển thị dữ liệu (Frontend)
- Tất cả màn hình MVP gọi API qua `services/api.ts` (axios hoặc fetch).
- Có `loading / empty / error` state rõ ràng, hiển thị thông báo từ response.
- Mapping response vào UI (ProductCard, Cart rows, Order Summary, Order Success).

---

## 5. Cam Kết Bảo Mật & Kỹ Thuật
- **Bảo mật:** Validate 2 lớp, sanitize với `DOMPurify` khi hiển thị nội dung có nguy cơ XSS.
- **Auth:** Ưu tiên HttpOnly cookie nếu backend hỗ trợ; nếu dùng JWT Bearer thì giới hạn lưu trữ (không lưu thông tin nhạy cảm) và clear khi logout/401.
- **CSRF:** Nếu dùng cookie auth, bật `withCredentials` và cơ chế XSRF token.
- **Hiệu năng:** Code-splitting theo Route để giảm thời gian tải trang đầu tiên.
- **Visual:** Giữ vững tiêu chuẩn `@frontend-design` cho mọi màn hình MVP để "WOW" khách hàng ngay từ bước thanh toán.


## 6. Full UI Inventory (Razor -> React)
### 6.1. Public site (Views)
| Razor View (ASP.NET) | React target | Status |
| :--- | :--- | :--- |
| `Views/Home/Index.cshtml` | `CoffeeHouse.React/src/pages/Home.tsx` | Exists (mock data) |
| `Views/Home/_HeroSection.cshtml` | `CoffeeHouse.React/src/components/home/Hero.tsx` | Exists |
| `Views/Home/_CategoryStrip.cshtml` | `CoffeeHouse.React/src/components/home/CategoryStrip.tsx` | Exists (mock data) |
| `Views/Home/_PromoStrip.cshtml` | Missing | Not built |
| `Views/Home/_MagazineSection.cshtml` | Missing | Not built |
| `Views/Home/_ProductSuggestions.cshtml` | `CoffeeHouse.React/src/components/home/FeaturedProducts.tsx` | Exists (mock data) |
| `Views/Products/Index.cshtml` | `CoffeeHouse.React/src/pages/Products.tsx` | Exists (mock data) |
| `Views/Products/Details.cshtml` | `CoffeeHouse.React/src/pages/ProductDetails.tsx` | Missing |
| `Views/Products/Modern.cshtml` | `CoffeeHouse.React/src/pages/ProductsModern.tsx` | Missing |
| `Views/Products/Type.cshtml` | `CoffeeHouse.React/src/pages/ProductsByCategory.tsx` | Missing |
| `Views/Cart/Index.cshtml` | `CoffeeHouse.React/src/pages/Cart.tsx` | Missing |
| `Views/Cart/Checkout.cshtml` | `CoffeeHouse.React/src/pages/Checkout.tsx` | Missing |
| `Views/Cart/Confirmation.cshtml` | `CoffeeHouse.React/src/pages/OrderConfirmation.tsx` | Missing |
| `Views/Cart/Success.cshtml` | `CoffeeHouse.React/src/pages/OrderSuccess.tsx` | Missing |
| `Views/Auth/Login.cshtml` | `CoffeeHouse.React/src/pages/Login.tsx` | Missing |
| `Views/Auth/Register.cshtml` | `CoffeeHouse.React/src/pages/Register.tsx` | Missing |
| `Views/Account/ThongTin.cshtml` | `CoffeeHouse.React/src/pages/Profile.tsx` | Missing |
| `Views/Account/EditThongTin.cshtml` | `CoffeeHouse.React/src/pages/ProfileEdit.tsx` | Missing |
| `Views/HoaDon/Index.cshtml` | `CoffeeHouse.React/src/pages/Orders.tsx` | Missing |
| `Views/HoaDon/Details.cshtml` | `CoffeeHouse.React/src/pages/OrderDetails.tsx` | Missing |
| `Views/News/Index.cshtml` | `CoffeeHouse.React/src/pages/News.tsx` | Placeholder route only |
| `Views/News/Details.cshtml` | `CoffeeHouse.React/src/pages/NewsDetails.tsx` | Missing |
| `Views/About/Index.cshtml` | `CoffeeHouse.React/src/pages/About.tsx` | Placeholder route only |
| `Views/Search/Index.cshtml` | `CoffeeHouse.React/src/pages/Search.tsx` | Missing |

### 6.2. Layout / Shared / Components / Errors
| Razor View (ASP.NET) | React target | Status |
| :--- | :--- | :--- |
| `Views/Shared/_Layout.cshtml` | `CoffeeHouse.React/src/components/layout/MainLayout.tsx` | Exists |
| `Views/Shared/_LayoutAuth.cshtml` | `CoffeeHouse.React/src/layouts/AuthLayout.tsx` | Missing |
| `Views/Shared/_LayoutModern.cshtml` | `CoffeeHouse.React/src/layouts/ModernLayout.tsx` | Missing |
| `Views/Shared/_HeaderPartial.cshtml` | `CoffeeHouse.React/src/components/layout/Navbar.tsx` | Exists |
| `Views/Shared/_HeaderPartial_New.cshtml` | `CoffeeHouse.React/src/components/layout/Navbar.tsx` | Exists (needs UI parity) |
| `Views/Shared/_FooterPartial.cshtml` | `CoffeeHouse.React/src/components/layout/Footer.tsx` | Exists |
| `Views/Shared/_FooterPartial_New.cshtml` | `CoffeeHouse.React/src/components/layout/Footer.tsx` | Exists (needs UI parity) |
| `Views/Shared/_SearchBoxPartial.cshtml` | `CoffeeHouse.React/src/components/search/SearchBox.tsx` | Missing |
| `Views/Shared/_BackToTopPartial.cshtml` | `CoffeeHouse.React/src/components/common/BackToTop.tsx` | Missing |
| `Views/Shared/_InstagramPartial.cshtml` | `CoffeeHouse.React/src/components/common/InstagramStrip.tsx` | Missing |
| `Views/Shared/_PreloaderPartial.cshtml` | `CoffeeHouse.React/src/components/common/Preloader.tsx` | Missing |
| `Views/Shared/_LoadingSkeleton.cshtml` | `CoffeeHouse.React/src/components/common/LoadingSkeleton.tsx` | Missing |
| `Views/Shared/_EmptyState.cshtml` | `CoffeeHouse.React/src/components/common/EmptyState.tsx` | Missing |
| `Views/Shared/_ComponentShowcase.cshtml` | `CoffeeHouse.React/src/components/common/ComponentShowcase.tsx` | Missing |
| `Views/Shared/_ValidationScriptsPartial.cshtml` | N/A | Boilerplate (no UI) |
| `Views/_ViewImports.cshtml` | N/A | Boilerplate (no UI) |
| `Views/_ViewStart.cshtml` | N/A | Boilerplate (no UI) |
| `Views/Shared/Error.cshtml` | `CoffeeHouse.React/src/pages/Error.tsx` | Missing |
| `Views/Shared/Error404.cshtml` | `CoffeeHouse.React/src/pages/NotFound.tsx` | Missing |
| `Views/Shared/Error500.cshtml` | `CoffeeHouse.React/src/pages/Error500.tsx` | Missing |
| `Views/Shared/Offline.cshtml` | `CoffeeHouse.React/src/pages/Offline.tsx` | Missing |
| `Views/Shared/Components/CsrfToken/Default.cshtml` | N/A | Token render (no UI) |
| `Views/Shared/Components/NhomSpMenu/Default.cshtml` | `CoffeeHouse.React/src/components/products/CategoryMenu.tsx` | Missing |
| `Views/Shared/Components/ShoppingCartSummary/Default.cshtml` | `CoffeeHouse.React/src/components/cart/CartSummaryBadge.tsx` | Missing (Navbar is mock) |

### 6.3. Admin area (Areas/Admin/Views)
| Razor View (ASP.NET) | React target | Status |
| :--- | :--- | :--- |
| `Areas/Admin/Views/Shared/_LayoutAdmin.cshtml` | `CoffeeHouse.React/src/admin/layout/AdminLayout.tsx` | Missing |
| `Areas/Admin/Views/Bill/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/BillIndex.tsx` | Missing |
| `Areas/Admin/Views/Bill/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/BillDetails.tsx` | Missing |
| `Areas/Admin/Views/Bill/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/BillSearch.tsx` | Missing |
| `Areas/Admin/Views/Clients/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/ClientsIndex.tsx` | Missing |
| `Areas/Admin/Views/Clients/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/ClientsDetails.tsx` | Missing |
| `Areas/Admin/Views/Clients/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/ClientsEdit.tsx` | Missing |
| `Areas/Admin/Views/Clients/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/ClientsDelete.tsx` | Missing |
| `Areas/Admin/Views/Clients/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/ClientsSearch.tsx` | Missing |
| `Areas/Admin/Views/GroupsProduct/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/GroupsProductIndex.tsx` | Missing |
| `Areas/Admin/Views/GroupsProduct/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/GroupsProductCreate.tsx` | Missing |
| `Areas/Admin/Views/GroupsProduct/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/GroupsProductEdit.tsx` | Missing |
| `Areas/Admin/Views/GroupsProduct/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/GroupsProductSearch.tsx` | Missing |
| `Areas/Admin/Views/HomeAdmin/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/HomeAdminIndex.tsx` | Missing |
| `Areas/Admin/Views/HomeAdmin/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/HomeAdminCreate.tsx` | Missing |
| `Areas/Admin/Views/HomeAdmin/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/HomeAdminEdit.tsx` | Missing |
| `Areas/Admin/Views/HomeAdmin/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/HomeAdminDetails.tsx` | Missing |
| `Areas/Admin/Views/HomeAdmin/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/HomeAdminSearch.tsx` | Missing |
| `Areas/Admin/Views/NewsManage/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/NewsManageIndex.tsx` | Missing |
| `Areas/Admin/Views/NewsManage/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/NewsManageCreate.tsx` | Missing |
| `Areas/Admin/Views/NewsManage/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/NewsManageEdit.tsx` | Missing |
| `Areas/Admin/Views/NewsManage/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/NewsManageDetails.tsx` | Missing |
| `Areas/Admin/Views/NewsManage/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/NewsManageSearch.tsx` | Missing |
| `Areas/Admin/Views/NguyenLieus/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/NguyenLieuIndex.tsx` | Missing |
| `Areas/Admin/Views/NguyenLieus/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/NguyenLieuCreate.tsx` | Missing |
| `Areas/Admin/Views/NguyenLieus/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/NguyenLieuEdit.tsx` | Missing |
| `Areas/Admin/Views/NguyenLieus/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/NguyenLieuDelete.tsx` | Missing |
| `Areas/Admin/Views/NguyenLieus/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/NguyenLieuSearch.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapIndex.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapCreate.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapEdit.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapDetails.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapDelete.tsx` | Missing |
| `Areas/Admin/Views/NhaCungCap/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/NhaCungCapSearch.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienIndex.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienCreate.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienEdit.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienDetails.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienDelete.tsx` | Missing |
| `Areas/Admin/Views/NhanViens/Search.cshtml` | `CoffeeHouse.React/src/admin/pages/NhanVienSearch.tsx` | Missing |
| `Areas/Admin/Views/PhieuNhapHang/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/PhieuNhapHangIndex.tsx` | Missing |
| `Areas/Admin/Views/PhieuNhapHang/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/PhieuNhapHangDetails.tsx` | Missing |
| `Areas/Admin/Views/PhieuNhapHang/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/PhieuNhapHangEdit.tsx` | Missing |
| `Areas/Admin/Views/PhieuNhapHang/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/PhieuNhapHangDelete.tsx` | Missing |
| `Areas/Admin/Views/PhieuNhapHang/Nhap.cshtml` | `CoffeeHouse.React/src/admin/pages/PhieuNhapHangNhap.tsx` | Missing |
| `Areas/Admin/Views/QuanCafe/Index.cshtml` | `CoffeeHouse.React/src/admin/pages/QuanCafeIndex.tsx` | Missing |
| `Areas/Admin/Views/QuanCafe/Create.cshtml` | `CoffeeHouse.React/src/admin/pages/QuanCafeCreate.tsx` | Missing |
| `Areas/Admin/Views/QuanCafe/Edit.cshtml` | `CoffeeHouse.React/src/admin/pages/QuanCafeEdit.tsx` | Missing |
| `Areas/Admin/Views/QuanCafe/Details.cshtml` | `CoffeeHouse.React/src/admin/pages/QuanCafeDetails.tsx` | Missing |
| `Areas/Admin/Views/QuanCafe/Delete.cshtml` | `CoffeeHouse.React/src/admin/pages/QuanCafeDelete.tsx` | Missing |
| `Areas/Admin/Views/ThuChi/All.cshtml` | `CoffeeHouse.React/src/admin/pages/ThuChiAll.tsx` | Missing |
| `Areas/Admin/Views/ThuChi/ThuChiChiTiet.cshtml` | `CoffeeHouse.React/src/admin/pages/ThuChiChiTiet.tsx` | Missing |

---

## 7. Conversion Plan (Razor -> React)
### 7.1. Public MVP (Phase 1)
- Build missing pages: `ProductDetails`, `Cart`, `Checkout`, `OrderSuccess`, `Login`.
- Replace mock data in `Home` and `Products` with live API data.
- Add routing for MVP flow and state management (auth + cart).
- Implement loading / empty / error states for all MVP pages.

### 7.2. Public non-MVP (Phase 2)
- Build `News`, `NewsDetails`, `About`, `Search` pages with API-backed data.
- Build `Register`, `Profile`, `ProfileEdit`, `Orders`, `OrderDetails`.
- Recreate `ProductsModern` and `ProductsByCategory` pages or merge into one smart listing page.

### 7.3. Shared UI / Layout (Phase 3)
- Build missing shared components: SearchBox, BackToTop, InstagramStrip, Preloader, LoadingSkeleton, EmptyState.
- Add AuthLayout and ModernLayout if still needed.
- Align Header/Footer visuals with Razor parity.

### 7.4. Admin Dashboard (Phase 4)
- Create Admin layout and route group (`/admin`).
- Build Admin pages by feature groups (Products, Categories, Orders, Customers, Staff, Inventory, Suppliers, Purchase Orders, Cashflow, News).
- Enforce role-based access for admin routes.

### 7.5. QA / Parity (Phase 5)
- Validate UI parity with Razor (content, layout, states).
- Validate API integration and error handling.
- Cross-browser and mobile QA.

---

## 8. API Wiring Plan
### 8.1. Existing API endpoints (already in backend)
- `GET /api/v1/products` (list, filter, search, pagination).
- `GET /api/v1/products/{id}` (details).
- `GET /api/v1/categories` (category list).
- `POST /api/v1/auth/login`, `GET /api/v1/auth/me`, `POST /api/v1/auth/logout`.
- `POST /api/v1/orders`, `GET /api/v1/orders`, `GET /api/v1/orders/{id}`.
- `GET /api/v1/customers`, `GET /api/v1/customers/{id}`, `PUT /api/v1/customers/{id}` (profile).

### 8.2. Missing endpoints to add (required for full React migration)
- Cart API: `GET/POST/PATCH/DELETE /api/v1/cart` and `GET /api/v1/cart/count`.
- Stores API: `GET /api/v1/stores` (from `GetCafeStoresQuery`).
- News/Blog API: list + detail endpoints.
- Admin CRUD APIs for inventory, suppliers, purchase orders, cashflow, staff, store management.

### 8.3. Page-to-API mapping (public)
- Home: `GET /api/v1/categories`, `GET /api/v1/products` (featured or filtered).
- Products list: `GET /api/v1/products` with search/filter.
- Product details: `GET /api/v1/products/{id}`.
- Cart: use Cart API for add/update/remove, or session-based cart with server sync.
- Checkout: `GET /api/v1/auth/me`, `GET /api/v1/stores`, `POST /api/v1/orders`.
- Orders list/detail: `GET /api/v1/orders`, `GET /api/v1/orders/{id}`.
- Profile: `GET /api/v1/auth/me` and `PUT /api/v1/customers/{id}`.

### 8.4. Page-to-API mapping (admin)
- Products/Categories: use existing admin endpoints or extend `/api/v1/products` and `/api/v1/categories` with admin role.
- Orders/Customers: `/api/v1/orders`, `/api/v1/customers` (admin role).
- Suppliers, Inventory, Purchase Orders, Cashflow, Stores, Staff: add dedicated admin endpoints.

### 8.5. Security and transport rules
- All API calls go through `CoffeeHouse.React/src/services/api.ts`.
- Attach JWT in Authorization header (or HttpOnly cookie if enabled).
- Handle 401 globally (logout + redirect to login).
- Use role-based guards for admin routes.

---
*Plan updated at 16:40, 08/02/2026.*