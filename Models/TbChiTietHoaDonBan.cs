using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    [PrimaryKey("MaHoaDon", "MaSanPham")]
    public class TbChiTietHoaDonBan
    {
        [Key]
        public Guid MaHoaDon { get; set; }

        [ForeignKey("MaHoaDon")]
        public TbHoaDonBan MaHoaDonNavigation { get; set; }


        [Key]
        public int MaSanPham { get; set; }

        [ForeignKey("MaSanPham")]
        public virtual TbSanPham MaSanPhamNavigation { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(15, 2)")]
        public decimal? ThanhTien { get; set; }

    }
}
