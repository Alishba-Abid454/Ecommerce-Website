using EcommerceFullstackDesign.ViewModels;

namespace EcommerceFullstackDesign.Models.Interface
{
    public interface IHomeRepository
    {
        // Categories
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryByNameAsync(string name);

        // Products
        Task<List<Product>> GetRecommendedItemsAsync(int count = 8);
        Task<List<Product>> GetDealsAndOffersAsync(int count = 5);
        Task<List<Product>> GetProductsByCategoryAsync(string categoryName, int count = 4);
        Task<Dictionary<string, List<Product>>> GetProductsByCategoriesAsync(List<string> categoryNames);
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByIdsAsync(List<int> ids);

        // Cart
        Task<int> GetUserCartCountAsync(int userId);
        Task<int> AddToCartAsync(int userId, int productId, int quantity = 1);
        Task<List<CartItem>> GetUserCartAsync(int userId);

        // User
        Task<string> GetUserGreetingAsync(int userId);
        Task<UserInfo> GetUserInfoAsync(string? userId = null);

        // Banners and Promotions
        Task<Banner?> GetActiveHeroBannerAsync();
        Task<List<PromotionCard>> GetActivePromotionCardsAsync();

        // Services and Suppliers
        Task<List<Service>> GetActiveServicesAsync(int count = 4);
        Task<List<Supplier>> GetActiveSuppliersAsync(int count = 10);

        // Home Page Data
        Task<HomeViewModel> GetHomePageDataAsync(int? userId = null);

        // Inquiry
        Task<bool> SubmitInquiryAsync(Inquiry inquiry);
    }

    public class UserInfo
    {
        public string Greeting { get; set; } = "Hi, user";
        public string SubGreeting { get; set; } = "let's get started";
        public string AvatarUrl { get; set; } = "/images/avatar.jpg";
    }
}




/*using EcommerceFullstackDesign.ViewModels;

namespace EcommerceFullstackDesign.Models.Interface
{
    public interface IHomeRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Product>> GetRecommendedItemsAsync();
        Task<List<Product>> GetRecommendedItemsAsync(int count);
        Task<Product?> GetProductByIdAsync(int id);
        Task<int> GetUserCartCountAsync(int userId);
        Task<string> GetUserGreetingAsync(int userId);
        Task<Banner?> GetActiveHeroBannerAsync();
        Task<List<PromotionCard>> GetActivePromotionCardsAsync();
        Task<UserInfo> GetUserInfoAsync(string? userId = null);
        Task<HomeViewModel> GetHomePageDataAsync(int? userId = null);
        Task<int> AddToCartAsync(int userId, int productId, int quantity = 1);
    }

    public class UserInfo
    {
        public string Greeting { get; set; } = "Hi, user";
        public string SubGreeting { get; set; } = "let's get started";
        public string AvatarUrl { get; set; } = "/Image/profile.png";
    }
}



*//*using EcommerceFullstackDesign.ViewModels;

namespace EcommerceFullstackDesign.Models.Interface
{
    public interface IHomeRepository
    {
        Task<List<Category>> GetCategoriesAsync();

        Task<List<Product>> GetRecommendedItemsAsync();
        Task<List<Product>> GetRecommendedItemsAsync(int count);

        Task<Product> GetProductByIdAsync(int id);

        Task<int> GetUserCartCountAsync(int userId);

        Task<string> GetUserGreetingAsync(int userId);

        Task<Banner> GetActiveHeroBannerAsync();

        Task<List<PromotionCard>> GetActivePromotionCardsAsync();

        Task<UserInfo> GetUserInfoAsync(string userId = null);

        Task<HomeViewModel> GetHomePageDataAsync();
        Task<HomeViewModel> GetHomePageDataAsync(int? userId);
    }

    public class UserInfo
    {
        public string Greeting { get; set; }
        public string SubGreeting { get; set; }
        public string AvatarUrl { get; set; }
    }
}*/