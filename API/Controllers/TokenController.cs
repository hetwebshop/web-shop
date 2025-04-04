using API.Data;
using API.DTOs;
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
        private readonly bool IsProductionEnvironment;

        public TokenController(DataContext dataContext, ITokenService tokenService, IConfiguration configuration)
        {
            this._dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _configuration = configuration;
            Environment = configuration.GetSection("Environment").Value;
            IsProductionEnvironment = configuration.GetSection("Environment").Value == "Production";
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
            // Find the refresh token in the RefreshTokens table
            var storedRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token");
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.Id == storedRefreshToken.UserId);
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            var newAccessToken = await _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            storedRefreshToken.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var newToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                UserAgent = storedRefreshToken.UserAgent, 
                IPAddress = storedRefreshToken.IPAddress,
                DeviceId = storedRefreshToken.DeviceId, 
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.RefreshTokens.Remove(storedRefreshToken);
            await _dbContext.RefreshTokens.AddAsync(newToken);
            await _dbContext.SaveChangesAsync();

            _tokenService.SetTokenInsideCookie(newAccessToken, newRefreshToken, HttpContext);

            return Ok();
        }

        //[HttpPost]
        //[Route("refresh")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
        //{
        //    if (tokenApiModel is null)
        //        return BadRequest("Invalid client request");
        //    string accessToken = tokenApiModel.AccessToken;
        //    string refreshToken = tokenApiModel.RefreshToken;
        //    var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        //    var username = principal.Identity.Name; //this is mapped to the Name claim by default
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.UserName == username);
        //    if (user == null)
        //        return BadRequest("User not found");

        //    // Find the refresh token in the RefreshTokens table based on the user and provided token
        //    var storedRefreshToken = await _dbContext.RefreshTokens
        //        .Where(rt => rt.UserId == user.Id && rt.Token == refreshToken)
        //        .FirstOrDefaultAsync();

        //    if (storedRefreshToken == null || storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
        //        return BadRequest("Invalid or expired refresh token");

        //    // Optionally, check if the DeviceId and UserAgent match (to make sure it’s from the same device)
        //    if (storedRefreshToken.DeviceId != tokenApiModel.DeviceId || storedRefreshToken.UserAgent != tokenApiModel.UserAgent)
        //    {
        //        return BadRequest("Invalid refresh token or device mismatch");
        //    }

        //    var newAccessToken = await _tokenService.CreateToken(user);
        //    var newRefreshToken = _tokenService.CreateRefreshToken();

        //    _dbContext.RefreshTokens.Remove(storedRefreshToken);

        //    // Create a new refresh token for the user
        //    var newToken = new RefreshToken
        //    {
        //        UserId = user.Id,
        //        Token = newRefreshToken,
        //        UserAgent = tokenApiModel.UserAgent,
        //        IPAddress = tokenApiModel.IPAddress,
        //        DeviceId = tokenApiModel.DeviceId,
        //        ExpiryDate = DateTime.UtcNow.AddDays(7),  // Set expiration time for the new refresh token
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _dbContext.RefreshTokens.AddAsync(newToken);
        //    await _dbContext.SaveChangesAsync();
        //    return Ok(new TokenApiModel()
        //    {
        //        AccessToken = newAccessToken,
        //        RefreshToken = newRefreshToken
        //    });
        //}

        //[HttpPost, Authorize]
        //[Route("revoke")]
        //public IActionResult Revoke()
        //{
        //    var username = User.Identity.Name;
        //    var user = _dbContext.Users.SingleOrDefault(u => u.UserName == username);
        //    if (user == null) return BadRequest();
        //    user.RefreshToken = null;
        //    _dbContext.SaveChanges();

        //    return NoContent();
        //}


        [HttpPost, Authorize]
        [Route("revoke-cookie")]
        public async Task<IActionResult> RevokeCookie([FromBody] RevokeDeviceDto revokeDeviceDto)
        {
            var username = User.Identity.Name;
            var user = _dbContext.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest("User not found");
            
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == user.Id && rt.DeviceId == revokeDeviceDto.DeviceId);

            if (refreshToken == null)
                return NotFound("Refresh token not found for this device");

            // Remove the refresh token for this specific device
            _dbContext.RefreshTokens.Remove(refreshToken);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Clear the cookies by setting their expiration to a past date
            Response.Cookies.Append("accessToken", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to a past date
                HttpOnly = true,
                Secure = true,
                SameSite =
#if DEBUG
                    SameSiteMode.None
#else
                        SameSiteMode.Lax,
                        Domain = ".poslovnioglasi.ba"
#endif
            });

            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to a past date
                HttpOnly = true,
                Secure = true,
                SameSite =
                #if DEBUG
                    SameSiteMode.None
#else
                        SameSiteMode.Lax,
                        Domain = ".poslovnioglasi.ba"
#endif
            });

            return NoContent();
        }
    }

    public class TokenApiModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public string UserAgent { get; set; }  // The User-Agent string from the HTTP request
        public string IPAddress { get; set; }  // The IP address from the HTTP request
        public string DeviceId { get; set; }   // A unique identifier for the device
    }

    public class RevokeDeviceDto
    {
        public string DeviceId { get; set; } // The unique identifier for the device
    }

}
