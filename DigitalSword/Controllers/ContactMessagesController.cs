using DigitalSword.Application.DTOs;
using DigitalSword.Domain.Entities;
using DigitalSword.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactMessagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContactMessagesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContactMessageDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contactMessage = new ContactMessage
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Subject = dto.Subject ?? "Genel İletişim",
            Message = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _context.ContactMessages.AddAsync(contactMessage);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Mesaj başarıyla kaydedildi." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var messages = await _context.ContactMessages
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return Ok(messages);
    }
}