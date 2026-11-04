using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSupportController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminSupportController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("SupportTeam");
            var teamMembers = new List<SupportTeamMemberDto>();

            if (response.IsSuccessStatusCode)
            {
                teamMembers = await response.Content.ReadFromJsonAsync<List<SupportTeamMemberDto>>() ?? new List<SupportTeamMemberDto>();
            }

            return View(teamMembers);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"SupportTeam/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var member = await response.Content.ReadFromJsonAsync<SupportTeamMemberDto>();
            if (member == null) return NotFound();

            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SupportTeamMemberDto dto)
        {
            if (id != dto.Id) return BadRequest();

            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PutAsJsonAsync($"SupportTeam/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Güncelleme yapılırken bir hata oluştu.");
            return View(dto);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SupportTeamMemberDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var response = await _httpClient.PostAsJsonAsync("SupportTeam", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Hatası: {response.StatusCode} - {errorContent}");

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _httpClient.DeleteAsync($"SupportTeam/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}