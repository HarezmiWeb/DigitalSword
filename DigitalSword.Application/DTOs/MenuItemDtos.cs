namespace DigitalSword.Application.DTOs
{
    public class MenuItemResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; }
        public int? ParentId { get; set; }
        public string? ParentTitle { get; set; }
        public bool IsActive { get; set; }
        public List<MenuItemResponseDto> SubMenus { get; set; } = new();
    }

    public class CreateMenuItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; } = 1;
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMenuItemDto : CreateMenuItemDto
    {
        public int Id { get; set; }
    }
}
