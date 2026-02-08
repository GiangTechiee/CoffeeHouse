using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.ViewModels
{
    public class PhieuNhapViewModel
    {
        [Display(Name = "Ngày nh?p")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Nhân viên")]
        public int EmployeeId { get; set; }

        [Display(Name = "Quán")]
        public int StoreId { get; set; }

        [Display(Name = "Nhà cung c?p")]
        public int SupplierId { get; set; }

        public List<PhieuNhapChiTietViewModel> ChiTietNhap { get; set; } = new List<PhieuNhapChiTietViewModel>();
    }

    public class PhieuNhapChiTietViewModel
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;

        [Display(Name = "S? lu?ng nh?p")]
        public decimal QuantityNhap { get; set; }

        [Display(Name = "Ðon giá nh?p")]
        public decimal UnitPriceNhap { get; set; }
    }
}


