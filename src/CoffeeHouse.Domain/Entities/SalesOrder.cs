using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    public class SalesOrder : BaseEntity
    {
        [Key]
        public Guid OrderId { get; set; }

        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public CafeStore Store { get; set; }


        public DateTime OrderDate { get; set; }

        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }


        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }


        [StringLength(50)]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Total amount for this sales order. This is an intentionally denormalized field for performance optimization.
        /// </summary>
        /// <remarks>
        /// <para><strong>Denormalization Rationale:</strong></para>
        /// <para>This field stores a cached aggregate value (sum of all SalesOrderItem.LineTotal values) to avoid expensive 
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

        public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();

        /// <summary>
        /// Adds an item to the order
        /// </summary>
        public void AddItem(int productId, int quantity, decimal unitPrice)
        {
            if (Status != "Pending")
            {
                throw new InvalidOperationException("Cannot add items to an order that is not pending");
            }

            var existingItem = SalesOrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                SalesOrderItems.Add(new SalesOrderItem
                {
                    OrderId = this.OrderId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            CalculateTotalAmount();
        }

        /// <summary>
        /// Removes an item from the order
        /// </summary>
        public void RemoveItem(int productId)
        {
            if (Status != "Pending")
            {
                throw new InvalidOperationException("Cannot remove items from an order that is not pending");
            }

            var item = SalesOrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                SalesOrderItems.Remove(item);
                CalculateTotalAmount();
            }
        }

        /// <summary>
        /// Recalculates the TotalAmount from the sum of all line items.
        /// </summary>
        public decimal CalculateTotalAmount()
        {
            TotalAmount = SalesOrderItems?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
            return TotalAmount;
        }

        public void Confirm()
        {
            if (Status != "Pending") throw new InvalidOperationException("Only pending orders can be confirmed");
            if (!SalesOrderItems.Any()) throw new InvalidOperationException("Cannot confirm an order with no items");
            Status = "Confirmed";
        }

        public void Process()
        {
            if (Status != "Confirmed" && Status != "Pending") 
                throw new InvalidOperationException("Only confirmed or pending orders can be processed");
            Status = "Processing";
        }

        public void Complete()
        {
            if (Status != "Processing" && Status != "Confirmed") 
                throw new InvalidOperationException("Only processing or confirmed orders can be completed");
            Status = "Completed";
        }

        public void Cancel()
        {
            if (Status == "Completed" || Status == "Refunded") 
                throw new InvalidOperationException("Cannot cancel a completed or refunded order");
            Status = "Cancelled";
        }

        public void Refund()
        {
            if (Status != "Completed") 
                throw new InvalidOperationException("Only completed orders can be refunded");
            Status = "Refunded";
        }
    }
}
