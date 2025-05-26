namespace Project1.Data.Dtos
{
    public class UserRegister
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public IFormFile? Image { get; set; }
    }
}
