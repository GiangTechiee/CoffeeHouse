using CoffeeHouse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoffeeHouse
{
    public class CoffeeHouseContext : DbContext
    {
        public CoffeeHouseContext(DbContextOptions<CoffeeHouseContext> options) : base(options) { }

        // DbSet cho từng entity
        public DbSet<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; }
        public DbSet<TbGioHang> TbGioHangs { get; set; }
        public DbSet<TbHoaDonBan> TbHoaDonBans { get; set; }
        public DbSet<TbKhachHang> TbKhachHangs { get; set; }
        public DbSet<TbNguyenLieu> TbNguyenLieus { get; set; }
        public DbSet<TbNhaCungCap> TbNhaCungCaps { get; set; }
        public DbSet<TbNhanVien> TbNhanViens { get; set; }
        public DbSet<TbNhomSanPham> TbNhomSanPhams { get; set; }
        public DbSet<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; }
        public DbSet<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; }
        public DbSet<TbQuanCafe> TbQuanCafes { get; set; }
        public DbSet<TbQuanTriVien> TbQuanTriViens { get; set; }
        public DbSet<TbQuyen> TbQuyens { get; set; }
        public DbSet<TbSanPham> TbSanPhams { get; set; }
        public DbSet<TbTaiKhoan> TbTaiKhoans { get; set; }
        public DbSet<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; }
        public DbSet<TbTinTuc> TbTinTucs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình chuyển đổi DateTime sang UTC
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(nullableDateTimeConverter);
                    }
                }
            }


            // Cấu hình khóa chính composite
            modelBuilder.Entity<TbChiTietHoaDonBan>()
                .HasKey(c => new { c.MaHoaDon, c.MaSanPham });

            modelBuilder.Entity<TbGioHang>()
                .HasKey(g => new { g.MaKhachHang, g.MaSanPham });

            modelBuilder.Entity<TbPhieuNhapChiTiet>()
                .HasKey(p => new { p.MaPhieuNhap, p.MaNguyenLieu });

            // Cấu hình mối quan hệ
            // TbHoaDonBan
            modelBuilder.Entity<TbHoaDonBan>()
                .HasMany(h => h.TbChiTietHoaDonBans)
                .WithOne(c => c.MaHoaDonNavigation)
                .HasForeignKey(c => c.MaHoaDon);

            modelBuilder.Entity<TbHoaDonBan>()
                .HasOne(h => h.MaQuanNavigation)
                .WithMany(q => q.TbHoaDonBans)
                .HasForeignKey(h => h.MaQuan);

            modelBuilder.Entity<TbHoaDonBan>()
                .HasOne(h => h.MaNhanVienNavigation)
                .WithMany(n => n.TbHoaDonBans)
                .HasForeignKey(h => h.MaNhanVien);

            modelBuilder.Entity<TbHoaDonBan>()
                .HasOne(h => h.MaKhachHangNavigation)
                .WithMany(k => k.TbHoaDonBans)
                .HasForeignKey(h => h.MaKhachHang);

            // TbSanPham
            modelBuilder.Entity<TbSanPham>()
                .HasOne(s => s.MaNhomSpNavigation)
                .WithMany(n => n.TbSanPhams)
                .HasForeignKey(s => s.MaNhomSp);

            modelBuilder.Entity<TbSanPham>()
                .HasMany(s => s.TbChiTietHoaDonBans)
                .WithOne(c => c.MaSanPhamNavigation)
                .HasForeignKey(c => c.MaSanPham);

            modelBuilder.Entity<TbSanPham>()
                .HasMany(s => s.TbGioHangs)
                .WithOne(g => g.MaSanPhamNavigation)
                .HasForeignKey(g => g.MaSanPham);

            // TbKhachHang
            modelBuilder.Entity<TbKhachHang>()
                .HasMany(k => k.TbGioHangs)
                .WithOne(g => g.MaKhachHangNavigation)
                .HasForeignKey(g => g.MaKhachHang);

            modelBuilder.Entity<TbKhachHang>()
                .HasMany(k => k.TbTaiKhoanKhs)
                .WithOne(t => t.MaKhachHangNavigation)
                .HasForeignKey(t => t.MaKhachHang);

            // TbNhanVien
            modelBuilder.Entity<TbNhanVien>()
                .HasOne(n => n.MaQuanNavigation)
                .WithMany(q => q.TbNhanViens)
                .HasForeignKey(n => n.MaQuan);

            modelBuilder.Entity<TbNhanVien>()
                .HasMany(n => n.TbTaiKhoans)
                .WithOne(t => t.MaNhanVienNavigation)
                .HasForeignKey(t => t.MaNhanVien);

            modelBuilder.Entity<TbNhanVien>()
                .HasMany(n => n.TbPhieuNhapHangs)
                .WithOne(p => p.MaNhanVienNavigation)
                .HasForeignKey(p => p.MaNhanVien);

            // TbPhieuNhapHang
            modelBuilder.Entity<TbPhieuNhapHang>()
                .HasMany(p => p.TbPhieuNhapChiTiets)
                .WithOne(c => c.MaPhieuNhapNavigation)
                .HasForeignKey(c => c.MaPhieuNhap);

            modelBuilder.Entity<TbPhieuNhapHang>()
                            .HasOne(p => p.MaNhaCungCapNavigation)
                            .WithMany(n => n.TbPhieuNhapHangs)
                            .HasForeignKey(p => p.MaNhaCungCap);

            modelBuilder.Entity<TbPhieuNhapHang>()
                .HasOne(p => p.MaQuanNavigation)
                .WithMany(q => q.TbPhieuNhapHangs)
                .HasForeignKey(p => p.MaQuan);

            // TbNguyenLieu
            modelBuilder.Entity<TbNguyenLieu>()
                .HasMany(n => n.TbPhieuNhapChiTiets)
                .WithOne(c => c.MaNguyenLieuNavigation)
                .HasForeignKey(c => c.MaNguyenLieu);

            // TbQuyen
            modelBuilder.Entity<TbQuyen>()
                .HasMany(q => q.TbTaiKhoans)
                .WithOne(t => t.MaQuyenNavigation)
                .HasForeignKey(t => t.MaQuyen);

            modelBuilder.Entity<TbQuyen>()
                .HasMany(q => q.TbTaiKhoanKhs)
                .WithOne(t => t.MaQuyenNavigation)
                .HasForeignKey(t => t.MaQuyen);

            // Cấu hình độ chính xác cho các trường decimal
            modelBuilder.Entity<TbChiTietHoaDonBan>()
                .Property(c => c.DonGia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbChiTietHoaDonBan>()
                .Property(c => c.ThanhTien)
                .HasPrecision(21, 2);

            modelBuilder.Entity<TbHoaDonBan>()
                .Property(h => h.TongTien)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbNguyenLieu>()
                .Property(n => n.SoLuong)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbNguyenLieu>()
                .Property(n => n.DonGia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbNguyenLieu>()
                .Property(n => n.SoLuongToiThieu)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbNhanVien>()
                .Property(n => n.LuongCoBan)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbNhanVien>()
                .Property(n => n.HeSoLuong)
                .HasPrecision(4, 2);

            modelBuilder.Entity<TbPhieuNhapChiTiet>()
                .Property(p => p.SoLuong)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbPhieuNhapChiTiet>()
                .Property(p => p.DonGia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TbPhieuNhapChiTiet>()
                .Property(p => p.ThanhTien)
                .HasPrecision(21, 4);

            modelBuilder.Entity<TbSanPham>()
                .Property(s => s.GiaBan)
                .HasPrecision(10, 2);

            // Thêm các chỉ mục (index)
            modelBuilder.Entity<TbKhachHang>()
                .HasIndex(k => k.SdtkhachHang)
                .HasDatabaseName("IX_TbKhachHang_SdtkhachHang");

            modelBuilder.Entity<TbNhaCungCap>()
                .HasIndex(n => n.Sdtncc)
                .HasDatabaseName("IX_TbNhaCungCap_Sdtncc");

            modelBuilder.Entity<TbNhanVien>()
                .HasIndex(n => n.SoCccd)
                .HasDatabaseName("IX_TbNhanVien_SoCccd");

            modelBuilder.Entity<TbNhanVien>()
                .HasIndex(n => n.Email)
                .HasDatabaseName("IX_TbNhanVien_Email");

            modelBuilder.Entity<TbNhanVien>()
                .HasIndex(n => n.Sdt)
                .HasDatabaseName("IX_TbNhanVien_Sdt");

            modelBuilder.Entity<TbQuanCafe>()
                .HasIndex(q => q.Email)
                .HasDatabaseName("IX_TbQuanCafe_Email");

            modelBuilder.Entity<TbQuanCafe>()
                .HasIndex(q => q.Sdt)
                .HasDatabaseName("IX_TbQuanCafe_Sdt");

            modelBuilder.Entity<TbQuyen>()
                .HasIndex(q => q.TenQuyen)
                .HasDatabaseName("IX_TbQuyen_TenQuyen");

            modelBuilder.Entity<TbSanPham>()
                .HasIndex(s => s.TenSanPham)
                .HasDatabaseName("IX_TbSanPham_TenSanPham");

            modelBuilder.Entity<TbTaiKhoan>()
                .HasIndex(t => t.TenTaiKhoan)
                .HasDatabaseName("IX_TbTaiKhoan_TenTaiKhoan");

            modelBuilder.Entity<TbTaiKhoanKh>()
                .HasIndex(t => t.TenTaiKhoan)
                .HasDatabaseName("IX_TbTaiKhoanKh_TenTaiKhoan");
        }
    }
}
