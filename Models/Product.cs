namespace EcommerceFullstackDesign.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public string? ImageUrl { get; set; }
        //public string? ImageGallery { get; set; }
        public int CategoryId { get; set; }
        //public int SupplierId { get; set; }
        //public int StockQuantity { get; set; }
        //public string? SKU { get; set; }
/*        public string? Brand { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public string? Weight { get; set; }
        public string? Dimensions { get; set; }*/
        public bool IsRecommended { get; set; }
        //public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        // public DateTime? UpdatedAt { get; set; }
        public Category? Category { get; set; }
    }
}
