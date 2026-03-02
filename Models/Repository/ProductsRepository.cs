using EcommerceFullstackDesign.Models;
using EcommerceFullstackDesign.Models.Interface;
using Microsoft.Data.SqlClient;
using System.Text;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(IConfiguration configuration, ILogger<ProductsRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            _logger = logger;
        }
        public async Task<ProductListResponse> GetFilteredProductsAsync(ProductFilterViewModel filter)
        {
            var response = new ProductListResponse();
            var products = new List<ProductViewModel>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // SIMPLIFIED QUERY FOR TESTING
                string query = @"
            SELECT 
                p.Id, p.Name, p.Description, p.Price, 
                p.OldPrice, p.DiscountPrice, p.ImageUrl, 
                p.CategoryId, p.Brand, p.Rating, 
                p.ReviewCount, p.OrderCount, p.IsVerified,
                p.FreeShipping, p.CreatedAt,
                c.Name as CategoryName
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
            ORDER BY p.Name";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", filter.CategoryId.HasValue ? (object)filter.CategoryId.Value : DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapToProductViewModel(reader));
                }

                response.Products = products;
                response.TotalCount = products.Count;
                response.Page = filter.Page;
                response.PageSize = filter.PageSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered products");
            }

            return response;
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        p.Id, p.Name, p.Description, p.Price, 
                        p.OldPrice, p.DiscountPrice, p.ImageUrl, 
                        p.CategoryId, p.Brand, p.Rating, 
                        p.ReviewCount, p.OrderCount, p.IsVerified,
                        p.FreeShipping, p.CreatedAt,
                        c.Name as CategoryName
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    WHERE p.Id = @Id";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return MapToProductViewModel(reader);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by id: {Id}", id);
            }

            return null;
        }

        public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId, int count = 10)
        {
            var products = new List<ProductViewModel>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT TOP (@Count)
                        p.Id, p.Name, p.Description, p.Price, 
                        p.OldPrice, p.DiscountPrice, p.ImageUrl, 
                        p.CategoryId, p.Brand, p.Rating, 
                        p.ReviewCount, p.OrderCount, p.IsVerified,
                        p.FreeShipping, p.CreatedAt,
                        c.Name as CategoryName
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    WHERE p.CategoryId = @CategoryId
                    ORDER BY p.CreatedAt DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapToProductViewModel(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category");
            }

            return products;
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(int productId, int categoryId, int count = 6)
        {
            var products = new List<ProductViewModel>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT TOP (@Count)
                        p.Id, p.Name, p.Description, p.Price, 
                        p.OldPrice, p.DiscountPrice, p.ImageUrl, 
                        p.CategoryId, p.Brand, p.Rating, 
                        p.ReviewCount, p.OrderCount, p.IsVerified,
                        p.FreeShipping, p.CreatedAt,
                        c.Name as CategoryName
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    WHERE p.CategoryId = @CategoryId AND p.Id != @ProductId
                    ORDER BY NEWID()"; // Random order

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapToProductViewModel(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related products");
            }

            return products;
        }

        public async Task<List<string>> GetBrandsAsync(int? categoryId = null)
        {
            var brands = new List<string>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT DISTINCT Brand FROM Products WHERE Brand IS NOT NULL AND Brand != ''";

                if (categoryId.HasValue)
                {
                    query += " AND CategoryId = @CategoryId";
                }

                query += " ORDER BY Brand";

                using var command = new SqlCommand(query, connection);

                if (categoryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    brands.Add(reader.GetString(0));
                }

                _logger.LogInformation($"Found {brands.Count} brands for category {categoryId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brands");
            }

            return brands;
        }

        /*public async Task<List<string>> GetBrandsAsync(int? categoryId = null)
        {
            var brands = new List<string>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT DISTINCT Brand FROM Products WHERE Brand IS NOT NULL";

                if (categoryId.HasValue)
                {
                    query += " AND CategoryId = @CategoryId";
                }

                query += " ORDER BY Brand";

                using var command = new SqlCommand(query, connection);

                if (categoryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    brands.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brands");
            }

            return brands;
        }*/

        public async Task<Dictionary<string, int>> GetBrandCountsAsync(int? categoryId = null)
        {
            var brandCounts = new Dictionary<string, int>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT Brand, COUNT(*) as Count FROM Products WHERE Brand IS NOT NULL";

                if (categoryId.HasValue)
                {
                    query += " AND CategoryId = @CategoryId";
                }

                query += " GROUP BY Brand ORDER BY Brand";

                using var command = new SqlCommand(query, connection);

                if (categoryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    brandCounts.Add(reader.GetString(0), reader.GetInt32(1));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brand counts");
            }

            return brandCounts;
        }

        public async Task<List<string>> GetFeaturesAsync(int? categoryId = null)
        {
            // You can implement this based on your database structure
            // For now, returning static list
            return new List<string> { "Metallic", "8GB Ram", "Super power", "Plastic cover", "Large Memory" };
        }

        public async Task<(decimal Min, decimal Max)> GetPriceRangeAsync(int? categoryId = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT ISNULL(MIN(Price), 0), ISNULL(MAX(Price), 1000) FROM Products WHERE 1=1";

                if (categoryId.HasValue)
                {
                    query += " AND CategoryId = @CategoryId";
                }

                using var command = new SqlCommand(query, connection);

                if (categoryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var min = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                    var max = reader.IsDBNull(1) ? 1000 : reader.GetDecimal(1);

                    _logger.LogInformation($"Price range for category {categoryId}: Min={min}, Max={max}");
                    return (min, max);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price range");
            }

            return (0, 1000);
        }

        /* public async Task<(decimal Min, decimal Max)> GetPriceRangeAsync(int? categoryId = null)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT MIN(Price), MAX(Price) FROM Products WHERE 1=1";

                if (categoryId.HasValue)
                {
                    query += " AND CategoryId = @CategoryId";
                }

                using var command = new SqlCommand(query, connection);

                if (categoryId.HasValue)
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var min = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                    var max = reader.IsDBNull(1) ? 1000 : reader.GetDecimal(1);
                    return (min, max);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price range");
            }

            return (0, 1000);
        }*/

        public async Task<ProductListResponse> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var filter = new ProductFilterViewModel
            {
                Page = page,
                PageSize = pageSize
            };

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var searchPattern = $"%{searchTerm}%";

                var whereClause = "WHERE p.Name LIKE @Search OR p.Description LIKE @Search OR p.Brand LIKE @Search";

                // Get total count
                var countQuery = $@"
                    SELECT COUNT(*) 
                    FROM Products p 
                    {whereClause}";

                using var countCommand = new SqlCommand(countQuery, connection);
                countCommand.Parameters.AddWithValue("@Search", searchPattern);
                var totalCount = (int)await countCommand.ExecuteScalarAsync();

                // Get paginated products
                var offset = (page - 1) * pageSize;
                var query = $@"
                    SELECT 
                        p.Id, p.Name, p.Description, p.Price, 
                        p.OldPrice, p.DiscountPrice, p.ImageUrl, 
                        p.CategoryId, p.Brand, p.Rating, 
                        p.ReviewCount, p.OrderCount, p.IsVerified,
                        p.FreeShipping, p.CreatedAt,
                        c.Name as CategoryName
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    {whereClause}
                    ORDER BY p.Name
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Search", searchPattern);
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                var products = new List<ProductViewModel>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(MapToProductViewModel(reader));
                }

                return new ProductListResponse
                {
                    Products = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return new ProductListResponse();
            }
        }

        public async Task<List<BreadcrumbItem>> GetCategoryBreadcrumbAsync(int categoryId)
        {
            var breadcrumb = new List<BreadcrumbItem>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // You might need a recursive query if you have parent-child categories
                // For now, simple implementation
                var query = @"
                    WITH CategoryCTE AS (
                        SELECT Id, Name, ParentId, 0 as Level
                        FROM Categories
                        WHERE Id = @CategoryId
                        UNION ALL
                        SELECT c.Id, c.Name, c.ParentId, Level + 1
                        FROM Categories c
                        INNER JOIN CategoryCTE cte ON c.Id = cte.ParentId
                    )
                    SELECT Id, Name, Level FROM CategoryCTE
                    ORDER BY Level DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryId", categoryId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    breadcrumb.Add(new BreadcrumbItem
                    {
                        Title = reader.GetString(1),
                        Url = $"/Products/Index/{reader.GetInt32(0)}",
                        IsActive = reader.GetInt32(2) == 0 // Last one is active
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category breadcrumb");
            }

            // Add Home if breadcrumb is empty
            if (!breadcrumb.Any())
            {
                breadcrumb.Insert(0, new BreadcrumbItem { Title = "Home", Url = "/" });
            }

            return breadcrumb;
        }

        private ProductViewModel MapToProductViewModel(SqlDataReader reader)
        {
            return new ProductViewModel
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                OldPrice = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                DiscountPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                ImageUrl = reader.IsDBNull(6) ? "/images/default-product.png" : reader.GetString(6),
                CategoryId = reader.GetInt32(7),
                Brand = reader.IsDBNull(8) ? null : reader.GetString(8),
                Rating = reader.IsDBNull(9) ? 0 : reader.GetDouble(9),
                ReviewCount = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                OrderCount = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                IsVerified = reader.IsDBNull(12) ? false : reader.GetBoolean(12),
                FreeShipping = reader.IsDBNull(13) ? false : reader.GetBoolean(13),
                CreatedAt = reader.GetDateTime(14),
                CategoryName = reader.IsDBNull(15) ? null : reader.GetString(15)
            };
        }
    }
}



