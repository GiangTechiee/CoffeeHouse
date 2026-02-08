using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    public partial class PurchaseOrder : BaseEntity
    {
        [Key]
        public Guid PurchaseOrderId { get; set; }

        public int StoreId { get; set; }


        public DateTime OrderDate { get; set; }

        public int EmployeeId { get; set; }

        public int SupplierId { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        /// <summary>
        /// Total amount for this purchase order. This is an intentionally denormalized field for performance optimization.
        /// </summary>
        /// <remarks>
        /// <para><strong>Denormalization Rationale:</strong></para>
        /// <para>This field stores a cached aggregate value (sum of all PurchaseOrderItem.LineTotal values) to avoid expensive 
        /// JOIN operations when querying order totals. This is a common performance optimization for frequently accessed 
        /// aggregate data.</para>
        /// 
        /// <para><strong>Consistency Maintenance:</strong></para>
        /// <para>The application logic MUST maintain consistency between this field and the actual sum of line items. 
        /// Use the <see cref="CalculateTotalAmount"/> method to recalculate and update this value whenever order items change.</para>
        /// 
        /// <para><strong>Data Integrity:</strong></para>
        /// <para>While this creates data redundancy, it significantly improves query performance for order listing and reporting. 
        /// The trade-off is acceptable given the read-heavy nature of order queries.</para>
        /// </remarks>
        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [ForeignKey("StoreId")]
        public CafeStore Store { get; set; }


        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        /// <summary>
        /// Recalculates the TotalAmount from the sum of all line items.
        /// </summary>
        /// <remarks>
        /// <para>This method should be called whenever:</para>
        /// <list type="bullet">
        /// <item><description>A new PurchaseOrderItem is added to the order</description></item>
        /// <item><description>An existing PurchaseOrderItem is modified (quantity or price changes)</description></item>
        /// <item><description>A PurchaseOrderItem is removed from the order</description></item>
        /// <item><description>Data integrity validation is needed</description></item>
        /// </list>
        /// <para>This ensures the denormalized TotalAmount field remains consistent with the actual sum of line items.</para>
        /// </remarks>
        /// <returns>The calculated total amount</returns>
        public decimal CalculateTotalAmount()
        {
            TotalAmount = PurchaseOrderItems?.Sum(item => item.LineTotal) ?? 0;
            return TotalAmount;
        }
    }
}

