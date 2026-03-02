using EcommerceFullstackDesign.Models;

namespace EcommerceFullstackDesign.ViewModels
{
    public class CartPageViewModel
    {
        public CartSummaryViewModel? CartSummary { get; set; }
        public List<Product>? RecommendedProducts { get; set; }

    }
}
