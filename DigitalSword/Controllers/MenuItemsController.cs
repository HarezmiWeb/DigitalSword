using DigitalSword.Application.DTOs;
using DigitalSword.Domain.Entities;
using DigitalSword.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuItemsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("tree")]
        public async Task<IActionResult> GetMenuTree()
        {
            var items = await _context.MenuItems
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .ToListAsync();

            var tree = items
                .Where(m => m.ParentId == null)
                .Select(m => new MenuItemResponseDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Url = m.Url,
                    Icon = m.Icon,
                    Order = m.Order,
                    SubMenus = items
                        .Where(s => s.ParentId == m.Id)
                        .OrderBy(s => s.Order)
                        .Select(s => new MenuItemResponseDto
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Url = s.Url,
                            Icon = s.Icon,
                            Order = s.Order,
                            ParentId = s.ParentId
                        }).ToList()
                }).ToList();

            return Ok(tree);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.MenuItems
                .Include(m => m.Parent)
                .OrderBy(m => m.ParentId)
                .ThenBy(m => m.Order)
                .Select(m => new MenuItemResponseDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Url = m.Url,
                    Icon = m.Icon,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    ParentTitle = m.Parent != null ? m.Parent.Title : "-",
                    IsActive = m.IsActive
                }).ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var m = await _context.MenuItems.FindAsync(id);
            if (m == null) return NotFound();

            return Ok(new MenuItemResponseDto
            {
                Id = m.Id,
                Title = m.Title,
                Url = m.Url,
                Icon = m.Icon,
                Order = m.Order,
                ParentId = m.ParentId,
                IsActive = m.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
        {
            var item = new MenuItem
            {
                Title = dto.Title,
                Url = dto.Url,
                Icon = dto.Icon,
                Order = dto.Order,
                ParentId = dto.ParentId,
                IsActive = dto.IsActive
            };

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Title = dto.Title;
            item.Url = dto.Url;
            item.Icon = dto.Icon;
            item.Order = dto.Order;
            item.ParentId = dto.ParentId;
            item.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();
            var subItems = await _context.MenuItems.Where(m => m.ParentId == id).ToListAsync();
            _context.MenuItems.RemoveRange(subItems);

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
