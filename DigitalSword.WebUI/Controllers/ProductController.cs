using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly HttpClient _httpClient;

    public ProductController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }

    private async Task LoadCategoriesAndPublishersAsync()
    {
        var categoryResponse = await _httpClient.GetAsync("Categories");
        if (categoryResponse.IsSuccessStatusCode)
        {
            var categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryResponseDto>>();
            ViewBag.Categories = categories ?? new List<CategoryResponseDto>();
        }
        else
        {
            ViewBag.Categories = new List<CategoryResponseDto>();
        }

        var publisherResponse = await _httpClient.GetAsync("Publishers");
        if (publisherResponse.IsSuccessStatusCode)
        {
            var publishers = await publisherResponse.Content.ReadFromJsonAsync<List<PublisherResponseDto>>();
            ViewBag.Publishers = publishers ?? new List<PublisherResponseDto>();
        }
        else
        {
            ViewBag.Publishers = new List<PublisherResponseDto>();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("Products");
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();
            return View(products);
        }

        return View(new List<ProductResponseDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAndPublishersAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndPublishersAsync();
            return View(dto);
        }

        var response = await _httpClient.PostAsJsonAsync("Products", dto);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        var errorDetails = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"API Hatası ({response.StatusCode}): {errorDetails}");

        await LoadCategoriesAndPublishersAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"Products/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = $"Ürün silinemedi. Hata kodu: {response.StatusCode}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _httpClient.GetAsync($"Products/{id}");
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        if (product == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateProductDto(
            product.Id,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.ImageUrl ?? "",
            product.ProductType,
            product.CategoryId,
            product.PublisherId,
            product.IsActive
        );

        await LoadCategoriesAndPublishersAsync();
        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndPublishersAsync();
            return View(dto);
        }

        try
        {
            // Rota: "Products/{dto.Id}" (BaseAddress'e göre)
            var response = await _httpClient.PutAsJsonAsync($"Products/{dto.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"API Hatası ({response.StatusCode}): {errorDetails}");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Bağlantı Hatası: {ex.Message}");
        }

        await LoadCategoriesAndPublishersAsync();
        return View(dto);
    }
}