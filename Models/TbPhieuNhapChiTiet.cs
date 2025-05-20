using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    [PrimaryKey("MaPhieuNhap", "MaNguyenLieu")]
    public partial class TbPhieuNhapChiTiet
    {
        [Key]
        public Guid MaPhieuNhap { get; set; }

        [Key]
        public int MaNguyenLieu { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal SoLuong { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(20, 4)")]
        public decimal? ThanhTien { get; set; }

        [ForeignKey("MaNguyenLieu")]
        public TbNguyenLieu MaNguyenLieuNavigation { get; set; }

        [ForeignKey("MaPhieuNhap")]
        public TbPhieuNhapHang MaPhieuNhapNavigation { get; set; }
    }
}
