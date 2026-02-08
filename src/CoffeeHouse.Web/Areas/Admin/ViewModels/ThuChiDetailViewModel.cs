

using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.ViewModels
{
    public class ThuChiDetailViewModel
    {
        // Thông tin l?c/chung
        public int CafeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Danh sách chi ti?t các b?ng liên quan
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<SalesOrder> Invoices { get; set; }
        public IEnumerable<PurchaseOrder> PhieuNhapHangs { get; set; }
    }
}

