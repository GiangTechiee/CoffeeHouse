using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoffeeHouse.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbKhachHangs",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenKhachHang = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SdtkhachHang = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbKhachHangs", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "TbNguyenLieus",
                columns: table => new
                {
                    MaNguyenLieu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNguyenLieu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DonViTinh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HanSuDung = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DonGia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SoLuongToiThieu = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNguyenLieus", x => x.MaNguyenLieu);
                });

            migrationBuilder.CreateTable(
                name: "TbNhaCungCaps",
                columns: table => new
                {
                    MaNhaCungCap = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNhaCungCap = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Sdtncc = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Stk = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNhaCungCaps", x => x.MaNhaCungCap);
                });

            migrationBuilder.CreateTable(
                name: "TbNhomSanPhams",
                columns: table => new
                {
                    MaNhomSp = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNhomSp = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNhomSanPhams", x => x.MaNhomSp);
                });

            migrationBuilder.CreateTable(
                name: "TbQuanCafes",
                columns: table => new
                {
                    MaQuan = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenQuan = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Sdt = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbQuanCafes", x => x.MaQuan);
                });

            migrationBuilder.CreateTable(
                name: "TbQuanTriViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNguoiDung = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbQuanTriViens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbQuyens",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenQuyen = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbQuyens", x => x.MaQuyen);
                });

            migrationBuilder.CreateTable(
                name: "TbTinTucs",
                columns: table => new
                {
                    MaTinTuc = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TieuDe = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NgayDang = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NoiDung = table.Column<string>(type: "text", nullable: false),
                    HinhAnh = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTinTucs", x => x.MaTinTuc);
                });

            migrationBuilder.CreateTable(
                name: "TbSanPhams",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenSanPham = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    GiaBan = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    HinhAnh = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    MaNhomSp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbSanPhams", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_TbSanPhams_TbNhomSanPhams_MaNhomSp",
                        column: x => x.MaNhomSp,
                        principalTable: "TbNhomSanPhams",
                        principalColumn: "MaNhomSp",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbNhanViens",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaQuan = table.Column<int>(type: "integer", nullable: false),
                    HoTen = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GioiTinh = table.Column<bool>(type: "boolean", nullable: true),
                    ChucVu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sdt = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    SoCccd = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LuongCoBan = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    HeSoLuong = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNhanViens", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_TbNhanViens_TbQuanCafes_MaQuan",
                        column: x => x.MaQuan,
                        principalTable: "TbQuanCafes",
                        principalColumn: "MaQuan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbTaiKhoanKhs",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenTaiKhoan = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MaKhachHang = table.Column<int>(type: "integer", nullable: false),
                    MaQuyen = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTaiKhoanKhs", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoanKhs_TbKhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "TbKhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoanKhs_TbQuyens_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "TbQuyens",
                        principalColumn: "MaQuyen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbGioHangs",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "integer", nullable: false),
                    MaSanPham = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbGioHangs", x => new { x.MaKhachHang, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_TbGioHangs_TbKhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "TbKhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbGioHangs_TbSanPhams_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "TbSanPhams",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbHoaDonBans",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uuid", nullable: false),
                    MaQuan = table.Column<int>(type: "integer", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaNhanVien = table.Column<int>(type: "integer", nullable: true),
                    MaKhachHang = table.Column<int>(type: "integer", nullable: false),
                    HinhThucThanhToan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TongTien = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TrangThai = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbHoaDonBans", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_TbHoaDonBans_TbKhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "TbKhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbHoaDonBans_TbNhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "TbNhanViens",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_TbHoaDonBans_TbQuanCafes_MaQuan",
                        column: x => x.MaQuan,
                        principalTable: "TbQuanCafes",
                        principalColumn: "MaQuan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbPhieuNhapHangs",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<Guid>(type: "uuid", nullable: false),
                    MaQuan = table.Column<int>(type: "integer", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaNhanVien = table.Column<int>(type: "integer", nullable: false),
                    MaNhaCungCap = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPhieuNhapHangs", x => x.MaPhieuNhap);
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapHangs_TbNhaCungCaps_MaNhaCungCap",
                        column: x => x.MaNhaCungCap,
                        principalTable: "TbNhaCungCaps",
                        principalColumn: "MaNhaCungCap",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapHangs_TbNhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "TbNhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapHangs_TbQuanCafes_MaQuan",
                        column: x => x.MaQuan,
                        principalTable: "TbQuanCafes",
                        principalColumn: "MaQuan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbTaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenTaiKhoan = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MatKhauHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MaNhanVien = table.Column<int>(type: "integer", nullable: false),
                    MaQuyen = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTaiKhoans", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoans_TbNhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "TbNhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoans_TbQuyens_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "TbQuyens",
                        principalColumn: "MaQuyen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbChiTietHoaDonBans",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSanPham = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(15,2)", precision: 21, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbChiTietHoaDonBans", x => new { x.MaHoaDon, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_TbChiTietHoaDonBans_TbHoaDonBans_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "TbHoaDonBans",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbChiTietHoaDonBans_TbSanPhams_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "TbSanPhams",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbPhieuNhapChiTiets",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<Guid>(type: "uuid", nullable: false),
                    MaNguyenLieu = table.Column<int>(type: "integer", nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(20,4)", precision: 21, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPhieuNhapChiTiets", x => new { x.MaPhieuNhap, x.MaNguyenLieu });
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapChiTiets_TbNguyenLieus_MaNguyenLieu",
                        column: x => x.MaNguyenLieu,
                        principalTable: "TbNguyenLieus",
                        principalColumn: "MaNguyenLieu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapChiTiets_TbPhieuNhapHangs_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "TbPhieuNhapHangs",
                        principalColumn: "MaPhieuNhap",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbChiTietHoaDonBans_MaSanPham",
                table: "TbChiTietHoaDonBans",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_TbGioHangs_MaSanPham",
                table: "TbGioHangs",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_MaKhachHang",
                table: "TbHoaDonBans",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_MaNhanVien",
                table: "TbHoaDonBans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_MaQuan",
                table: "TbHoaDonBans",
                column: "MaQuan");

            migrationBuilder.CreateIndex(
                name: "IX_TbKhachHang_SdtkhachHang",
                table: "TbKhachHangs",
                column: "SdtkhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhaCungCap_Sdtncc",
                table: "TbNhaCungCaps",
                column: "Sdtncc");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_Email",
                table: "TbNhanViens",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_Sdt",
                table: "TbNhanViens",
                column: "Sdt");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_SoCccd",
                table: "TbNhanViens",
                column: "SoCccd");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanViens_MaQuan",
                table: "TbNhanViens",
                column: "MaQuan");

            migrationBuilder.CreateIndex(
                name: "IX_TbPhieuNhapChiTiets_MaNguyenLieu",
                table: "TbPhieuNhapChiTiets",
                column: "MaNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_TbPhieuNhapHangs_MaNhaCungCap",
                table: "TbPhieuNhapHangs",
                column: "MaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_TbPhieuNhapHangs_MaNhanVien",
                table: "TbPhieuNhapHangs",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TbPhieuNhapHangs_MaQuan",
                table: "TbPhieuNhapHangs",
                column: "MaQuan");

            migrationBuilder.CreateIndex(
                name: "IX_TbQuanCafe_Email",
                table: "TbQuanCafes",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TbQuanCafe_Sdt",
                table: "TbQuanCafes",
                column: "Sdt");

            migrationBuilder.CreateIndex(
                name: "IX_TbQuyen_TenQuyen",
                table: "TbQuyens",
                column: "TenQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_TbSanPham_TenSanPham",
                table: "TbSanPhams",
                column: "TenSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_TbSanPhams_MaNhomSp",
                table: "TbSanPhams",
                column: "MaNhomSp");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKh_TenTaiKhoan",
                table: "TbTaiKhoanKhs",
                column: "TenTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKhs_MaKhachHang",
                table: "TbTaiKhoanKhs",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKhs_MaQuyen",
                table: "TbTaiKhoanKhs",
                column: "MaQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoan_TenTaiKhoan",
                table: "TbTaiKhoans",
                column: "TenTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoans_MaNhanVien",
                table: "TbTaiKhoans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoans_MaQuyen",
                table: "TbTaiKhoans",
                column: "MaQuyen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbChiTietHoaDonBans");

            migrationBuilder.DropTable(
                name: "TbGioHangs");

            migrationBuilder.DropTable(
                name: "TbPhieuNhapChiTiets");

            migrationBuilder.DropTable(
                name: "TbQuanTriViens");

            migrationBuilder.DropTable(
                name: "TbTaiKhoanKhs");

            migrationBuilder.DropTable(
                name: "TbTaiKhoans");

            migrationBuilder.DropTable(
                name: "TbTinTucs");

            migrationBuilder.DropTable(
                name: "TbHoaDonBans");

            migrationBuilder.DropTable(
                name: "TbSanPhams");

            migrationBuilder.DropTable(
                name: "TbNguyenLieus");

            migrationBuilder.DropTable(
                name: "TbPhieuNhapHangs");

            migrationBuilder.DropTable(
                name: "TbQuyens");

            migrationBuilder.DropTable(
                name: "TbKhachHangs");

            migrationBuilder.DropTable(
                name: "TbNhomSanPhams");

            migrationBuilder.DropTable(
                name: "TbNhaCungCaps");

            migrationBuilder.DropTable(
                name: "TbNhanViens");

            migrationBuilder.DropTable(
                name: "TbQuanCafes");
        }
    }
}
