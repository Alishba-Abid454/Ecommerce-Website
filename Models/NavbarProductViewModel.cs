namespace EcommerceFullstackDesign.Models
{
    public class NavbarProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? DiscountPercent { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public bool IsFeatured { get; set; }
    }
}
