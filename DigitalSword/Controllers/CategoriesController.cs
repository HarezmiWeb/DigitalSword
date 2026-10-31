using DigitalSword.Application.DTOs;
using DigitalSword.Domain.Common;
using DigitalSword.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryResponseDto(
                c.Id,
                c.Name,
                c.Description
            ))
            .ToListAsync();

        return Ok(categories);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Kategori bulunamadı.");

        return Ok(new CategoryResponseDto(category.Id, category.Name, category.Description));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Kategori adı boş olamaz.");

        var category = new DigitalSword.Domain.Entities.Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(new CategoryResponseDto(category.Id, category.Name, category.Description));
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID uyuşmazlığı.");

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Kategori bulunamadı.");

        category.Name = dto.Name;
        category.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Silinecek kategori bulunamadı.");

        // Güvenlik Kontrolü: Bu kategoriye bağlı ürün var mı?
        bool hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
        {
            return BadRequest("Bu kategoriye ait ürünler bulunmaktadır. Önce ürünlerin kategorisini değiştirin veya ürünleri silin.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}
