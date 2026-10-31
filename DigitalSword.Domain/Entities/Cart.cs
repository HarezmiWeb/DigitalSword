using DigitalSword.Domain.Common;

namespace DigitalSword.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public required string AppUserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
