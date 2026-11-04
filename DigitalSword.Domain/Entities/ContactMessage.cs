using DigitalSword.Domain.Common;

namespace DigitalSword.Domain.Entities;

public class ContactMessage : BaseEntity
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; } = false;
}