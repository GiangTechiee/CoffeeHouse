using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbTaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [StringLength(255)]
        public string TenTaiKhoan { get; set; }

        [StringLength(255)]
        public string MatKhauHash { get; set; }

        public int MaNhanVien { get; set; }

        public int MaQuyen { get; set; }

        [ForeignKey("MaNhanVien")]
        public TbNhanVien? MaNhanVienNavigation { get; set; }

        [ForeignKey("MaQuyen")]
        public TbQuyen? MaQuyenNavigation { get; set; }
    }
}
