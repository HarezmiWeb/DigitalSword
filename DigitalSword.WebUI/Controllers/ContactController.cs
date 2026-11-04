using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

public class ContactController : Controller
{
    private readonly HttpClient _httpClient;

    public ContactController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.SupportEmail = "support@digitalsword.com";
        ViewBag.LocationAddress = "Başakşehir, İstanbul / Türkiye";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(ContactMessageDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Lütfen alanları eksiksiz doldurun.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/ContactMessages", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesajınız başarıyla iletildi.";
            }
            else
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Durum Kodu: {(int)response.StatusCode} ({response.StatusCode}) - Detay: {errorDetail}";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"API Bağlantı Hatası: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

}