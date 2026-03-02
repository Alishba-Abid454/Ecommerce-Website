using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeRepository> _logger;

        public HomeRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<HomeRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // ==================== CATEGORIES ====================

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT Id, Name, Description, ImageUrl,
                      IsActive, DisplayOrder, CreatedAt
                      FROM Categories
                      WHERE IsActive = 1
                      ORDER BY DisplayOrder", connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    categories.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                        IsActive = reader.GetBoolean(4),
                        DisplayOrder = reader.GetInt32(5),
                        CreatedAt = reader.GetDateTime(6)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
            }

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT Id, Name, Description, ImageUrl,
                      IsActive, DisplayOrder, CreatedAt
                      FROM Categories
                      WHERE Id = @Id AND IsActive = 1", connection);

                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                        IsActive = reader.GetBoolean(4),
                        DisplayOrder = reader.GetInt32(5),
                        CreatedAt = reader.GetDateTime(6)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id: {Id}", id);
            }

            return null;
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT Id, Name, Description, ImageUrl,
                      IsActive, DisplayOrder, CreatedAt
                      FROM Categories
                      WHERE Name = @Name AND IsActive = 1", connection);

                command.Parameters.AddWithValue("@Name", name);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                        IsActive = reader.GetBoolean(4),
                        DisplayOrder = reader.GetInt32(5),
                        CreatedAt = reader.GetDateTime(6)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by name: {Name}", name);
            }

            return null;
        }

        // ==================== PRODUCTS ====================

        public async Task<List<Product>> GetRecommendedItemsAsync(int count = 8)
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP (@count) p.Id, p.Name, p.Description, p.Price,
                      p.DiscountPrice, p.OldPrice, p.ImageUrl, p.CategoryId,
                      p.IsRecommended, p.IsPublished, p.CreatedAt,
                      c.Name as CategoryName
                      FROM Products p
                      LEFT JOIN Categories c ON p.CategoryId = c.Id
                      WHERE p.IsRecommended = 1 AND p.IsPublished = 1
                      ORDER BY p.CreatedAt DESC", connection);

                command.Parameters.AddWithValue("@count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        OldPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        ImageUrl = reader.IsDBNull(6) ? "/images/products/default.jpg" : reader.GetString(6),
                        CategoryId = reader.GetInt32(7),
                        IsRecommended = reader.GetBoolean(8),
                        IsPublished = reader.GetBoolean(9),
                        CreatedAt = reader.GetDateTime(10)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended items");
            }

            return products;
        }

        public async Task<List<Product>> GetDealsAndOffersAsync(int count = 5)
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP (@count) Id, Name, Description, Price,
                      DiscountPrice, OldPrice, ImageUrl, CategoryId,
                      IsRecommended, IsPublished, CreatedAt
                      FROM Products
                      WHERE (DiscountPrice IS NOT NULL AND DiscountPrice < Price)
                         OR (OldPrice IS NOT NULL AND OldPrice > Price)
                      AND IsPublished = 1
                      ORDER BY CreatedAt DESC", connection);

                command.Parameters.AddWithValue("@count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        OldPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        ImageUrl = reader.IsDBNull(6) ? "/images/products/default.jpg" : reader.GetString(6),
                        CategoryId = reader.GetInt32(7),
                        IsRecommended = reader.GetBoolean(8),
                        IsPublished = reader.GetBoolean(9),
                        CreatedAt = reader.GetDateTime(10)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deals and offers");
            }

            return products;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string categoryName, int count = 4)
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP (@count) p.Id, p.Name, p.Description, p.Price,
                      p.DiscountPrice, p.OldPrice, p.ImageUrl, p.CategoryId,
                      p.IsRecommended, p.IsPublished, p.CreatedAt
                      FROM Products p
                      INNER JOIN Categories c ON p.CategoryId = c.Id
                      WHERE c.Name = @CategoryName AND p.IsPublished = 1
                      ORDER BY p.CreatedAt DESC", connection);

                command.Parameters.AddWithValue("@count", count);
                command.Parameters.AddWithValue("@CategoryName", categoryName);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        OldPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        ImageUrl = reader.IsDBNull(6) ? "/images/products/default.jpg" : reader.GetString(6),
                        CategoryId = reader.GetInt32(7),
                        IsRecommended = reader.GetBoolean(8),
                        IsPublished = reader.GetBoolean(9),
                        CreatedAt = reader.GetDateTime(10)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category: {CategoryName}", categoryName);
            }

            return products;
        }

        public async Task<Dictionary<string, List<Product>>> GetProductsByCategoriesAsync(List<string> categoryNames)
        {
            var result = new Dictionary<string, List<Product>>();

            try
            {
                foreach (var categoryName in categoryNames)
                {
                    var products = await GetProductsByCategoryAsync(categoryName, 4);
                    result[categoryName] = products;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by categories");
            }

            return result;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT p.Id, p.Name, p.Description, p.Price,
                      p.DiscountPrice, p.OldPrice, p.ImageUrl, p.CategoryId,
                      p.IsRecommended, p.IsPublished, p.CreatedAt,
                      c.Name as CategoryName
                      FROM Products p
                      LEFT JOIN Categories c ON p.CategoryId = c.Id
                      WHERE p.Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        OldPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        ImageUrl = reader.IsDBNull(6) ? "/images/products/default.jpg" : reader.GetString(6),
                        CategoryId = reader.GetInt32(7),
                        IsRecommended = reader.GetBoolean(8),
                        IsPublished = reader.GetBoolean(9),
                        CreatedAt = reader.GetDateTime(10),
                        Category = new Category
                        {
                            Id = reader.GetInt32(7),
                            Name = reader.IsDBNull(11) ? "" : reader.GetString(11)
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by id: {Id}", id);
            }

            return null;
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            var products = new List<Product>();

            if (ids == null || !ids.Any())
                return products;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var idList = string.Join(",", ids);
                var query = $@"SELECT Id, Name, Description, Price,
                              DiscountPrice, OldPrice, ImageUrl, CategoryId,
                              IsRecommended, IsPublished, CreatedAt
                              FROM Products
                              WHERE Id IN ({idList}) AND IsPublished = 1";

                using var command = new SqlCommand(query, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        OldPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                        ImageUrl = reader.IsDBNull(6) ? "/images/products/default.jpg" : reader.GetString(6),
                        CategoryId = reader.GetInt32(7),
                        IsRecommended = reader.GetBoolean(8),
                        IsPublished = reader.GetBoolean(9),
                        CreatedAt = reader.GetDateTime(10)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by ids");
            }

            return products;
        }

        // ==================== CART ====================

        public async Task<int> GetUserCartCountAsync(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT COUNT(*) FROM CartItems WHERE UserId = @UserId",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count for user: {UserId}", userId);
                return 0;
            }
        }

        public async Task<int> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Check if product already in cart
                using var checkCommand = new SqlCommand(
                    "SELECT Id, Quantity FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
                    connection);

                checkCommand.Parameters.AddWithValue("@UserId", userId);
                checkCommand.Parameters.AddWithValue("@ProductId", productId);

                using var reader = await checkCommand.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Update existing cart item
                    int cartItemId = reader.GetInt32(0);
                    int currentQuantity = reader.GetInt32(1);
                    reader.Close();

                    using var updateCommand = new SqlCommand(
                        "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id",
                        connection);

                    updateCommand.Parameters.AddWithValue("@Quantity", currentQuantity + quantity);
                    updateCommand.Parameters.AddWithValue("@Id", cartItemId);
                    await updateCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    reader.Close();

                    // Insert new cart item
                    using var insertCommand = new SqlCommand(
                        "INSERT INTO CartItems (UserId, ProductId, Quantity, AddedAt) VALUES (@UserId, @ProductId, @Quantity, @AddedAt)",
                        connection);

                    insertCommand.Parameters.AddWithValue("@UserId", userId);
                    insertCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                    insertCommand.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                    await insertCommand.ExecuteNonQueryAsync();
                }

                // Return updated cart count
                return await GetUserCartCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return 0;
            }
        }

        public async Task<List<CartItem>> GetUserCartAsync(int userId)
        {
            var cartItems = new List<CartItem>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT Id, UserId, ProductId, Quantity, AddedAt FROM CartItems WHERE UserId = @UserId ORDER BY AddedAt DESC",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    cartItems.Add(new CartItem
                    {
                        Id = reader.GetInt32(0),        // Column 0: Id
                        UserId = reader.GetInt32(1),     // Column 1: UserId
                        ProductId = reader.GetInt32(2),   // Column 2: ProductId
                        Quantity = reader.GetInt32(3),    // Column 3: Quantity
                        AddedAt = reader.GetDateTime(4)   // Column 4: AddedAt
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user cart");
            }

            return cartItems;
        }

        // ==================== USER ====================

        public async Task<string> GetUserGreetingAsync(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT Username FROM Users WHERE Id = @UserId",
                    connection);

                command.Parameters.AddWithValue("@UserId", userId);

                var result = await command.ExecuteScalarAsync();
                return result != null ? $"Hi, {result}" : "Hi, User";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user greeting");
                return "Hi, User";
            }
        }

        public async Task<UserInfo> GetUserInfoAsync(string? userId = null)
        {
            var context = _httpContextAccessor.HttpContext;

            // Check session first
            if (context?.Session.GetString("UserGreeting") != null)
            {
                return new UserInfo
                {
                    Greeting = context.Session.GetString("UserGreeting") ?? "Hi, user",
                    SubGreeting = context.Session.GetString("UserSubGreeting") ?? "let's get started",
                    AvatarUrl = context.Session.GetString("UserAvatar") ?? "/images/avatar.jpg"
                };
            }

            if (string.IsNullOrEmpty(userId) || userId == "0")
            {
                return new UserInfo();
            }

            if (!int.TryParse(userId, out int id))
            {
                return new UserInfo();
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT Username, AvatarUrl FROM Users WHERE Id = @UserId",
                    connection);

                command.Parameters.AddWithValue("@UserId", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var userInfo = new UserInfo
                    {
                        Greeting = $"Hi, {reader.GetString(0)}",
                        SubGreeting = "let's get started",
                        AvatarUrl = reader.IsDBNull(1) ? "/images/avatar.jpg" : reader.GetString(1)
                    };

                    // Store in session
                    context?.Session.SetString("UserGreeting", userInfo.Greeting);
                    context?.Session.SetString("UserSubGreeting", userInfo.SubGreeting);
                    context?.Session.SetString("UserAvatar", userInfo.AvatarUrl);

                    return userInfo;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info");
            }

            return new UserInfo();
        }

        // ==================== BANNERS AND PROMOTIONS ====================

        public async Task<Banner?> GetActiveHeroBannerAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP 1 Id, Title, Subtitle, ImageUrl, ButtonText,
                      ButtonLink, IsActive, DisplayOrder
                      FROM Banners WHERE IsActive = 1
                      ORDER BY DisplayOrder", connection);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Banner
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ImageUrl = reader.GetString(3),
                        ButtonText = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ButtonLink = reader.IsDBNull(5) ? null : reader.GetString(5),
                        IsActive = reader.GetBoolean(6),
                        DisplayOrder = reader.GetInt32(7)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active hero banner");
            }

            return null;
        }

        public async Task<List<PromotionCard>> GetActivePromotionCardsAsync()
        {
            var promotionCards = new List<PromotionCard>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT Id, Title, Description, BackgroundColor, TextColor,
                      ButtonText, ButtonLink, IsActive, DisplayOrder
                      FROM PromotionCards WHERE IsActive = 1
                      ORDER BY DisplayOrder", connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    promotionCards.Add(new PromotionCard
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Description = reader.GetString(2),
                        BackgroundColor = reader.GetString(3),
                        TextColor = reader.GetString(4),
                        ButtonText = reader.IsDBNull(5) ? null : reader.GetString(5),
                        ButtonLink = reader.IsDBNull(6) ? null : reader.GetString(6),
                        IsActive = reader.GetBoolean(7),
                        DisplayOrder = reader.GetInt32(8)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active promotion cards");
            }

            return promotionCards;
        }

        // ==================== SERVICES AND SUPPLIERS ====================

        public async Task<List<Service>> GetActiveServicesAsync(int count = 4)
        {
            var services = new List<Service>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP (@count) Id, Title, Description, ImageUrl, Icon,
                      IsActive, DisplayOrder
                      FROM Services WHERE IsActive = 1
                      ORDER BY DisplayOrder", connection);

                command.Parameters.AddWithValue("@count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    services.Add(new Service
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        ImageUrl = reader.GetString(3),
                        Icon = reader.IsDBNull(4) ? null : reader.GetString(4),
                        IsActive = reader.GetBoolean(5),
                        DisplayOrder = reader.GetInt32(6)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active services");
            }

            return services;
        }

        public async Task<List<Supplier>> GetActiveSuppliersAsync(int count = 10)
        {
            var suppliers = new List<Supplier>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP (@count) Id, Name, Country, FlagIcon, Website,
                      IsActive, DisplayOrder
                      FROM Suppliers WHERE IsActive = 1
                      ORDER BY DisplayOrder", connection);

                command.Parameters.AddWithValue("@count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    suppliers.Add(new Supplier
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Country = reader.GetString(2),
                        FlagIcon = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Website = reader.IsDBNull(4) ? null : reader.GetString(4),
                        IsActive = reader.GetBoolean(5),
                        DisplayOrder = reader.GetInt32(6)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active suppliers");
            }

            return suppliers;
        }

        // ==================== HOME PAGE DATA ====================

        public async Task<HomeViewModel> GetHomePageDataAsync(int? userId = null)
        {
            var viewModel = new HomeViewModel
            {
                Categories = await GetCategoriesAsync(),
                RecommendedItems = await GetRecommendedItemsAsync(8),
                DealsAndOffers = await GetDealsAndOffersAsync(5),
                Services = await GetActiveServicesAsync(4),
                Suppliers = await GetActiveSuppliersAsync(10),
                HeroBanner = await GetActiveHeroBannerAsync(),
                PromotionCards = await GetActivePromotionCardsAsync()
            };

            // Get products for specific categories
            var categoryNames = new List<string> { "Home & Living", "Electronics" };
            viewModel.ProductsByCategory = await GetProductsByCategoriesAsync(categoryNames);

            if (userId.HasValue && userId.Value > 0)
            {
                viewModel.UserId = userId.Value;
                viewModel.CartCount = await GetUserCartCountAsync(userId.Value);

                var userInfo = await GetUserInfoAsync(userId.Value.ToString());
                viewModel.UserGreeting = userInfo.Greeting;
                viewModel.UserSubGreeting = userInfo.SubGreeting;
                viewModel.UserAvatarUrl = userInfo.AvatarUrl;
            }

            return viewModel;
        }

        // ==================== INQUIRY ====================

        public async Task<bool> SubmitInquiryAsync(Inquiry inquiry)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"INSERT INTO Inquiries (Product, Details, Quantity, Unit, CreatedAt, UserId)
                      VALUES (@Product, @Details, @Quantity, @Unit, @CreatedAt, @UserId)",
                    connection);

                command.Parameters.AddWithValue("@Product", inquiry.Product);
                command.Parameters.AddWithValue("@Details", inquiry.Details);
                command.Parameters.AddWithValue("@Quantity", inquiry.Quantity);
                command.Parameters.AddWithValue("@Unit", inquiry.Unit);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UserId", inquiry.UserId ?? (object)DBNull.Value);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting inquiry");
                return false;
            }
        }
    }
}




