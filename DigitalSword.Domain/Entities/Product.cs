using DigitalSword.Domain.Common;
using DigitalSword.Domain.Enums;

namespace DigitalSword.Domain.Entities
{
    public class Product : BaseEntity
    {
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;
    }
}
