using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly HttpClient _httpClient;

    public CategoryController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("Categories");
        if (response.IsSuccessStatusCode)
        {
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponseDto>>();
            return View(categories ?? new List<CategoryResponseDto>());
        }

        return View(new List<CategoryResponseDto>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PostAsJsonAsync("Categories", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Kategori eklenirken hata oluştu.");
        return View(dto);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _httpClient.GetAsync($"Categories/{id}");
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var category = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        if (category == null)
            return NotFound();

        var updateDto = new UpdateCategoryDto(category.Id, category.Name, category.Description);
        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PutAsJsonAsync($"Categories/{dto.Id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Kategori güncellenirken hata oluştu.");
        return View(dto);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"Categories/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(errorMessage)
                ? "Kategori silinirken bir hata oluştu."
                : errorMessage;
        }

        return RedirectToAction(nameof(Index));
    }



}