/*using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.Data.SqlClient;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT Id, Name, Description, ImageUrl,
                  IsActive, DisplayOrder, CreatedAt
                  FROM Categories
                  WHERE IsActive = 1
                  ORDER BY DisplayOrder", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsActive = reader.GetBoolean(4),
                    DisplayOrder = reader.GetInt32(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            return categories;
        }

        public async Task<List<Product>> GetRecommendedItemsAsync()
        {
            return await GetRecommendedItemsAsync(7);
        }

        *//*    public async Task<List<Product>> GetRecommendedItemsAsync(int count)
            {
                var products = new List<Product>();

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    $@"SELECT TOP {count} Id, Name, Description, Price,
                       DiscountPrice, ImageUrl, CategoryId,
                       IsRecommended, IsPublished, CreatedAt
                       FROM Products
                       WHERE IsRecommended = 1 AND IsPublished = 1
                       ORDER BY CreatedAt DESC", connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                        CategoryId = reader.GetInt32(6),
                        IsRecommended = reader.GetBoolean(7),
                        IsPublished = reader.GetBoolean(8),
                        CreatedAt = reader.GetDateTime(9)
                    });
                }

                return products;
            }*//*
        public async Task<List<Product>> GetRecommendedItemsAsync(int count)
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    @"SELECT TOP @count Id, Name, Description, Price,
              DiscountPrice, ImageUrl, CategoryId,
              IsRecommended, IsPublished, CreatedAt
              FROM Products
              WHERE IsRecommended = 1 AND IsPublished = 1
              ORDER BY CreatedAt DESC", connection);

                command.Parameters.AddWithValue("@count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                        ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                        CategoryId = reader.GetInt32(6),
                        IsRecommended = reader.GetBoolean(7),
                        IsPublished = reader.GetBoolean(8),
                        CreatedAt = reader.GetDateTime(9)
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the error (you'll need to inject ILogger or use Console.WriteLine for now)
                Console.WriteLine($"Error getting recommended items: {ex.Message}");
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT Id, Name, Description, Price,
                  DiscountPrice, ImageUrl, CategoryId,
                  IsRecommended, IsPublished, CreatedAt
                  FROM Products WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.GetInt32(6),
                    IsRecommended = reader.GetBoolean(7),
                    IsPublished = reader.GetBoolean(8),
                    CreatedAt = reader.GetDateTime(9)
                };
            }

            return null;
        }

        public async Task<int> GetUserCartCountAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT COUNT(*) FROM CartItems WHERE UserId = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<string> GetUserGreetingAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Username FROM Users WHERE Id = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();
            return result != null ? $"Hi, {result}" : "Hi, User";
        }

        public async Task<Banner?> GetActiveHeroBannerAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT TOP 1 Id, Title, Subtitle, ImageUrl, ButtonText,
                  ButtonLink, IsActive, DisplayOrder
                  FROM Banners WHERE IsActive = 1
                  ORDER BY DisplayOrder", connection);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Banner
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ImageUrl = reader.GetString(3),
                    ButtonText = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ButtonLink = reader.IsDBNull(5) ? null : reader.GetString(5),
                    IsActive = reader.GetBoolean(6),
                    DisplayOrder = reader.GetInt32(7)
                };
            }

            return null;
        }

        public async Task<List<PromotionCard>> GetActivePromotionCardsAsync()
        {
            var promotionCards = new List<PromotionCard>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT Id, Title, Description, BackgroundColor, TextColor,
                  ButtonText, ButtonLink, IsActive, DisplayOrder
                  FROM PromotionCards WHERE IsActive = 1
                  ORDER BY DisplayOrder", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                promotionCards.Add(new PromotionCard
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Description = reader.GetString(2),
                    BackgroundColor = reader.GetString(3),
                    TextColor = reader.GetString(4),
                    ButtonText = reader.IsDBNull(5) ? null : reader.GetString(5),
                    ButtonLink = reader.IsDBNull(6) ? null : reader.GetString(6),
                    IsActive = reader.GetBoolean(7),
                    DisplayOrder = reader.GetInt32(8)
                });
            }

            return promotionCards;
        }

        public async Task<UserInfo> GetUserInfoAsync(string? userId = null)
        {
            // Check session first
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session.GetString("UserGreeting") != null)
            {
                return new UserInfo
                {
                    Greeting = context.Session.GetString("UserGreeting") ?? "Hi, user",
                    SubGreeting = context.Session.GetString("UserSubGreeting") ?? "let's get started",
                    AvatarUrl = context.Session.GetString("UserAvatar") ?? "/Image/profile.png"
                };
            }

            if (string.IsNullOrEmpty(userId) || userId == "0")
            {
                return new UserInfo();
            }

            if (!int.TryParse(userId, out int id))
            {
                return new UserInfo();
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Username, AvatarUrl FROM Users WHERE Id = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var userInfo = new UserInfo
                {
                    Greeting = $"Hi, {reader.GetString(0)}",
                    SubGreeting = "let's get started",
                    AvatarUrl = reader.IsDBNull(1) ? "/Image/profile.png" : reader.GetString(1)
                };

                // Store in session
                context?.Session.SetString("UserGreeting", userInfo.Greeting);
                context?.Session.SetString("UserSubGreeting", userInfo.SubGreeting);
                context?.Session.SetString("UserAvatar", userInfo.AvatarUrl);

                return userInfo;
            }

            return new UserInfo();
        }

        public async Task<HomeViewModel> GetHomePageDataAsync(int? userId = null)
        {
            var viewModel = new HomeViewModel
            {
                Categories = await GetCategoriesAsync(),
                RecommendedItems = await GetRecommendedItemsAsync(7),
                HeroBanner = await GetActiveHeroBannerAsync(),
                PromotionCards = await GetActivePromotionCardsAsync()
            };

            if (userId.HasValue && userId.Value > 0)
            {
                viewModel.UserId = userId.Value;
                viewModel.CartCount = await GetUserCartCountAsync(userId.Value);

                var userInfo = await GetUserInfoAsync(userId.Value.ToString());
                viewModel.UserGreeting = userInfo.Greeting;
                viewModel.UserSubGreeting = userInfo.SubGreeting;
                viewModel.UserAvatarUrl = userInfo.AvatarUrl;
            }

            return viewModel;
        }

        public async Task<int> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if product already in cart
            using var checkCommand = new SqlCommand(
                "SELECT Id, Quantity FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId",
                connection);

            checkCommand.Parameters.AddWithValue("@UserId", userId);
            checkCommand.Parameters.AddWithValue("@ProductId", productId);

            using var reader = await checkCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Update existing cart item
                int cartItemId = reader.GetInt32(0);
                int currentQuantity = reader.GetInt32(1);
                reader.Close();

                using var updateCommand = new SqlCommand(
                    "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id",
                    connection);

                updateCommand.Parameters.AddWithValue("@Quantity", currentQuantity + quantity);
                updateCommand.Parameters.AddWithValue("@Id", cartItemId);
                await updateCommand.ExecuteNonQueryAsync();
            }
            else
            {
                reader.Close();

                // Insert new cart item
                using var insertCommand = new SqlCommand(
                    "INSERT INTO CartItems (UserId, ProductId, Quantity, AddedAt) VALUES (@UserId, @ProductId, @Quantity, @AddedAt)",
                    connection);

                insertCommand.Parameters.AddWithValue("@UserId", userId);
                insertCommand.Parameters.AddWithValue("@ProductId", productId);
                insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                insertCommand.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                await insertCommand.ExecuteNonQueryAsync();
            }

            // Return updated cart count
            return await GetUserCartCountAsync(userId);
        }
    }
}




*//*using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.Data.SqlClient;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly string _connectionString;

        public HomeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ==============================
        // HOME PAGE DATA
        // ==============================

        public async Task<HomeViewModel> GetHomePageDataAsync()
        {
            return await GetHomePageDataAsync(null);
        }

        public async Task<HomeViewModel> GetHomePageDataAsync(int? userId)
        {
            var model = new HomeViewModel
            {
                Categories = await GetCategoriesAsync(),
                RecommendedItems = await GetRecommendedItemsAsync(8),
                HeroBanner = await GetActiveHeroBannerAsync(),
                PromotionCards = await GetActivePromotionCardsAsync()
            };

            if (userId.HasValue)
            {
                model.CartCount = await GetUserCartCountAsync(userId.Value);

                var userInfo = await GetUserInfoAsync(userId.Value.ToString());
                model.UserGreeting = userInfo.Greeting;
                model.UserSubGreeting = userInfo.SubGreeting;
                model.UserAvatarUrl = userInfo.AvatarUrl;
            }
            else
            {
                model.CartCount = 0;
                model.UserGreeting = "Hi, Guest";
                model.UserSubGreeting = "Welcome to our store";
                model.UserAvatarUrl = "/Image/profile.png";
            }

            return model;
        }

        // ==============================
        // CATEGORIES
        // ==============================

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT DISTINCT Id, Name, Description, ImageUrl,
                  IsActive, DisplayOrder, CreatedAt
                  FROM Categories
                  WHERE IsActive = 1
                  ORDER BY DisplayOrder", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsActive = reader.GetBoolean(4),
                    DisplayOrder = reader.GetInt32(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            return categories;
        }

        // ==============================
        // RECOMMENDED PRODUCTS
        // ==============================

        public async Task<List<Product>> GetRecommendedItemsAsync()
        {
            return await GetRecommendedItemsAsync(7);
        }

        public async Task<List<Product>> GetRecommendedItemsAsync(int count)
        {
            var products = new List<Product>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                $@"SELECT TOP {count} Id, Name, Description, Price,
                   DiscountPrice, ImageUrl, CategoryId,
                   IsRecommended, IsPublished, CreatedAt
                   FROM Products
                   WHERE IsRecommended = 1 AND IsPublished = 1
                   ORDER BY CreatedAt DESC", connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    IsRecommended = reader.GetBoolean(7),
                    IsPublished = reader.GetBoolean(8),
                    CreatedAt = reader.GetDateTime(9)
                });
            }

            return products;
        }

        // ==============================
        // PRODUCT DETAILS
        // ==============================

        public async Task<Product> GetProductByIdAsync(int id)
        {
            Product product = null;

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                @"SELECT Id, Name, Description, Price,
                  DiscountPrice, ImageUrl, CategoryId,
                  IsRecommended, IsPublished, CreatedAt
                  FROM Products WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                product = new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                    CategoryId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    IsRecommended = reader.GetBoolean(7),
                    IsPublished = reader.GetBoolean(8),
                    CreatedAt = reader.GetDateTime(9)
                };
            }

            return product;
        }

        // ==============================
        // CART
        // ==============================

        public async Task<int> GetUserCartCountAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT COUNT(*) FROM CartItems WHERE UserId = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<string> GetUserGreetingAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Username FROM Users WHERE Id = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();

            return result != null ? $"Hi, {result}" : "Hi, User";
        }

        // ==============================
        // USER INFO
        // ==============================

        public async Task<UserInfo> GetUserInfoAsync(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new UserInfo
                {
                    Greeting = "Hi, User",
                    SubGreeting = "Let's get started",
                    AvatarUrl = "/Image/profile.png"
                };
            }

            int id = int.Parse(userId);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Username, AvatarUrl FROM Users WHERE Id = @UserId",
                connection);

            command.Parameters.AddWithValue("@UserId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserInfo
                {
                    Greeting = $"Hi, {reader.GetString(0)}",
                    SubGreeting = "Let's get started",
                    AvatarUrl = reader.IsDBNull(1)
                        ? "/Image/profile.png"
                        : reader.GetString(1)
                };
            }

            return new UserInfo
            {
                Greeting = "Hi, User",
                SubGreeting = "Let's get started",
                AvatarUrl = "/Image/profile.png"
            };
        }

        public Task<Banner> GetActiveHeroBannerAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<PromotionCard>> GetActivePromotionCardsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
*/


