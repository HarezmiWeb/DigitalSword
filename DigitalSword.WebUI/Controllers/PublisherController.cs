using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class PublisherController : Controller
{
    private readonly HttpClient _httpClient;

    public PublisherController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("Publishers");
        if (response.IsSuccessStatusCode)
        {
            var publishers = await response.Content.ReadFromJsonAsync<List<PublisherResponseDto>>();
            return View(publishers ?? new List<PublisherResponseDto>());
        }

        return View(new List<PublisherResponseDto>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePublisherDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PostAsJsonAsync("Publishers", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Yayıncı eklenirken hata oluştu.");
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _httpClient.GetAsync($"Publishers/{id}");
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Yayıncı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var publisher = await response.Content.ReadFromJsonAsync<PublisherResponseDto>();
        if (publisher == null)
            return NotFound();

        var updateDto = new UpdatePublisherDto(publisher.Id, publisher.Name, publisher.Description);
        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdatePublisherDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PutAsJsonAsync($"Publishers/{dto.Id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Yayıncı güncellenirken hata oluştu.");
        return View(dto);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"Publishers/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(errorMessage)
                ? "Yayıncı silinirken bir hata oluştu."
                : errorMessage;
        }

        return RedirectToAction(nameof(Index));
    }
}