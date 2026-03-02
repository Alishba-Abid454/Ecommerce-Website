namespace EcommerceFullstackDesign.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? ImageUrl { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonLink { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
