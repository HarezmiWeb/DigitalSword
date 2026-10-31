using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly HttpClient _httpClient;

        public MenuItemController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/MenuItems");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>();
                return View(list);
            }
            return View(new List<MenuItemResponseDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadParentMenus();
            return View(new CreateMenuItemDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMenuItemDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MenuItems", dto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await LoadParentMenus();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/MenuItems/{id}");
            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<UpdateMenuItemDto>();
                await LoadParentMenus(id);
                return View(item);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateMenuItemDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MenuItems/{dto.Id}", dto);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await LoadParentMenus(dto.Id);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _httpClient.DeleteAsync($"api/MenuItems/{id}");
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadParentMenus(int? currentId = null)
        {
            var response = await _httpClient.GetAsync("api/MenuItems");
            if (response.IsSuccessStatusCode)
            {
                var all = await response.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>();
                ViewBag.ParentMenus = all?.Where(m => m.ParentId == null && m.Id != currentId).ToList()
                                     ?? new List<MenuItemResponseDto>();
            }
        }
    }
}