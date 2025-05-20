using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    public partial class TbPhieuNhapHang
    {
        [Key]
        public Guid MaPhieuNhap { get; set; }

        public int MaQuan { get; set; }


        public DateTime NgayLap { get; set; }

        public int MaNhanVien { get; set; }

        public int MaNhaCungCap { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaNhaCungCap")]
        public TbNhaCungCap MaNhaCungCapNavigation { get; set; }

        [ForeignKey("MaNhanVien")]
        public TbNhanVien MaNhanVienNavigation { get; set; }

        [ForeignKey("MaQuan")]
        public TbQuanCafe MaQuanNavigation { get; set; }


        public ICollection<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; } = new List<TbPhieuNhapChiTiet>();
    }
}
