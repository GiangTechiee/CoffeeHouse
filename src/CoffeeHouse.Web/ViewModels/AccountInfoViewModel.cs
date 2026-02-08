using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.ViewModels
{
    public class AccountInfoViewModel
    {
        // Lo?i tài kho?n: "Admin" ho?c "User"
        public string Role { get; set; } = string.Empty;

        // Thông tin tài kho?n chung
        public string Username { get; set; } = string.Empty;

        // Thông tin chi ti?t: n?u admin, dây là thông tin nhân viên; n?u user, là thông tin khách hàng
        public Employee? EmployeeInfo { get; set; }
        public Customer? CustomerInfo { get; set; }
        public string? Password { get; internal set; }
    }
}