/*using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.Data.SqlClient;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly string _connectionString;

        public HomeRepository(IConfiguration configuration)
        {
            // _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Use DISTINCT to prevent duplicates
                using (var command = new SqlCommand("SELECT DISTINCT Id, Name, Description, ImageUrl, " +
                    "IsActive, DisplayOrder, CreatedAt FROM Categories " +
                    "WHERE IsActive = 1 ORDER BY DisplayOrder", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new Category
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                                IsActive = reader.GetBoolean(4),
                                DisplayOrder = reader.GetInt32(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }
            return categories;
        }
        
        public async Task<List<Product>> GetRecommendedItemsAsync()
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Name, Description, Price, DiscountPrice," +
                    " ImageUrl, CategoryId, IsRecommended, IsPublished, CreatedAt FROM Products WHERE IsRecommended = 1 AND IsPublished = 1 ORDER BY CreatedAt DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CategoryId = reader.GetInt32(6),
                                IsRecommended = reader.GetBoolean(7),
                                IsPublished = reader.GetBoolean(8),
                                CreatedAt = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }

            return products;
        }

        // Overloaded method with count parameter
        public async Task<List<Product>> GetRecommendedItemsAsync(int count = 7)
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand($"SELECT TOP {count} Id, Name, Description, Price, " +
                    $"DiscountPrice, ImageUrl, CategoryId, IsRecommended, IsPublished, CreatedAt FROM Products WHERE IsRecommended = 1 AND IsPublished = 1 ORDER BY CreatedAt DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CategoryId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                IsRecommended = reader.GetBoolean(7),
                                IsPublished = reader.GetBoolean(8),
                                CreatedAt = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            Product product = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Name, Description, Price, DiscountPrice," +
                    " ImageUrl, CategoryId, IsRecommended, IsPublished, CreatedAt FROM Products WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                DiscountPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CategoryId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                IsRecommended = reader.GetBoolean(7),
                                IsPublished = reader.GetBoolean(8),
                                CreatedAt = reader.GetDateTime(9)
                            };
                        }
                    }
                }
            }

            return product;
        }
        public async Task<HomeViewModel> GetHomePageDataAsync(int? userId)
        {
            var model = new HomeViewModel
            {
                Categories = await GetCategoriesAsync(),
                RecommendedItems = await GetRecommendedItemsAsync(8),
                HeroBanner = await GetActiveHeroBannerAsync(),
                PromotionCards = await GetActivePromotionCardsAsync()
            };

            if (userId.HasValue)
            {
                model.CartCount = await GetUserCartCountAsync(userId.Value);

                var userInfo = await GetUserInfoAsync(userId.Value.ToString());
                model.UserGreeting = userInfo.Greeting;
                model.UserSubGreeting = userInfo.SubGreeting;
                model.UserAvatarUrl = userInfo.AvatarUrl;
            }
            else
            {
                model.CartCount = 0;
                model.UserGreeting = "Hi, Guest";
                model.UserSubGreeting = "Welcome to our store";
                model.UserAvatarUrl = "/Image/profile.png";
            }

            return model;
        }
        public async Task<int> GetUserCartCountAsync(int userId)
        {
            int count = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT COUNT(*) FROM CartItems WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    count = (int)await command.ExecuteScalarAsync();
                }
            }

            return count;
        }

        public async Task<string> GetUserGreetingAsync(int userId)
        {
            string username = "user";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Username FROM Users WHERE Id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    var result = await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }
            }

            return $"Hi, {username}";
        }

        public async Task<Banner> GetActiveHeroBannerAsync()
        {
            Banner banner = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT TOP 1 Id, Title, Subtitle, ImageUrl, ButtonText, " +
                    "ButtonLink, IsActive, DisplayOrder FROM Banners WHERE IsActive = 1 ORDER BY DisplayOrder", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            banner = new Banner
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Subtitle = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImageUrl = reader.GetString(3),
                                ButtonText = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ButtonLink = reader.IsDBNull(5) ? null : reader.GetString(5),
                                IsActive = reader.GetBoolean(6),
                                DisplayOrder = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }

            return banner;
        }

        public async Task<List<PromotionCard>> GetActivePromotionCardsAsync()
        {
            var promotionCards = new List<PromotionCard>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Id, Title, Description, BackgroundColor, TextColor," +
                    " ButtonText, ButtonLink, IsActive, DisplayOrder FROM PromotionCards WHERE IsActive = 1 ORDER BY DisplayOrder", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            promotionCards.Add(new PromotionCard
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Description = reader.GetString(2),
                                BackgroundColor = reader.GetString(3),
                                TextColor = reader.GetString(4),
                                ButtonText = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ButtonLink = reader.IsDBNull(6) ? null : reader.GetString(6),
                                IsActive = reader.GetBoolean(7),
                                DisplayOrder = reader.GetInt32(8)
                            });
                        }
                    }
                }
            }

            return promotionCards;
        }
        public async Task<HomeViewModel> GetHomePageDataAsync()
        {
            var viewModel = new HomeViewModel
            {
                Categories = await GetCategoriesAsync(),
                RecommendedItems = await GetRecommendedItemsAsync(7),
                HeroBanner = await GetActiveHeroBannerAsync(),
                PromotionCards = await GetActivePromotionCardsAsync(),
                CartCount = await GetUserCartCountAsync(1)  // 👈 ADD THIS LINE (default userId=1)
            };

            var userInfo = await GetUserInfoAsync();
            viewModel.UserGreeting = userInfo.Greeting;
            viewModel.UserSubGreeting = userInfo.SubGreeting;
            viewModel.UserAvatarUrl = userInfo.AvatarUrl;

            return viewModel;
        }

        public async Task<UserInfo> GetUserInfoAsync(string userId = null)
        {
            // For demo purposes, return default user info
            // In a real app, you would get this from the logged-in user
            if (string.IsNullOrEmpty(userId))
            {
                return new UserInfo
                {
                    Greeting = "Hi, user",
                    SubGreeting = "let's get started",
                    AvatarUrl = "/Image/profile.png"
                };
            }

            // If userId is provided, get from database
            int id = int.Parse(userId);
            string username = "user";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT Username, AvatarUrl FROM Users WHERE Id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            username = reader.GetString(0);
                            var avatarUrl = reader.IsDBNull(1) ? "/Image/profile.png" : reader.GetString(1);

                            return new UserInfo
                            {
                                Greeting = $"Hi, {username}",
                                SubGreeting = "let's get started",
                                AvatarUrl = avatarUrl
                            };
                        }
                    }
                }
            }

            return new UserInfo
            {
                Greeting = $"Hi, {username}",
                SubGreeting = "let's get started",
                AvatarUrl = "/Image/profile.png"
            };
        }

        Task<string?> IHomeRepository.GetHomePageDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}*/