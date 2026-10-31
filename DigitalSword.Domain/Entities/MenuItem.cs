namespace DigitalSword.Domain.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; } = 1;
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public MenuItem? Parent { get; set; }
        public ICollection<MenuItem> SubMenus { get; set; } = new List<MenuItem>();
    }
}
