using DigitalSword.Domain.Common;
namespace DigitalSword.Domain.Entities
{
    public class Publisher : BaseEntity
    {
        public required string Name { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
