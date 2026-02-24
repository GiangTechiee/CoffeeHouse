# Coffee Shop Management System

Há»‡ thá»‘ng quáº£n lÃ½ quÃ¡n cÃ  phÃª Ä‘Æ°á»£c phÃ¡t triá»ƒn báº±ng .NET vá»›i MVC, cung cáº¥p giáº£i phÃ¡p toÃ n diá»‡n cho viá»‡c quáº£n lÃ½ váº­n hÃ nh quÃ¡n cÃ  phÃª.

## ðŸ“‹ Má»¥c lá»¥c

- [Tá»•ng quan](#tá»•ng-quan)
- [TÃ­nh nÄƒng](#tÃ­nh-nÄƒng)
- [CÃ´ng nghá»‡ sá»­ dá»¥ng](#cÃ´ng-nghá»‡-sá»­-dá»¥ng)
- [CÃ i Ä‘áº·t](#cÃ i-Ä‘áº·t)
- [Cáº¥u hÃ¬nh](#cáº¥u-hÃ¬nh)
- [Sá»­ dá»¥ng](#sá»­-dá»¥ng)
- [ÄÃ³ng gÃ³p](#Ä‘Ã³ng-gÃ³p)
- [Giáº¥y phÃ©p](#giáº¥y-phÃ©p)

## ðŸŽ¯ Tá»•ng quan

Há»‡ thá»‘ng quáº£n lÃ½ quÃ¡n cÃ  phÃª lÃ  má»™t á»©ng dá»¥ng web Ä‘Æ°á»£c thiáº¿t káº¿ Ä‘á»ƒ há»— trá»£ quáº£n lÃ½ toÃ n diá»‡n cÃ¡c hoáº¡t Ä‘á»™ng cá»§a quÃ¡n cÃ  phÃª, tá»« quáº£n lÃ½ kho hÃ ng, khÃ¡ch hÃ ng, nhÃ  cung cáº¥p Ä‘áº¿n viá»‡c táº¡o hÃ³a Ä‘Æ¡n vÃ  bÃ¡o cÃ¡o thá»‘ng kÃª.

## âœ¨ TÃ­nh nÄƒng

### ðŸª Quáº£n lÃ½ cá»­a hÃ ng
- **Dashboard Admin**: Tá»•ng quan thá»‘ng kÃª doanh thu, sáº£n pháº©m bÃ¡n cháº¡y, khÃ¡ch hÃ ng
- **Quáº£n lÃ½ danh má»¥c**: PhÃ¢n loáº¡i sáº£n pháº©m theo danh má»¥c
- **Quáº£n lÃ½ sáº£n pháº©m**: ThÃªm, sá»­a, xÃ³a thÃ´ng tin sáº£n pháº©m

### ðŸ“¦ Quáº£n lÃ½ kho
- **Xuáº¥t nháº­p kho**: Theo dÃµi hÃ ng hÃ³a vÃ o ra
- **Tá»“n kho**: Kiá»ƒm soÃ¡t sá»‘ lÆ°á»£ng tá»“n kho theo thá»i gian thá»±c
- **BÃ¡o cÃ¡o kho**: Thá»‘ng kÃª xuáº¥t nháº­p tá»“n

### ðŸ‘¥ Quáº£n lÃ½ quan há»‡
- **Quáº£n lÃ½ khÃ¡ch hÃ ng**: ThÃ´ng tin, lá»‹ch sá»­ mua hÃ ng
- **Quáº£n lÃ½ nhÃ  cung cáº¥p**: Danh sÃ¡ch nhÃ  cung cáº¥p, lá»‹ch sá»­ giao dá»‹ch

### ðŸ’° Quáº£n lÃ½ bÃ¡n hÃ ng
- **Táº¡o hÃ³a Ä‘Æ¡n**: Xá»­ lÃ½ Ä‘Æ¡n hÃ ng vÃ  thanh toÃ¡n
- **Lá»‹ch sá»­ giao dá»‹ch**: Theo dÃµi cÃ¡c giao dá»‹ch Ä‘Ã£ thá»±c hiá»‡n
- **BÃ¡o cÃ¡o doanh thu**: Thá»‘ng kÃª theo ngÃ y, thÃ¡ng, nÄƒm

## ðŸ› ï¸ CÃ´ng nghá»‡ sá»­ dá»¥ng

### Backend
- **.NET 8.0**: Framework chÃ­nh
- **ASP.NET Core MVC**: Web framework vá»›i pattern MVC
- **Razor Pages**: View engine cho UI
- **Entity Framework Core**: ORM
- **Neon PostgreSQL**: Co so du lieu

### Frontend
- **HTML5 & CSS3**: Markup vÃ  styling
- **JavaScript/jQuery**: TÆ°Æ¡ng tÃ¡c client-side
- **Bootstrap 5**: CSS framework
- **Chart.js**: Biá»ƒu Ä‘á»“ thá»‘ng kÃª

## ðŸš€ CÃ i Ä‘áº·t

### YÃªu cáº§u há»‡ thá»‘ng
- .NET 8.0 SDK
- Neon PostgreSQL (connection string)
- Visual Studio 2022 hoáº·c VS Code

### BÆ°á»›c cÃ i Ä‘áº·t

1. **Clone repository**
```bash
git clone https://github.com/yourusername/coffee-shop-management.git
cd coffee-shop-management
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Cau hinh Neon PostgreSQL**
- Tao database tren Neon
- Lay connection string va set vao `ConnectionStrings__CoffeeHouseDb`

4. **Cáº¥u hÃ¬nh connection string**
```bash
# Sao chÃ©p file cáº¥u hÃ¬nh
cp appsettings.example.json appsettings.json
```

5. **Cháº¡y migration**
```bash
dotnet ef database update
```

6. **Seed dá»¯ liá»‡u máº«u** (tÃ¹y chá»n)
```bash
dotnet run --seed-data
```

7. **Cháº¡y á»©ng dá»¥ng**
```bash
dotnet run
```

á»¨ng dá»¥ng sáº½ cháº¡y táº¡i: `https://localhost:5001`

## âš™ï¸ Cáº¥u hÃ¬nh

### Database Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "CoffeeHouseDb": "Host=YOUR_NEON_HOST;Port=5432;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;Ssl Mode=Require;Trust Server Certificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## ðŸ“– Sá»­ dá»¥ng

### ÄÄƒng nháº­p Admin
- URL: `https://localhost:5001/admin`
- Username: `admin@coffeeshop.com`
- Password: `Admin123!`

### CÃ¡c chá»©c nÄƒng chÃ­nh

1. **Dashboard**: Xem tá»•ng quan thá»‘ng kÃª
2. **Sáº£n pháº©m**: Quáº£n lÃ½ menu vÃ  giÃ¡ cáº£
3. **Kho hÃ ng**: Theo dÃµi tá»“n kho vÃ  xuáº¥t nháº­p
4. **KhÃ¡ch hÃ ng**: Quáº£n lÃ½ thÃ´ng tin khÃ¡ch hÃ ng
5. **HÃ³a Ä‘Æ¡n**: Táº¡o vÃ  quáº£n lÃ½ Ä‘Æ¡n hÃ ng
6. **BÃ¡o cÃ¡o**: Xem bÃ¡o cÃ¡o doanh thu vÃ  thá»‘ng kÃª

## ðŸš€ Deployment

### Docker
```bash
# Build image
docker build -t coffee-shop-management .

# Run container
docker-compose up -d
```

### Production Setup
1. Cáº¥u hÃ¬nh production database
2. Thiáº¿t láº­p HTTPS certificates
3. Cáº¥u hÃ¬nh logging vÃ  monitoring
4. Setup backup strategy

## ðŸ¤ ÄÃ³ng gÃ³p

1. Fork repository
2. Táº¡o feature branch: `git checkout -b feature/new-feature`
3. Commit changes: `git commit -am 'Add new feature'`
4. Push to branch: `git push origin feature/new-feature`
5. Táº¡o Pull Request

### Coding Standards
- Sá»­ dá»¥ng C# naming conventions
- TuÃ¢n thá»§ clean code principles
- Sá»­ dá»¥ng async/await pattern

## ðŸ“„ Giáº¥y phÃ©p

Dá»± Ã¡n nÃ y Ä‘Æ°á»£c cáº¥p phÃ©p dÆ°á»›i [MIT License](LICENSE).

## ðŸ‘¥ TÃ¡c giáº£

- **GiangTechiee** - *Developer* - [GitHub](https://github.com/GiangTechiee)

## ðŸ™ Acknowledgments

- Entity Framework Core documentation
- ASP.NET Core community
- PostgreSQL team
- Bootstrap CSS framework

---

**LÆ°u Ã½**: ÄÃ¢y lÃ  phiÃªn báº£n development. Vui lÃ²ng khÃ´ng sá»­ dá»¥ng trong production mÃ  khÃ´ng thá»±c hiá»‡n security audit vÃ  performance testing.


