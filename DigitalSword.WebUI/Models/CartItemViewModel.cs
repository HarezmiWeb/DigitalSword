namespace DigitalSword.WebUI.Models;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}

public class OrderResultViewModel
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemKeyViewModel> Items { get; set; } = new();
}

public class OrderItemKeyViewModel
{
    public string ProductTitle { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
}
