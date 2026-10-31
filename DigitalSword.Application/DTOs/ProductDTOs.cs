using DigitalSword.Domain.Enums;
namespace DigitalSword.Application.DTOs;

public record CreateProductDto(
    string Title,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    ProductType ProductType,
    int CategoryId,
    int PublisherId
);

public record UpdateProductDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    ProductType ProductType,
    int CategoryId,
    int PublisherId,
    bool IsActive
);

public record ProductResponseDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    string ImageUrl,
    ProductType ProductType,
    int CategoryId,
    int PublisherId,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateCategoryDto(
    string Name,
    string? Description
);

public record CreatePublisherDto(
    string Name,
    string? Description
);