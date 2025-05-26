namespace Project1.Data.Dtos
{
    public class UserUpdate
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public IFormFile? Image { get; set; }
    }
}
