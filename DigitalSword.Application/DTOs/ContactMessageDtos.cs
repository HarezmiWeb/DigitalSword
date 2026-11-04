namespace DigitalSword.Application.DTOs;

public class ContactMessageDto
{
    public int Id { get; set; }
    private string _senderName = string.Empty;
    public string SenderName
    {
        get => _senderName;
        set => _senderName = value;
    }
    public string FullName
    {
        get => _senderName;
        set => _senderName = value;
    }
    private string _senderEmail = string.Empty;
    public string SenderEmail
    {
        get => _senderEmail;
        set => _senderEmail = value;
    }
    public string Email
    {
        get => _senderEmail;
        set => _senderEmail = value;
    }

    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    private DateTime _sentDate = DateTime.UtcNow;
    public DateTime SentDate
    {
        get => _sentDate;
        set => _sentDate = value;
    }
    public DateTime CreatedAt
    {
        get => _sentDate;
        set => _sentDate = value;
    }

    public bool IsRead { get; set; } = false;
}