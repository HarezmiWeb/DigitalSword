namespace DigitalSword.Application.DTOs;

public record UpdateCategoryDto(
    int Id,
    string Name,
    string? Description
);
