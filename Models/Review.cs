namespace EcommerceFullstackDesign.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public bool IsApproved { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
