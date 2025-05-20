using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbQuanCafe
    {
        [Key]
        public int MaQuan { get; set; }

        [StringLength(255)]
        public string TenQuan { get; set; }

        [StringLength(255)]
        public string DiaChi { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        public string Sdt { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }


        public ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();


        public ICollection<TbNhanVien> TbNhanViens { get; set; } = new List<TbNhanVien>();


        public ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();
    }
}
