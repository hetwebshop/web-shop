using System.Security.Claims;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Http;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
        void SetTokenInsideCookie(string accessToken, string refreshToken, HttpContext context);
    }
}