using DigitalSword.Domain.Common;
using DigitalSword.Domain.Enums;

namespace DigitalSword.Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public CategoryType Type { get; set; } = CategoryType.Genre;
    public int? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}