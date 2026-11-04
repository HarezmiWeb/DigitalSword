using DigitalSword.Application.DTOs;
using DigitalSword.WebUI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ProductResponseDto = DigitalSword.Application.DTOs.ProductResponseDto;

namespace DigitalSword.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        // Projenizdeki HttpClientFactory mimarisiyle uyumlu DI
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null)
        {
            // 1. Kategorileri Çek (BaseAddress üzerinden)
            var catResponse = await _httpClient.GetAsync("Categories");
            var categories = new List<CategoryResponseDto>();

            if (catResponse.IsSuccessStatusCode)
            {
                categories = await catResponse.Content.ReadFromJsonAsync<List<CategoryResponseDto>>()
                             ?? new List<CategoryResponseDto>();
            }

            // 2. Ürünleri Çek (BaseAddress üzerinden)
            var prodResponse = await _httpClient.GetAsync("Products");
            var products = new List<ProductResponseDto>();

            if (prodResponse.IsSuccessStatusCode)
            {
                products = await prodResponse.Content.ReadFromJsonAsync<List<ProductResponseDto>>()
                           ?? new List<ProductResponseDto>();
            }
            else
            {
                // Eğer istek başarısız olursa sebebi konsola veya debug ekranına düşsün
                var error = await prodResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası ({prodResponse.StatusCode}): {error}");
            }

            // 3. Arama Filtresi
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products
                    .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ViewBag.CurrentSearch = search;
            }

            ViewBag.Categories = categories;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<DigitalSword.WebUI.Models.DTO.ProductResponseDto>();
                if (product != null)
                {
                    return View(product);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}