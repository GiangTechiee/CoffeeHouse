using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Models
{
    public partial class TbQuyen
    {
        [Key]
        public int MaQuyen { get; set; }

        [StringLength(255)]
        public string TenQuyen { get; set; }


        public ICollection<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; } = new List<TbTaiKhoanKh>();


        public ICollection<TbTaiKhoan> TbTaiKhoans { get; set; } = new List<TbTaiKhoan>();
    }
}
