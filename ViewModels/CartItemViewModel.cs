namespace EcommerceFullstackDesign.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal Total => Price * Quantity;
    }
}
