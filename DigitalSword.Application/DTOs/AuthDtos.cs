using System.ComponentModel.DataAnnotations;

namespace DigitalSword.Application.DTOs;

public record RegisterDto(
    [Required(ErrorMessage = "Ad Soyad zorunludur.")]
    string FullName,

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    string UserName,

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    string Email,

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    string Password
);

public record LoginDto(
    [Required(ErrorMessage = "E-posta veya Kullanıcı Adı giriniz.")]
    string EmailOrUserName,

    [Required(ErrorMessage = "Şifre zorunludur.")]
    string Password
);

public record AuthResponseDto(
    bool IsSuccess,
    string Message,
    string? Token,
    string? UserName,
    string? Email,
    List<string>? Roles
);