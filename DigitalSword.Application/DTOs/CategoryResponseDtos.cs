namespace DigitalSword.Application.DTOs;

public record CategoryResponseDto(
    int Id,
    string Name,
    string? Description
);
