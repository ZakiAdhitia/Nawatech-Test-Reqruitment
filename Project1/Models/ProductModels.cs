using System.ComponentModel.DataAnnotations.Schema;

namespace Project1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required Guid UserId { get; set; }

        [ForeignKey("ProductCategory")]
        public int CategoryId { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User? User { get; set; }
        public ProductCategory? ProductCategory { get; set; }
    }
}
