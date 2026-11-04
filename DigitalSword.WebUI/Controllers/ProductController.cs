using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;
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
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(string? type = null)
    {
        var response = await _httpClient.GetAsync("Products");
        var products = new List<ProductResponseDto>();

        if (response.IsSuccessStatusCode)
        {
            products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>() ?? new List<ProductResponseDto>();
        }

        if (!string.IsNullOrEmpty(type))
        {
            products = products.Where(p =>
                p.ProductType != null && (
                    p.ProductType.ToString().Equals(type, StringComparison.OrdinalIgnoreCase) ||
                    (type.Equals("Game", StringComparison.OrdinalIgnoreCase) &&
                        (p.ProductType.ToString() == "1" || p.ProductType.ToString().Equals("GameKey", StringComparison.OrdinalIgnoreCase) || p.ProductType.ToString() == "4" || p.ProductType.ToString().Equals("DLC", StringComparison.OrdinalIgnoreCase))) ||
                    (type.Equals("Epin", StringComparison.OrdinalIgnoreCase) &&
                        (p.ProductType.ToString() == "2" || p.ProductType.ToString().Equals("E_Pin", StringComparison.OrdinalIgnoreCase) || p.ProductType.ToString() == "3" || p.ProductType.ToString().Equals("InGameItem", StringComparison.OrdinalIgnoreCase)))
                )
            ).ToList();

            ViewBag.CurrentType = type;
            ViewBag.PageTitle = type.ToLower() switch
            {
                "game" or "gamekey" => "Oyunlar",
                "epin" or "e_pin" => "E-Pin & Paketler",
                "ingameitem" => "Oyun İçi Ürünler",
                _ => "Tüm Ürünler"
            };
        }
        else
        {
            ViewBag.PageTitle = "Tüm Ürünler";
        }

        return View(products);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Epin(string? category = null)
    {
        var url = "Products?type=Epin";
        if (!string.IsNullOrWhiteSpace(category) && category.ToLower() != "tümü")
        {
            url += $"&category={category}";
        }

        var response = await _httpClient.GetAsync(url);
        var products = new List<ProductResponseDto>();

        if (response.IsSuccessStatusCode)
        {
            products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>() ?? new List<ProductResponseDto>();
        }

        ViewBag.SelectedCategory = category ?? "Tümü";
        return View(products);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create(string? type = null)
    {
        await LoadCategoriesAndPublishersAsync();
        ViewBag.DefaultType = type ?? "Game";
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto, string? type = null)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndPublishersAsync();
            return View(dto);
        }

        var response = await _httpClient.PostAsJsonAsync("Products", dto);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index), new { type = dto.ProductType.ToString() });
        }

        var errorDetails = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"API Hatası ({response.StatusCode}): {errorDetails}");

        await LoadCategoriesAndPublishersAsync();
        return View(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, string? returnType = null)
    {
        var response = await _httpClient.DeleteAsync($"Products/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index), new { type = returnType });
        }

        TempData["ErrorMessage"] = $"Ürün silinemedi. Hata kodu: {response.StatusCode}";
        return RedirectToAction(nameof(Index), new { type = returnType });
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndPublishersAsync();
            return View(dto);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Products/{dto.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), new { type = dto.ProductType });
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