namespace Project1.Data.Dtos
{
    public class ProductCreate
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
