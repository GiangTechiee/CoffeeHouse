using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Models
{

    public class TbNhomSanPham
    {
        [Key]
        public int MaNhomSp { get; set; }


        [StringLength(255)]
        public string TenNhomSp { get; set; }


        public ICollection<TbSanPham> TbSanPhams { get; set; } = new List<TbSanPham>();
    }
}
