using DigitalSword.Domain.Common;

namespace DigitalSword.Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
