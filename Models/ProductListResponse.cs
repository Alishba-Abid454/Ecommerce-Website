namespace EcommerceFullstackDesign.Models
{
    public class ProductListResponse
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public Dictionary<string, int> BrandCounts { get; set; } = new();
        public Dictionary<string, int> FeatureCounts { get; set; } = new();
        public (decimal Min, decimal Max) PriceRange { get; set; }
    }
}
