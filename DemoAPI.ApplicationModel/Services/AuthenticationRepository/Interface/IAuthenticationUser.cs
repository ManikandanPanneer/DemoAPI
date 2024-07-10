using DemoAPI.ApplicationModel.DTO;
using DemoAPI.DataModel.AuthenticationModel;

namespace DemoAPI.ApplicationModel.Services.AuthenticationRepository.Interface
{
    public interface IAuthenticationUser
    {
        Task<int> RegisterUser(UserRegisterDto user);
        Task<User> ValidateUser(LoginDto login);
        Task<string> GenerateToken(User user);
    }
}
