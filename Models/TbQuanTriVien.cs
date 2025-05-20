using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{

    public partial class TbQuanTriVien
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string TenNguoiDung { get; set; }

        [StringLength(255)]
        public string MatKhauHash { get; set; }
    }
}
