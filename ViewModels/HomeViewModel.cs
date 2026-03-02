using EcommerceFullstackDesign.Models;
using System.Reflection;

namespace EcommerceFullstackDesign.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Product> RecommendedItems { get; set; } = new();
        public List<Product> DealsAndOffers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Supplier> Suppliers { get; set; } = new();
        public Banner? HeroBanner { get; set; }
        public List<PromotionCard> PromotionCards { get; set; } = new();
        public Dictionary<string, List<Product>> ProductsByCategory { get; set; } = new();
        public int CartCount { get; set; }
        public string UserGreeting { get; set; } = "Hi, user";
        public string UserSubGreeting { get; set; } = "let's get started";
        public string UserAvatarUrl { get; set; } = "/images/avatar.jpg";
        public int? UserId { get; set; }
        /*        public List<Category> Categories { get; set; }
                public List<Product> RecommendedItems { get; set; }
                public string? UserGreeting { get; set; }
                public string? UserSubGreeting { get; set; }
                public string? UserAvatarUrl { get; set; }
                public Banner HeroBanner { get; set; }
                public List<PromotionCard> PromotionCards { get; set; }
                public int CartCount { get; set; }
                public int UserId { get; internal set; }*/
    }
}
