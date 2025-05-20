using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public class TbTaiKhoanKh
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [StringLength(255)]
        [Required]
        public string TenTaiKhoan { get; set; }

        [StringLength(255)]
        [Required]
        public string MatKhauHash { get; set; }

        public int MaKhachHang { get; set; }

        [ForeignKey("MaKhachHang")]
        public TbKhachHang MaKhachHangNavigation { get; set; }


        public int MaQuyen { get; set; }

        [ForeignKey("MaQuyen")]
        public TbQuyen MaQuyenNavigation { get; set; }
    }
}
