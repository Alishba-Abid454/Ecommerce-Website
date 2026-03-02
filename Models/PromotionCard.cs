namespace EcommerceFullstackDesign.Models
{
    public class PromotionCard
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonLink { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
