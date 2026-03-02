namespace EcommerceFullstackDesign.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Website { get; set; }
        public string? FlagIcon { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
