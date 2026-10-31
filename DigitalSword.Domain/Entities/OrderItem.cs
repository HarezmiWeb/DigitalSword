using DigitalSword.Domain.Common;
using DigitalSword.Domain.Enums;
using DigitalSword.Domain.ValueObjects;

namespace DigitalSword.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public MoneyControl Price { get; set; } = MoneyControl.Zero();
        public int Quantity { get; private set; } = 1;
        public string? ActivationKey { get; set; }
    }
}