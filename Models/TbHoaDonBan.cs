using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    public class TbHoaDonBan
    {
        [Key]
        public Guid MaHoaDon { get; set; }

        public int MaQuan { get; set; }

        [ForeignKey("MaQuan")]
        public TbQuanCafe MaQuanNavigation { get; set; }


        public DateTime NgayLap { get; set; }

        public int? MaNhanVien { get; set; }

        [ForeignKey("MaNhanVien")]
        public TbNhanVien MaNhanVienNavigation { get; set; }


        public int MaKhachHang { get; set; }

        [ForeignKey("MaKhachHang")]
        public TbKhachHang? MaKhachHangNavigation { get; set; }


        [StringLength(50)]
        public string HinhThucThanhToan { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TongTien { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = null!;

        public virtual ICollection<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; } = new List<TbChiTietHoaDonBan>();
    }
}
