using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{
    public class TbTinTuc
    {
        [Key]
        public int MaTinTuc { get; set; }

        [StringLength(255)]
        public string TieuDe { get; set; } = null!;

        public DateTime NgayDang { get; set; }

        public string NoiDung { get; set; } = null!;

        [StringLength(255)]
        public string? HinhAnh { get; set; }
    }
}
