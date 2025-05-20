using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    [PrimaryKey("MaKhachHang", "MaSanPham")]
    public partial class TbGioHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [ForeignKey("MaKhachHang")]
        public TbKhachHang MaKhachHangNavigation { get; set; }


        [Key]
        public int MaSanPham { get; set; }

        [ForeignKey("MaSanPham")]
        public TbSanPham MaSanPhamNavigation { get; set; }


        public int SoLuong { get; set; }

    }
}
