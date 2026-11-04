using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminContactController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminContactController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }
        [HttpGet]
        public async Task<IActionResult> Messages()
        {
            var response = await _httpClient.GetAsync("api/ContactMessages");
            var messages = new List<ContactMessageDto>();

            if (response.IsSuccessStatusCode)
            {
                messages = await response.Content.ReadFromJsonAsync<List<ContactMessageDto>>() ?? new List<ContactMessageDto>();
            }

            return View(messages);
        }
        [HttpGet]
        public IActionResult Settings()
        {
            ViewBag.SupportEmail = "support@digitalsword.com";
            ViewBag.LocationAddress = "Başakşehir, İstanbul / Türkiye";
            return View();
        }

        [HttpPost]
        public IActionResult Settings(string supportEmail, string locationAddress)
        {
            TempData["SuccessMessage"] = "Kurumsal iletişim bilgileri güncellendi.";
            return RedirectToAction(nameof(Settings));
        }
    }
}
