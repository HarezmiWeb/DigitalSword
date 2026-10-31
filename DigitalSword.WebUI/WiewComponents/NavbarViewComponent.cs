using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.ViewComponents
{
    [ViewComponent(Name = "NavbarMenu")]
    public class NavbarViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;
        public NavbarViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/MenuItems/tree");

                if (response.IsSuccessStatusCode)
                {
                    var menus = await response.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>();
                    return View(menus ?? new List<MenuItemResponseDto>());
                }
            }
            catch
            {
            }

            return View(new List<MenuItemResponseDto>());
        }
    }
}