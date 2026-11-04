using DigitalSword.Application.Abstractions;
using DigitalSword.Application.DTOs;
using DigitalSword.Domain.Entities;
using DigitalSword.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalSword.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IGenericRepository<Product> productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? type = null, [FromQuery] string? category = null)
    {
        var products = await _productRepository.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (type.Equals("Epin", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => (int)p.ProductType == 2 || (int)p.ProductType == 3).ToList();
            }
            else if (type.Equals("Game", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => (int)p.ProductType == 1 || (int)p.ProductType == 4).ToList();
            }
        }
        if (!string.IsNullOrWhiteSpace(category) && !category.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            products = products.Where(p =>
                (p.Category != null && p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { message = "Ürün bulunamadı." });

        var response = new ProductResponseDto(
            product.Id,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.ProductType,
            product.CategoryId,
            product.PublisherId,
            product.IsActive,
            product.CreatedAt
        );

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            ProductType = dto.ProductType,
            CategoryId = dto.CategoryId == 0 ? 4 : dto.CategoryId,
            PublisherId = dto.PublisherId == 0 ? 1 : dto.PublisherId
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { id = product.Id, message = "Ürün başarıyla oluşturuldu." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { message = "Güncellenecek ürün bulunamadı." });

        product.Title = dto.Title;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.ImageUrl = dto.ImageUrl;
        product.ProductType = dto.ProductType;
        product.CategoryId = dto.CategoryId;
        product.PublisherId = dto.PublisherId;
        product.IsActive = dto.IsActive;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { message = "Silinecek ürün bulunamadı." });

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}