using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    public partial class TbNguyenLieu
    {
        [Key]
        public int MaNguyenLieu { get; set; }

        [StringLength(255)]
        public string TenNguyenLieu { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal SoLuong { get; set; }

        [StringLength(50)]
        public string DonViTinh { get; set; }

        public DateTime? HanSuDung { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal SoLuongToiThieu { get; set; }


        public ICollection<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; } = new List<TbPhieuNhapChiTiet>();
    }
}
