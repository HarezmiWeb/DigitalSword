using DigitalSword.Application.DTOs;
using DigitalSword.Domain.Common;
using DigitalSword.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublishersController : ControllerBase
{
    private readonly AppDbContext _context;

    public PublishersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var publishers = await _context.Publishers
            .AsNoTracking()
            .Select(p => new PublisherResponseDto(p.Id, p.Name, p.Description))
            .ToListAsync();

        return Ok(publishers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
            return NotFound("Yayıncı bulunamadı.");

        return Ok(new PublisherResponseDto(publisher.Id, publisher.Name, publisher.Description));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePublisherDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Yayıncı adı boş olamaz.");

        var publisher = new DigitalSword.Domain.Entities.Publisher
        {
            Name = dto.Name,
            Description = dto.Description,
            Country = "Global",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        return Ok(new PublisherResponseDto(publisher.Id, publisher.Name, publisher.Description));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePublisherDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID uyuşmazlığı.");

        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
            return NotFound("Yayıncı bulunamadı.");

        publisher.Name = dto.Name;
        publisher.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
            return NotFound("Silinecek yayıncı bulunamadı.");

        // Güvenlik Kontrolü: Bu yayıncıya bağlı ürün var mı?
        bool hasProducts = await _context.Products.AnyAsync(p => p.PublisherId == id);
        if (hasProducts)
        {
            return BadRequest("Bu yayıncıya ait kayıtlı ürünler bulunmaktadır. Önce ürünlerin yayıncısını değiştirin.");
        }

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
