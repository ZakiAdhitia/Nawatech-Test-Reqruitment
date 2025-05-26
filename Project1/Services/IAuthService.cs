using System.Threading.Tasks;
using Project1.Data.Dtos;

namespace Project1.Services
{
    public interface IAuthService
    {
        Task<string> Register(UserRegister dto, string? imagePath);
        Task<(bool IsSuccess, string Message, string? Token)> Login(UserLogin dto);
    }
}
