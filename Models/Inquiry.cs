namespace EcommerceFullstackDesign.Models
{
    public class Inquiry
    {
        public int Id { get; set; }
        public string Product { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
    }
}
