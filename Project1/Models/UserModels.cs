namespace Project1.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductCategory> ProductCategories { get; set; } =
            new List<ProductCategory>();
    }
}
