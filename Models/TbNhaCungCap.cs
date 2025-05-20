using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbNhaCungCap
    {
        [Key]
        public int MaNhaCungCap { get; set; }

        [StringLength(255)]
        [Required]
        public string TenNhaCungCap { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        public string Sdtncc { get; set; }


        [StringLength(50)]
        [Unicode(false)]
        public string? Stk { get; set; }


        public ICollection<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; } = new List<TbPhieuNhapHang>();
    }
}
