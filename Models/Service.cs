namespace EcommerceFullstackDesign.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
