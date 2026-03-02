namespace EcommerceFullstackDesign.Models
{
    public class FAQItem
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
