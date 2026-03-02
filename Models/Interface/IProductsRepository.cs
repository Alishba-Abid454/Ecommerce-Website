using EcommerceFullstackDesign.Models;

namespace EcommerceFullstackDesign.Models.Interface
{
    public interface IProductsRepository
    {
        // Get products with filtering and pagination
        Task<ProductListResponse> GetFilteredProductsAsync(ProductFilterViewModel filter);

        // Get single product by ID
        Task<ProductViewModel?> GetProductByIdAsync(int id);

        // Get products by category
        Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int count = 10);

        // Get related products
        Task<List<ProductViewModel>> GetRelatedProductsAsync(int productId, int categoryId, int count = 6);

        // Get all brands
        Task<List<string>> GetBrandsAsync(int? categoryId = null);

        // Get all features
        Task<List<string>> GetFeaturesAsync(int? categoryId = null);

        // Get price range for category
        Task<(decimal Min, decimal Max)> GetPriceRangeAsync(int? categoryId = null);

        // Search products
        Task<ProductListResponse> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10);

        // Get category breadcrumb
        Task<List<BreadcrumbItem>> GetCategoryBreadcrumbAsync(int categoryId);
    }
}






/*namespace EcommerceFullstackDesign.Models.Interface
{
    public interface IProductsRepository
    {
        Task<List<Product>> GetProductsAsync(int? categoryId = null, string? search = null, string? sortBy = null);

        // Get product by ID
        Task<Product?> GetProductByIdAsync(int id);

        // Get products by category
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);

        // Get related products (same category, excluding current product)
        Task<List<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 6);

        // Get filter options (brands, features etc)
        Task<List<string>> GetBrandsAsync();
        Task<List<string>> GetFeaturesAsync();

        // Search products
        Task<List<Product>> SearchProductsAsync(string searchTerm);
    }
}
*/