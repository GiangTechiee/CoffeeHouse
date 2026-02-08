using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Areas.Admin.ViewModels
{
    public class EmployeeAccountViewModel
    {
        public Employee Employee { get; set; } = new Employee();
        public Account Account { get; set; } = new Account();
    }
}

