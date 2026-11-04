using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Categories");
            var categories = new List<CategoryResponseDto>();

            if (response.IsSuccessStatusCode)
            {
                categories = await response.Content.ReadFromJsonAsync<List<CategoryResponseDto>>()
                              ?? new List<CategoryResponseDto>();
            }
            else
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                ViewBag.ApiError = $"API Hatası ({response.StatusCode}): {response.RequestMessage?.RequestUri} -> {errorDetails}";
            }

            return View(categories);
        }

        // Admin Paneli Kategori Ekleme Sayfası (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // Admin Paneli Kategori Ekleme İşlemi (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _httpClient.PostAsJsonAsync("Categories", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Kategori eklenirken bir hata oluştu.");
            return View(dto);
        }

        // Admin Paneli Kategori Düzenleme Sayfası (GET) -> /Category/Edit/{id}
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
            {
                return NotFound();
            }

            // Constructor parametreleri ile nesne oluşturma:
            var updateDto = new UpdateCategoryDto(
                category.Id,
                category.Name,
                category.Description
            );

            return View(updateDto);
        }

        // Admin Paneli Kategori Düzenleme İşlemi (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _httpClient.PutAsJsonAsync($"Categories/{dto.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var errorDetails = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Kategori güncellenirken bir hata oluştu: {errorDetails}");
            return View(dto);
        }

        // Admin Paneli Kategori Silme İşlemi (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"Categories/{id}");
            return RedirectToAction(nameof(Index));
        }

        // Vitrin Tarafı Slug Tabanlı Kategori Detay / Filtreleme
        // Vitrin Tarafı Slug Tabanlı Kategori Detay / Filtreleme
        [HttpGet("kategori/{slug}")]
        public async Task<IActionResult> Detail(string slug, int? subId = null)
        {
            // 1. Kategorileri Getir
            var catResponse = await _httpClient.GetAsync("Categories");
            var allCategories = new List<CategoryResponseDto>();

            if (catResponse.IsSuccessStatusCode)
            {
                allCategories = await catResponse.Content.ReadFromJsonAsync<List<CategoryResponseDto>>()
                                ?? new List<CategoryResponseDto>();
            }

            string Normalize(string text) =>
                (text ?? "").Trim().ToLower()
                    .Replace(" ", "-")
                    .Replace("ı", "i")
                    .Replace("ğ", "g")
                    .Replace("ü", "u")
                    .Replace("ş", "s")
                    .Replace("ö", "o")
                    .Replace("ç", "c");

            string targetSlug = Normalize(slug);

            // 2. Ürünleri Getir
            var prodResponse = await _httpClient.GetAsync("Products");
            var allProducts = new List<ProductResponseDto>();

            if (prodResponse.IsSuccessStatusCode)
            {
                allProducts = await prodResponse.Content.ReadFromJsonAsync<List<ProductResponseDto>>()
                              ?? new List<ProductResponseDto>();
            }

            // 3. Ana Bölüm Filtresi (E-Pin, Oyunlar, Hediye Kartları)
            List<ProductResponseDto> sectionProducts;
            string pageTitle = "Katalog";

            if (targetSlug.Contains("epin") || targetSlug.Contains("oyun-ici"))
            {
                pageTitle = "EPIN & OYUN İÇİ ÜRÜNLER";
                // Sadece E-Pin (2) ve Oyun İçi Ürün (3) olanları al
                sectionProducts = allProducts.Where(p =>
                    p.ProductType != null && (
                        p.ProductType.ToString().Equals("Epin", StringComparison.OrdinalIgnoreCase) ||
                        p.ProductType.ToString() == "2" ||
                        p.ProductType.ToString().Equals("InGameItem", StringComparison.OrdinalIgnoreCase) ||
                        p.ProductType.ToString() == "3"
                    )
                ).ToList();
            }
            else if (targetSlug.Contains("hediye") || targetSlug.Contains("gift"))
            {
                pageTitle = "HEDİYE KARTLARI";
                // Hediye kartlarını al
                sectionProducts = allProducts.Where(p =>
                    p.Title.Contains("Kart", StringComparison.OrdinalIgnoreCase) ||
                    p.Title.Contains("Google Play", StringComparison.OrdinalIgnoreCase) ||
                    p.Title.Contains("Steam Cüzdan", StringComparison.OrdinalIgnoreCase) ||
                    p.Title.Contains("Gift", StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            else if (targetSlug.Contains("oyun") || targetSlug.Contains("game"))
            {
                pageTitle = "DİJİTAL OYUNLAR";
                // Sadece Oyun (1) ve DLC (4) olanları al
                sectionProducts = allProducts.Where(p =>
                    p.ProductType != null && (
                        p.ProductType.ToString().Equals("Game", StringComparison.OrdinalIgnoreCase) ||
                        p.ProductType.ToString().Equals("GameKey", StringComparison.OrdinalIgnoreCase) ||
                        p.ProductType.ToString() == "1" ||
                        p.ProductType.ToString() == "4" ||
                        p.ProductType.ToString().Equals("DLC", StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();
            }
            else
            {
                // Doğrudan veritabanındaki tek bir kategori ismiyle gelinmişse
                var matchedCategory = allCategories.FirstOrDefault(c => Normalize(c.Name) == targetSlug);
                if (matchedCategory != null)
                {
                    pageTitle = matchedCategory.Name;
                    sectionProducts = allProducts.Where(p => p.CategoryId == matchedCategory.Id).ToList();
                }
                else
                {
                    sectionProducts = allProducts;
                }
            }

            // 4. Hap Butonları (Pills) Filtrele:
            // Sadece bu sayfadaki ürünlerin ait olduğu kategorileri buton olarak göster
            var relevantCategoryIds = sectionProducts.Select(p => p.CategoryId).Distinct().ToList();
            var relevantCategories = allCategories.Where(c => relevantCategoryIds.Contains(c.Id)).ToList();

            // 5. Kullanıcı üstteki butonlardan bir alt kategori seçtiyse (subId filtresi)
            List<ProductResponseDto> finalProducts = sectionProducts;
            if (subId.HasValue && subId.Value > 0)
            {
                finalProducts = sectionProducts.Where(p => p.CategoryId == subId.Value).ToList();
            }

            ViewBag.CategoryTitle = pageTitle;
            ViewBag.CurrentSlug = slug;
            ViewBag.ActiveSubId = subId;
            ViewBag.SubCategories = relevantCategories;

            return View(finalProducts);
        }
    }
}