/*using EcommerceFullstackDesign.Models;
using EcommerceFullstackDesign.Models.Interface;
using Microsoft.Data.SqlClient;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly string _connectionString;

        public ProductsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Product>> GetProductsAsync(int? categoryId = null, string? search = null, string? sortBy = null)
        {
            var products = new List<Product>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"SELECT Id, Name, Description, Price, ImageUrl, CategoryId 
                             FROM Products WHERE 1=1";

            var parameters = new List<SqlParameter>();

            if (categoryId.HasValue)
            {
                query += " AND CategoryId = @CategoryId";
                parameters.Add(new SqlParameter("@CategoryId", categoryId.Value));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (Name LIKE @Search OR Description LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", $"%{search}%"));
            }

            // Sorting
            query += sortBy switch
            {
                "price_low" => " ORDER BY Price ASC",
                "price_high" => " ORDER BY Price DESC",
                "newest" => " ORDER BY Id DESC",
                _ => " ORDER BY Name"
            };

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"SELECT Id, Name, Description, Price, ImageUrl, CategoryId 
                             FROM Products WHERE Id = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProduct(reader);
            }

            return null;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = new List<Product>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"SELECT Id, Name, Description, Price, ImageUrl, CategoryId 
                             FROM Products WHERE CategoryId = @CategoryId
                             ORDER BY Name";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 6)
        {
            var products = new List<Product>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = @"SELECT TOP (@Count) Id, Name, Description, Price, ImageUrl, CategoryId 
                             FROM Products 
                             WHERE CategoryId = @CategoryId AND Id != @ProductId
                             ORDER BY NEWID()"; // Random order

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Count", count);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<List<string>> GetBrandsAsync()
        {
            var brands = new List<string>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT DISTINCT Brand FROM Products WHERE Brand IS NOT NULL ORDER BY Brand";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                brands.Add(reader.GetString(0));
            }

            return brands;
        }

        public async Task<List<string>> GetFeaturesAsync()
        {
            // You can implement this based on your database structure
            return new List<string> { "Metallic", "8GB Ram", "Super power", "Plastic cover" };
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            return await GetProductsAsync(null, searchTerm, null);
        }

        private Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Price = reader.GetDecimal(3),
                ImageUrl = reader.IsDBNull(4) ? "/images/default.png" : reader.GetString(4),
                CategoryId = reader.GetInt32(5)
            };
        }
    }
}*/