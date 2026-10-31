using DigitalSword.Domain.Common;
using DigitalSword.Domain.Enums;
using DigitalSword.Domain.ValueObjects;

namespace DigitalSword.Domain.Entities
{
    public class Order : BaseEntity
    {
        public required string AppUserId { get; set; }
        public MoneyControl TotalPrice { get; set; } = MoneyControl.Zero();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
