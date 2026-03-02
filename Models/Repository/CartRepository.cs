using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.Data.SqlClient;

namespace EcommerceFullstackDesign.Models.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(IConfiguration configuration, ILogger<CartRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            _logger = logger;
        }

        public async Task<List<CartItemViewModel>> GetCartItemsAsync(int userId)
        {
            var items = new List<CartItemViewModel>();

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"
                    SELECT c.Id, c.ProductId, p.Name, p.Description, 
                           ISNULL(p.ImageUrl, '/images/default.png') as ImageUrl, 
                           p.Price, c.Quantity
                    FROM CartItems c
                    INNER JOIN Products p ON c.ProductId = p.Id
                    WHERE c.UserId = @UserId
                    ORDER BY c.AddedAt DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(new CartItemViewModel
                    {
                        CartItemId = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        ImageUrl = reader.GetString(4),
                        Price = reader.GetDecimal(5),
                        Quantity = reader.GetInt32(6)
                    });
                }

                _logger.LogInformation($"Found {items.Count} items in cart for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items for user {UserId}", userId);
            }

            return items;
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // Check if product already in cart
                string checkQuery = @"
                    SELECT Id, Quantity FROM CartItems
                    WHERE UserId = @UserId AND ProductId = @ProductId";

                using SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                using SqlDataReader reader = await checkCmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int cartId = reader.GetInt32(0);
                    int existingQty = reader.GetInt32(1);
                    reader.Close();

                    // Update existing cart item
                    string updateQuery = @"
                        UPDATE CartItems 
                        SET Quantity = @Quantity, AddedAt = @AddedAt 
                        WHERE Id = @Id";

                    using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Quantity", existingQty + quantity);
                    updateCmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                    updateCmd.Parameters.AddWithValue("@Id", cartId);

                    await updateCmd.ExecuteNonQueryAsync();
                    _logger.LogInformation($"Updated cart item {cartId} with quantity {existingQty + quantity}");
                }
                else
                {
                    reader.Close();

                    // Insert new cart item
                    string insertQuery = @"
                        INSERT INTO CartItems (UserId, ProductId, Quantity, AddedAt)
                        VALUES (@UserId, @ProductId, @Quantity, @AddedAt)";

                    using SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);

                    await insertCmd.ExecuteNonQueryAsync();
                    _logger.LogInformation($"Added product {productId} to cart for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart for user {UserId}, product {ProductId}", userId, productId);
                throw;
            }
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = "DELETE FROM CartItems WHERE Id = @Id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", cartItemId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Successfully removed cart item {cartItemId}");
                }
                else
                {
                    _logger.LogWarning($"No item found with Id {cartItemId} to remove");
                }
                /*                _logger.LogInformation($"Removed cart item {cartItemId}, rows affected: {rowsAffected}");
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = "DELETE FROM CartItems WHERE UserId = @UserId";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation($"Cleared cart for user {userId}, removed {rowsAffected} items");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                throw;
            }
        }
        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"
            UPDATE CartItems 
            SET Quantity = @Quantity, AddedAt = @AddedAt 
            WHERE Id = @Id";

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@Id", cartItemId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation($"Updated cart item {cartItemId} quantity to {quantity}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} quantity", cartItemId);
                throw;
            }
        }
        /*
                public async Task UpdateQuantityAsync(int cartItemId, int quantity)
                {
                    try
                    {
                        using SqlConnection conn = new SqlConnection(_connectionString);
                        await conn.OpenAsync();

                        string query = @"
                            UPDATE CartItems 
                            SET Quantity = @Quantity, AddedAt = @AddedAt 
                            WHERE Id = @Id";

                        using SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Id", cartItemId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        _logger.LogInformation($"Updated cart item {cartItemId} quantity to {quantity}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating cart item {CartItemId} quantity", cartItemId);
                        throw;
                    }
                }*/

        public async Task<CartSummaryViewModel> GetCartSummaryAsync(int userId)
        {
            var items = await GetCartItemsAsync(userId);

            decimal subtotal = items.Sum(x => x.Price * x.Quantity);
            decimal discount = 0; // You can add discount logic here
            decimal tax = subtotal * 0.18m; // 18% GST

            return new CartSummaryViewModel
            {
                Items = items,
                Subtotal = subtotal,
                Discount = discount,
                Tax = tax,
                Total = subtotal - discount + tax
            };
        }

        public async Task<int> CreateOrderAsync(int userId)
        {
            try
            {
                // Get cart items
                var items = await GetCartItemsAsync(userId);

                if (!items.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                // Here you would:
                // 1. Create order in Orders table
                // 2. Add order items in OrderItems table
                // 3. Clear the cart

                // For now, just clear cart and return random order number
                await ClearCartAsync(userId);

                int orderNumber = new Random().Next(1000, 9999);
                _logger.LogInformation($"Created order #{orderNumber} for user {userId}");

                return orderNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                throw;
            }
        }
    }
}





/*using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

public class CartRepository : ICartRepository
{
    private readonly string _connectionString;

    public CartRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<CartItemViewModel>> GetCartItemsAsync(int userId)
    {
        var items = new List<CartItemViewModel>();

        using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string query = @"SELECT c.Id, c.ProductId, p.Name, p.Description, 
                                p.ImageUrl, p.Price, c.Quantity
                         FROM CartItems c
                         JOIN Products p ON c.ProductId = p.Id
                         WHERE c.UserId = @UserId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            items.Add(new CartItemViewModel
            {
                CartItemId = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                ImageUrl = reader.IsDBNull(4) ? "/images/default.png" : reader.GetString(4),
                Price = reader.GetDecimal(5),
                Quantity = reader.GetInt32(6)
            });
        }

        return items;
    }

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string checkQuery = @"SELECT Id, Quantity FROM CartItems
                              WHERE UserId=@UserId AND ProductId=@ProductId";

        using SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("@UserId", userId);
        checkCmd.Parameters.AddWithValue("@ProductId", productId);

        using SqlDataReader reader = await checkCmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int cartId = reader.GetInt32(0);
            int existingQty = reader.GetInt32(1);
            reader.Close();

            string updateQuery = @"UPDATE CartItems 
                                   SET Quantity=@Qty WHERE Id=@Id";

            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
            updateCmd.Parameters.AddWithValue("@Qty", existingQty + quantity);
            updateCmd.Parameters.AddWithValue("@Id", cartId);

            await updateCmd.ExecuteNonQueryAsync();
        }
        else
        {
            reader.Close();

            string insertQuery = @"INSERT INTO CartItems(UserId, ProductId, Quantity)
                                   VALUES(@UserId,@ProductId,@Quantity)";

            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@UserId", userId);
            insertCmd.Parameters.AddWithValue("@ProductId", productId);
            insertCmd.Parameters.AddWithValue("@Quantity", quantity);

            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    public async Task RemoveItemAsync(int cartItemId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string query = "DELETE FROM CartItems WHERE Id=@Id";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Id", cartItemId);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ClearCartAsync(int userId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string query = "DELETE FROM CartItems WHERE UserId=@UserId";

        using SqlCommand cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<CartSummaryViewModel> GetCartSummaryAsync(int userId)
    {
        var items = await GetCartItemsAsync(userId);

        decimal subtotal = items.Sum(x => x.Total);
        decimal discount = 0;
        decimal tax = 0;

        return new CartSummaryViewModel
        {
            Items = items,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            Total = subtotal - discount + tax
        };
    }

    public async Task<int> CreateOrderAsync(int userId)
    {
        // For now simple clear cart after checkout
        await ClearCartAsync(userId);
        return new Random().Next(1000, 9999);
    }
}*/