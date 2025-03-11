using API.Data;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class TokenController : BaseController
    {
        private readonly DataContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly string Environment;
        private readonly IConfiguration _configuration;
        public TokenController(DataContext dataContext, ITokenService tokenService, IConfiguration configuration)
        {
            this._dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _configuration = configuration;
            Environment = configuration.GetSection("Environment").Value;
        }
        [HttpPost]
        [Route("refresh-cookie")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshCookie()
        {
            HttpContext.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token not found");
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.RefreshToken == refreshToken);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");
            var newAccessToken = await _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);//SAME VALUE IS IN SetTokenInsideCookie
            await _dbContext.SaveChangesAsync();

            _tokenService.SetTokenInsideCookie(newAccessToken, newRefreshToken, HttpContext);

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = _dbContext.Users.FirstOrDefault(r => r.UserName == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest("Invalid client request");
            var newAccessToken = await _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _dbContext.SaveChanges();
            return Ok(new TokenApiModel()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = _dbContext.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            _dbContext.SaveChanges();

            return NoContent();
        }


        [HttpPost, Authorize]
        [Route("revoke-cookie")]
        public async Task<IActionResult> RevokeCookie()
        {
            var username = User.Identity.Name;
            var user = _dbContext.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            await _dbContext.SaveChangesAsync();

            // Clear the cookies by setting their expiration to a past date
            Response.Cookies.Append("accessToken", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to a past date
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".azurewebsites.net",
            });

            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to a past date
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Domain = ".azurewebsites.net",
            });

            return NoContent();
        }
    }

    public class TokenApiModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
