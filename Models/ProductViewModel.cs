namespace EcommerceFullstackDesign.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Brand { get; set; }
        public List<string>? Features { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int OrderCount { get; set; }
        public bool IsVerified { get; set; }
        public bool FreeShipping { get; set; }
        public DateTime CreatedAt { get; set; }

        // Calculated property for discount percentage
        public int? DiscountPercentage
        {
            get
            {
                if (OldPrice.HasValue && OldPrice > Price)
                {
                    return (int)((OldPrice.Value - Price) / OldPrice.Value * 100);
                }
                return null;
            }
        }
    }
}