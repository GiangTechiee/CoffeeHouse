using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{

    public class ProductCategory
    {
        [Key]
        public int CategoryId { get; set; }


        [StringLength(255)]
        public string CategoryName { get; set; }


        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
