using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers
{
    public class SupportController : Controller
    {
        private readonly HttpClient _httpClient;

        public SupportController(IHttpClientFactory httpClientFactory)
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

            ViewBag.SupportEmail = "support@digitalsword.com";
            ViewBag.LocationAddress = "Başakşehir, İstanbul / Türkiye";

            return View(teamMembers);
        }
    }
}
