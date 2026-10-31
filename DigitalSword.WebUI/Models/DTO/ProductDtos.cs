using DigitalSword.Domain.Enums;

namespace DigitalSword.WebUI.Models.DTO;

public record ProductResponseDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    int ProductType,
    int CategoryId,
    int PublisherId,
    bool IsActive,
    DateTime CreatedAt
);

public class CreateProductDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public ProductType ProductType { get; set; } = ProductType.GameKey;
    public int CategoryId { get; set; } = 1;
    public int PublisherId { get; set; } = 1;
}
