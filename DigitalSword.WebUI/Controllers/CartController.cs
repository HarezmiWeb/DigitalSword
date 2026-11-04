using DigitalSword.Application.DTOs;
using DigitalSword.WebUI.Extensions;
using DigitalSword.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

public class CartController : Controller
{
    private readonly HttpClient _httpClient;
    private const string CartSessionKey = "DigitalSword_Cart";

    public CartController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }
    [HttpGet]
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>(CartSessionKey)
                   ?? new List<CartItemViewModel>();
        return View(cart);
    }
    [HttpPost]
    public async Task<IActionResult> AddToCart(int id)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>(CartSessionKey)
                   ?? new List<CartItemViewModel>();

        var existingItem = cart.FirstOrDefault(x => x.ProductId == id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            var response = await _httpClient.GetAsync($"Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
                if (product != null)
                {
                    cart.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        Title = product.Title,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        Quantity = 1
                    });
                }
            }
        }

        HttpContext.Session.SetObject(CartSessionKey, cart);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true, totalCount = cart.Sum(x => x.Quantity) });
        }

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public IActionResult Remove(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>(CartSessionKey)
                   ?? new List<CartItemViewModel>();

        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public IActionResult Checkout()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>(CartSessionKey);
        if (cart == null || !cart.Any())
        {
            return RedirectToAction("Index", "Home");
        }
        var orderResult = new OrderResultViewModel
        {
            OrderNumber = "DS-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
            OrderDate = DateTime.Now,
            TotalAmount = cart.Sum(x => x.Total)
        };

        foreach (var item in cart)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                orderResult.Items.Add(new OrderItemKeyViewModel
                {
                    ProductTitle = item.Title,
                    LicenseKey = GenerateGameKey()
                });
            }
        }
        HttpContext.Session.Remove(CartSessionKey);

        return View("OrderSuccess", orderResult);
    }
    private string GenerateGameKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string Part() => new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
        return $"{Part()}-{Part()}-{Part()}";
    }
}