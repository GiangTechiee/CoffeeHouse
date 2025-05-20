using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbSanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [StringLength(255)]
        public string TenSanPham { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal GiaBan { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public string? GhiChu { get; set; }

        public int MaNhomSp { get; set; }

        [ForeignKey("MaNhomSp")]
        public TbNhomSanPham? MaNhomSpNavigation { get; set; }


        public ICollection<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; } = new List<TbChiTietHoaDonBan>();


        public ICollection<TbGioHang> TbGioHangs { get; set; } = new List<TbGioHang>();
    }
}
