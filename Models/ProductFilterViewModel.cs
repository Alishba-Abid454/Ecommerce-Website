namespace EcommerceFullstackDesign.Models
{
    public class ProductFilterViewModel
    {
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? SelectedBrands { get; set; }
        public List<string>? SelectedFeatures { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Condition { get; set; }
        public int? Rating { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool VerifiedOnly { get; set; }
    }
}
