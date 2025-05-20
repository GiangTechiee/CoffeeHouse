using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{

    public partial class TbNhanVien
    {
        [Key]
        public int MaNhanVien { get; set; }

        public int MaQuan { get; set; }

        [StringLength(255)]
        [Required]
        public string HoTen { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public DateTime? NgaySinh { get; set; }


        public bool? GioiTinh { get; set; }

        [StringLength(50)]
        public string ChucVu { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        public string Sdt { get; set; }


        [StringLength(50)]
        [Unicode(false)]
        public string SoCccd { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal LuongCoBan { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal HeSoLuong { get; set; }

        [ForeignKey("MaQuan")]
        public TbQuanCafe? MaQuanNavigation { get; set; }


        public ICollection<TbHoaDonBan> TbHoaDonBans { get; set; } = new List<TbHoaDonBan>();


        public ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();


        public ICollection<TbTaiKhoan> TbTaiKhoans { get; set; } = new List<TbTaiKhoan>();
    }
}
