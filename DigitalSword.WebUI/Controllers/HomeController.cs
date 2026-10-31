using DigitalSword.WebUI.Models;
using DigitalSword.WebUI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DigitalSword.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index(string? search = null)
        {
            var response = await _httpClient.GetAsync("https://localhost:7222/api/Products");

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();

                if (!string.IsNullOrEmpty(search) && products != null)
                {
                    products = products
                        .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return View(products);
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"API Hatası: {response.StatusCode} - {errorContent}");

            return View(new List<ProductResponseDto>());
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7222/api/Products/{id}");

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
