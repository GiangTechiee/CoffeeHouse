using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbKhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [StringLength(255)]
        public string TenKhachHang { get; set; }


        [StringLength(10)]
        [Unicode(false)]
        public string SdtkhachHang { get; set; }

        [StringLength(255)]
        public string DiaChi { get; set; }


        public ICollection<TbGioHang> TbGioHangs { get; set; } = new List<TbGioHang>();


        public ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();


        public ICollection<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; } = new List<TbTaiKhoanKh>();
    }
}
