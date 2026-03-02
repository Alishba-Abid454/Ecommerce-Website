using EcommerceFullstackDesign.ViewModels;

namespace EcommerceFullstackDesign.Models.Interface
{
    public interface ICartRepository
    {
        Task<List<CartItemViewModel>> GetCartItemsAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveItemAsync(int cartItemId);
        Task ClearCartAsync(int userId);
        Task<CartSummaryViewModel> GetCartSummaryAsync(int userId);
        Task<int> CreateOrderAsync(int userId);
        Task UpdateQuantityAsync(int cartItemId, int quantity);

    }
}
