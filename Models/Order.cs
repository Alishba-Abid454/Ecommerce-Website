namespace EcommerceFullstackDesign.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingZipCode { get; set; }
        public string? ShippingCountry { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
