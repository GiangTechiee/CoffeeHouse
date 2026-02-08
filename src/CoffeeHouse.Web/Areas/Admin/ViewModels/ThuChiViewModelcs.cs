using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.ViewModels
{
    public class ThuChiItemViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal TotalInvoice { get; set; }     // T?ng ti?n t? hóa don bán c?a quán
        public decimal TotalSalary { get; set; }        // Ti?n luong nhân viên c?a quán
        public decimal TotalRevenue { get; set; }       // T?ng Thu = TotalInvoice + TotalSalary
        public decimal TotalImport { get; set; }        // T?ng ti?n phi?u nh?p c?a quán
        public decimal Profit { get; set; }             // Lãi = TotalRevenue - TotalImport
    }

    public class AllThuChiViewModel
    {
        // Thông tin l?c chung
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        // Danh sách d? li?u thu chi cho t?ng quán
        public List<ThuChiItemViewModel> Items { get; set; } = new List<ThuChiItemViewModel>();
    }
}

