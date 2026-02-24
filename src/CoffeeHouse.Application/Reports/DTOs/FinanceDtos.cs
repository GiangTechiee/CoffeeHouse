namespace CoffeeHouse.Application.Reports.DTOs;

public class FinanceSummaryResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<FinanceSummaryItemDto> Items { get; set; } = new();
}

public class FinanceSummaryItemDto
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public decimal TotalInvoice { get; set; }
    public decimal TotalSalary { get; set; }
    public decimal TotalImport { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal Profit { get; set; }
}

public class FinanceDetailResponse
{
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<EmployeeSalaryDto> Employees { get; set; } = new();
    public List<SalesOrderSummaryDto> Invoices { get; set; } = new();
    public List<PurchaseOrderSummaryDto> Purchases { get; set; } = new();
}

public class EmployeeSalaryDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal SalaryCoefficient { get; set; }
    public decimal TotalSalary { get; set; }
}

public class SalesOrderSummaryDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CustomerId { get; set; }
}

public class PurchaseOrderSummaryDto
{
    public Guid PurchaseOrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}
