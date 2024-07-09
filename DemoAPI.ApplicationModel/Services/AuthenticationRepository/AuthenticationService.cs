using DemoAPI.ApplicationModel.DTO;
using DemoAPI.ApplicationModel.Services.AuthenticationRepository.Interface;
using DemoAPI.DataModel.AuthenticationModel;
using DemoAPI.DataModel.DataAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoAPI.ApplicationModel.Services.AuthenticationRepository
{
    public class AuthenticationService : IAuthenticationUser
    {
        private readonly AuthenticationContext _Context;
        private readonly IConfiguration _configuration;
        public AuthenticationService(AuthenticationContext Context, IConfiguration configuration)
        {
            _configuration = configuration;
            _Context = Context;
        }
        
        public async Task<int> RegisterUser(UserRegisterDto user)
        {
            User _user = new User();
            _user.Email= user.Email;
            _user.Password= user.Password;
            _user.AccessId= user.AccessId;
            int id = await _Context.Register(_user);   
            return id;
        }

        public async Task<User> ValidateUser(LoginDto login)
        {
            var user = await _Context.ValidateUser(login.Email,login.Password);
            return user;
        }

        public async Task<string> GenerateTocken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role,user.AccessId),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var token = new JwtSecurityToken
                (
                issuer: _configuration["jwt:Issuer"],
                audience: _configuration["jwt:Audience"],
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["jwt:Duation"]))
                );

           
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
