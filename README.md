# Coffee Shop Management System

Hệ thống quản lý quán cà phê được phát triển bằng .NET với MVC, cung cấp giải pháp toàn diện cho việc quản lý vận hành quán cà phê.

## 📋 Mục lục

- [Tổng quan](#tổng-quan)
- [Tính năng](#tính-năng)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Cài đặt](#cài-đặt)
- [Cấu hình](#cấu-hình)
- [Sử dụng](#sử-dụng)
- [Đóng góp](#đóng-góp)
- [Giấy phép](#giấy-phép)

## 🎯 Tổng quan

Hệ thống quản lý quán cà phê là một ứng dụng web được thiết kế để hỗ trợ quản lý toàn diện các hoạt động của quán cà phê, từ quản lý kho hàng, khách hàng, nhà cung cấp đến việc tạo hóa đơn và báo cáo thống kê.

## ✨ Tính năng

### 🏪 Quản lý cửa hàng
- **Dashboard Admin**: Tổng quan thống kê doanh thu, sản phẩm bán chạy, khách hàng
- **Quản lý danh mục**: Phân loại sản phẩm theo danh mục
- **Quản lý sản phẩm**: Thêm, sửa, xóa thông tin sản phẩm

### 📦 Quản lý kho
- **Xuất nhập kho**: Theo dõi hàng hóa vào ra
- **Tồn kho**: Kiểm soát số lượng tồn kho theo thời gian thực
- **Báo cáo kho**: Thống kê xuất nhập tồn

### 👥 Quản lý quan hệ
- **Quản lý khách hàng**: Thông tin, lịch sử mua hàng
- **Quản lý nhà cung cấp**: Danh sách nhà cung cấp, lịch sử giao dịch

### 💰 Quản lý bán hàng
- **Tạo hóa đơn**: Xử lý đơn hàng và thanh toán
- **Lịch sử giao dịch**: Theo dõi các giao dịch đã thực hiện
- **Báo cáo doanh thu**: Thống kê theo ngày, tháng, năm

## 🛠️ Công nghệ sử dụng

### Backend
- **.NET 8.0**: Framework chính
- **ASP.NET Core MVC**: Web framework với pattern MVC
- **Razor Pages**: View engine cho UI
- **Entity Framework Core**: ORM
- **PostgreSQL**: Cơ sở dữ liệu

### Frontend
- **HTML5 & CSS3**: Markup và styling
- **JavaScript/jQuery**: Tương tác client-side
- **Bootstrap 5**: CSS framework
- **Chart.js**: Biểu đồ thống kê

## 🚀 Cài đặt

### Yêu cầu hệ thống
- .NET 8.0 SDK
- PostgreSQL 13+
- Visual Studio 2022 hoặc VS Code

### Bước cài đặt

1. **Clone repository**
```bash
git clone https://github.com/yourusername/coffee-shop-management.git
cd coffee-shop-management
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Cài đặt PostgreSQL**
- Tải và cài đặt PostgreSQL
- Tạo database mới: `coffee_shop_db`

4. **Cấu hình connection string**
```bash
# Sao chép file cấu hình
cp appsettings.example.json appsettings.json
```

5. **Chạy migration**
```bash
dotnet ef database update
```

6. **Seed dữ liệu mẫu** (tùy chọn)
```bash
dotnet run --seed-data
```

7. **Chạy ứng dụng**
```bash
dotnet run
```

Ứng dụng sẽ chạy tại: `https://localhost:5001`

## ⚙️ Cấu hình

### Database Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=coffee_shop_db;Username=your_username;Password=your_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📖 Sử dụng

### Đăng nhập Admin
- URL: `https://localhost:5001/admin`
- Username: `admin@coffeeshop.com`
- Password: `Admin123!`

### Các chức năng chính

1. **Dashboard**: Xem tổng quan thống kê
2. **Sản phẩm**: Quản lý menu và giá cả
3. **Kho hàng**: Theo dõi tồn kho và xuất nhập
4. **Khách hàng**: Quản lý thông tin khách hàng
5. **Hóa đơn**: Tạo và quản lý đơn hàng
6. **Báo cáo**: Xem báo cáo doanh thu và thống kê

## 🚀 Deployment

### Docker
```bash
# Build image
docker build -t coffee-shop-management .

# Run container
docker-compose up -d
```

### Production Setup
1. Cấu hình production database
2. Thiết lập HTTPS certificates
3. Cấu hình logging và monitoring
4. Setup backup strategy

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch: `git checkout -b feature/new-feature`
3. Commit changes: `git commit -am 'Add new feature'`
4. Push to branch: `git push origin feature/new-feature`
5. Tạo Pull Request

### Coding Standards
- Sử dụng C# naming conventions
- Tuân thủ clean code principles
- Sử dụng async/await pattern

## 📄 Giấy phép

Dự án này được cấp phép dưới [MIT License](LICENSE).

## 👥 Tác giả

- **GiangTechiee** - *Developer* - [GitHub](https://github.com/GiangTechiee)

## 🙏 Acknowledgments

- Entity Framework Core documentation
- ASP.NET Core community
- PostgreSQL team
- Bootstrap CSS framework

---

**Lưu ý**: Đây là phiên bản development. Vui lòng không sử dụng trong production mà không thực hiện security audit và performance testing.
