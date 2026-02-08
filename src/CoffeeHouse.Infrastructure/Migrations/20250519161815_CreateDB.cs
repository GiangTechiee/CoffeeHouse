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
                    CustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumberkhachHang = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbKhachHangs", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "TbNguyenLieus",
                columns: table => new
                {
                    IngredientId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IngredientName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    QuantityToiThieu = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNguyenLieus", x => x.IngredientId);
                });

            migrationBuilder.CreateTable(
                name: "TbNhaCungCaps",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupplierName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Stk = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNhaCungCaps", x => x.SupplierId);
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
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbQuanCafes", x => x.MaQuan);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenNguoiDung = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbQuyens",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenQuyen = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbQuyens", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TbTinTucs",
                columns: table => new
                {
                    NewsArticleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTinTucs", x => x.NewsArticleId);
                });

            migrationBuilder.CreateTable(
                name: "TbSanPhams",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    GiaBan = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
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
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<bool>(type: "boolean", nullable: true),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    IdentityCardNumber = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SalaryCoefficient = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false)
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
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTaiKhoanKhs", x => x.MaTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoanKhs_TbKhachHangs_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TbKhachHangs",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbTaiKhoanKhs_TbQuyens_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TbQuyens",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    MaSanPham = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.CustomerId, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_CartItems_TbKhachHangs_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TbKhachHangs",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_TbSanPhams_MaSanPham",
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
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbHoaDonBans", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_TbHoaDonBans_TbKhachHangs_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TbKhachHangs",
                        principalColumn: "CustomerId",
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
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaQuan = table.Column<int>(type: "integer", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaNhanVien = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPhieuNhapHangs", x => x.PurchaseOrderId);
                    table.ForeignKey(
                        name: "FK_TbPhieuNhapHangs_TbNhaCungCaps_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "TbNhaCungCaps",
                        principalColumn: "SupplierId",
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
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MaNhanVien = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
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
                        name: "FK_TbTaiKhoans_TbQuyens_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TbQuyens",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbChiTietHoaDonBans",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSanPham = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(15,2)", precision: 21, scale: 2, nullable: true)
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
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(20,4)", precision: 21, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => new { x.PurchaseOrderId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_TbNguyenLieus_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "TbNguyenLieus",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_TbPhieuNhapHangs_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "TbPhieuNhapHangs",
                        principalColumn: "PurchaseOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbChiTietHoaDonBans_MaSanPham",
                table: "TbChiTietHoaDonBans",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MaSanPham",
                table: "CartItems",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_CustomerId",
                table: "TbHoaDonBans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_MaNhanVien",
                table: "TbHoaDonBans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TbHoaDonBans_MaQuan",
                table: "TbHoaDonBans",
                column: "MaQuan");

            migrationBuilder.CreateIndex(
                name: "IX_TbKhachHang_PhoneNumberkhachHang",
                table: "TbKhachHangs",
                column: "PhoneNumberkhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhaCungCap_PhoneNumber",
                table: "TbNhaCungCaps",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_Email",
                table: "TbNhanViens",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_PhoneNumber",
                table: "TbNhanViens",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanVien_IdentityCardNumber",
                table: "TbNhanViens",
                column: "IdentityCardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TbNhanViens_MaQuan",
                table: "TbNhanViens",
                column: "MaQuan");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_IngredientId",
                table: "PurchaseOrderItems",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_TbPhieuNhapHangs_SupplierId",
                table: "TbPhieuNhapHangs",
                column: "SupplierId");

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
                name: "IX_TbQuanCafe_PhoneNumber",
                table: "TbQuanCafes",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TbQuyen_TenQuyen",
                table: "TbQuyens",
                column: "TenQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_TbSanPham_ProductName",
                table: "TbSanPhams",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_TbSanPhams_MaNhomSp",
                table: "TbSanPhams",
                column: "MaNhomSp");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKh_TenTaiKhoan",
                table: "TbTaiKhoanKhs",
                column: "TenTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKhs_CustomerId",
                table: "TbTaiKhoanKhs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoanKhs_RoleId",
                table: "TbTaiKhoanKhs",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoan_TenTaiKhoan",
                table: "TbTaiKhoans",
                column: "TenTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoans_MaNhanVien",
                table: "TbTaiKhoans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TbTaiKhoans_RoleId",
                table: "TbTaiKhoans",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbChiTietHoaDonBans");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "Admins");

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

