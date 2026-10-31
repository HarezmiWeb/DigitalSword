using System.Security.Claims;
using DigitalSword.Application.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSword.WebUI.Controllers;

public class AuthController : Controller
{
    private readonly HttpClient _httpClient;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DigitalSwordApi");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.EmailOrUserName),
                new Claim(ClaimTypes.Email, dto.EmailOrUserName)
            };
            if (dto.EmailOrUserName.Contains("admin", StringComparison.OrdinalIgnoreCase))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"API Hatası ({response.StatusCode}): {error}");
        return View(dto);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction(nameof(Login));
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"Kayıt başarısız: {error}");
        return View(dto);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}