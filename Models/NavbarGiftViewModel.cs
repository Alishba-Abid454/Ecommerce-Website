namespace EcommerceFullstackDesign.Models
{
    public class NavbarGiftViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Items { get; set; }
        public string? Occasion { get; set; }
    }
}
