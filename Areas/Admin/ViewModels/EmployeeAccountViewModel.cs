using CoffeeHouse.Models;

namespace CoffeeHouse.Areas.Admin.ViewModels
{
    public class EmployeeAccountViewModel
    {
        public TbNhanVien Employee { get; set; } = new TbNhanVien();
        public TbTaiKhoan Account { get; set; } = new TbTaiKhoan();
    }
}
