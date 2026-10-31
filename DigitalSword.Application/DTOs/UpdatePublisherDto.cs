namespace DigitalSword.Application.DTOs;

public record UpdatePublisherDto(
    int Id,
    string Name,
    string? Description
);
