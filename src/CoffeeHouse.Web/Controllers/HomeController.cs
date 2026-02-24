using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoffeeHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CoffeeHouseContext _context;

        public HomeController(ILogger<HomeController> logger, CoffeeHouseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return Content(@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Hệ thống Backend CoffeeHouse - Thông tin Kỹ thuật</title>
    <link href=""https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;600&family=Playfair+Display:ital,wght@0,700;1,700&display=swap"" rel=""stylesheet"">
    <style>
        :root {
            --coffee-900: #2C1810;
            --coffee-800: #3D2B1F;
            --coffee-700: #5D4037;
            --coffee-100: #D7CCC8;
            --coffee-50: #EFEBE9;
            --accent: #D4A373;
            --info: #0288D1;
        }

        body {
            margin: 0; padding: 0;
            font-family: 'Outfit', sans-serif;
            background-color: var(--coffee-50);
            color: var(--coffee-900);
            line-height: 1.6;
        }

        .hero {
            background: white;
            padding: 80px 20px;
            text-align: center;
            border-bottom: 1px solid var(--coffee-100);
        }

        .container {
            max-width: 1000px;
            margin: -40px auto 100px;
            padding: 0 20px;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 24px;
        }

        .card {
            background: white;
            padding: 32px;
            border-radius: 20px;
            box-shadow: 0 10px 30px rgba(44, 24, 16, 0.05);
            transition: transform 0.3s ease;
        }

        .card:hover { transform: translateY(-5px); }

        .logo {
            font-family: 'Playfair Display', serif;
            font-size: 3rem;
            color: var(--coffee-800);
            margin-bottom: 20px;
            font-style: italic;
        }

        .section-title {
            font-size: 0.75rem;
            text-transform: uppercase;
            letter-spacing: 0.2em;
            color: var(--accent);
            font-weight: 600;
            margin-bottom: 40px;
            text-align: center;
        }

        .icon-box {
            width: 48px;
            height: 48px;
            background: var(--coffee-50);
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            margin-bottom: 20px;
        }

        .dev-badge {
            display: inline-block;
            padding: 4px 12px;
            background: var(--coffee-900);
            color: white;
            border-radius: 100px;
            font-size: 0.7rem;
            font-weight: 600;
            margin-bottom: 16px;
        }

        .insight-box {
            margin-top: 24px;
            padding: 16px;
            background: #F0F7FF;
            border-left: 4px solid var(--info);
            font-size: 0.85rem;
            color: #444;
            border-radius: 0 8px 8px 0;
        }

        .btn {
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            background: var(--coffee-800);
            color: white;
            text-decoration: none;
            border-radius: 10px;
            font-size: 0.9rem;
            font-weight: 600;
            transition: background 0.3s;
        }

        .btn:hover { background: var(--coffee-700); }

        footer {
            text-align: center;
            padding: 40px;
            color: var(--coffee-700);
            font-size: 0.8rem;
            letter-spacing: 0.1em;
        }
    </style>
</head>
<body>
    <div class=""hero"">
        <div class=""logo"">CoffeeHouse System</div>
        <p style=""max-width: 600px; margin: 0 auto; color: var(--coffee-700);"">
            Hệ thống API hiệu năng cao xây dựng trên nền tảng .NET 8, tối ưu hóa cho khả năng mở rộng, bảo mật và trải nghiệm phát triển.
        </p>
    </div>

    <div class=""container"">
        <div class=""section-title"">Hệ sinh thái Kỹ thuật</div>
        
        <div class=""grid"">
            <!-- RESTful Documentation -->
            <div class=""card"">
                <div class=""icon-box"">📚</div>
                <div class=""dev-badge"">SWAGGER UI</div>
                <h3>Tài liệu API</h3>
                <p>Khám phá và kiểm thử các tài nguyên hệ thống thông qua giao diện tương tác chuẩn OpenAPI 3.0.</p>
                <a href=""/api-docs"" class=""btn"">Xem Tài liệu</a>
                <div class=""insight-box"">
                    <b>Kiến trúc:</b> Hệ thống áp dụng <b>API Versioning (v1)</b> nhằm duy trì tính nhất quán và cho phép nâng cấp các phiên bản logic mà không làm gián đoạn ứng dụng khách.
                </div>
            </div>

            <!-- System Surveillance -->
            <div class=""card"">
                <div class=""icon-box"">🏥</div>
                <div class=""dev-badge"">GIÁM SÁT</div>
                <h3>Trạng thái Vận hành</h3>
                <p>Theo dõi thời gian thực tình trạng của các dịch vụ, Cơ sở dữ liệu (PostgreSQL) và các thành phần hạ tầng.</p>
                <a href=""/health"" class=""btn"" style=""background: #2D6A4F;"">Kiểm tra Hệ thống</a>
                <div class=""insight-box"">
                    <b>Vận hành:</b> Triển khai <b>Middleware HealthCheck</b> giúp tự động hóa quá trình giám sát, sẵn sàng tích hợp với các hệ thống CI/CD và điều phối container.
                </div>
            </div>

            <!-- Security Layer -->
            <div class=""card"">
                <div class=""icon-box"">🛡️</div>
                <div class=""dev-badge"">LỚP BẢO MẬT</div>
                <h3>Xác thực & Bảo vệ</h3>
                <p>Cơ chế bảo mật đa tầng kết hợp giữa JWT (JSON Web Token) và Identity Framework chuẩn công nghiệp.</p>
                <div class=""insight-box"">
                    <b>An toàn dữ liệu:</b> Phân tách rõ ràng giữa dữ liệu <b>Công khai (Read-only)</b> phục vụ hiển thị và dữ liệu <b>Nội bộ</b> yêu cầu định danh qua Claims-based Authorization.
                </div>
                <div style=""display: flex; gap: 10px; margin-top: 15px;"">
                    <a href=""/api/v1/products"" style=""font-size: 0.8rem; color: var(--accent);"">Danh sách Sản phẩm →</a>
                    <a href=""/api/v1/categories"" style=""font-size: 0.8rem; color: var(--accent);"">Danh sách Danh mục →</a>
                </div>
            </div>
        </div>
    </div>

    <footer>
        &copy; 2026 Crafted with Excellence by CoffeeHouse Developer
    </footer>
</body>
</html>
", "text/html");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



