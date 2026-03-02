namespace EcommerceFullstackDesign.ViewModels
{
    public class CartSummaryViewModel
    {
        public List<CartItemViewModel> Items { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int ItemCount => Items?.Sum(x => x.Quantity) ?? 0;

    }
}
