using DigitalSword.Domain.Entities;
using DigitalSword.Application.DTOs;
using DigitalSword.Persistence.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupportTeamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupportTeamController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var members = await _context.SupportTeamMembers
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SupportTeamMemberDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    Email = x.Email,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync();

            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var member = await _context.SupportTeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            return Ok(new SupportTeamMemberDto
            {
                Id = member.Id,
                Name = member.Name,
                Title = member.Title,
                Email = member.Email,
                ImageUrl = member.ImageUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupportTeamMemberDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new SupportTeamMember
            {
                Name = dto.Name,
                Title = dto.Title,
                Email = dto.Email,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _context.SupportTeamMembers.AddAsync(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ekip üyesi kaydedildi.", id = entity.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SupportTeamMemberDto dto)
        {
            var member = await _context.SupportTeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            member.Name = dto.Name;
            member.Title = dto.Title;
            member.Email = dto.Email;
            member.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Ekip üyesi güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.SupportTeamMembers.FindAsync(id);
            if (member == null) return NotFound();

            _context.SupportTeamMembers.Remove(member);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}