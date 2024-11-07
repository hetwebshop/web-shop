using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IUnitOfWork uow, IMapper mapper, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _uow = uow;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            if (await UserNameExist(registerDto.UserName))
                return BadRequest("Korisničko ime je zauzeto.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Email je već registrovan.");

            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            user.Photo = new Photo();
            user.IsApproved = false;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            result = await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationUrl = "http://localhost:4200/confirm-email?userId=" + user.Id + "&token=" + Uri.EscapeDataString(emailToken);

            // Slanje verifikacionog emaila (ovdje koristite svoju email uslugu)
            await _emailService.SendEmailAsync("hetcompany24@gmail.com",
                "Verifikujte svoju email adresu",
                $"Molimo potvrdite svoj email klikom na sljedeći link: <a href='{verificationUrl}'>Verifikuj Email</a>");

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                UserName = user.UserName,
                PhotoUrl = user.Photo?.Url,
                Email = user.Email
            };
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("Neispravan zahtjev");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("Korisnik nije pronađen");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest("Potvrda emaila nije uspjela");

            // Nakon što je email potvrđen, možete aktivirati korisnički račun
            user.IsApproved = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Preusmjeri na stranicu za potvrdu ili prijavu
            return Ok();  // Ili na bilo koju drugu stranicu na koju želite preusmjeriti
        }


        [HttpPost("register-company")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> RegisterCompany(CompanyRegisterDto registerDto)
        {
            if (await UserNameExist(registerDto.UserName))
                return BadRequest("Email nije dostupan.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Email nije dostupan.");

            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            user.Photo = new Photo();
            user.IsApproved = false;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            result = await _userManager.AddToRoleAsync(user, RoleType.Company.ToString());
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            //var token = await _tokenService.CreateToken(user);

            try
            {
                await _emailService.SendEmailAsync("hetcompany24@gmail.com",
                                   "Registracija uspješna - čekate verifikaciju od našeg admina",
                                   $"Dragi {user.Company.CompanyName},\n\nVaš korisnički račun za kompaniju je uspješno registrovan, ali čeka odobrenje od strane administratora.\n\nHvala što ste se registrovali!");

            }
            catch (Exception ex)
            {
                throw;
            }
            return new UserDto
            {
                CompanyName = user.Company.CompanyName,
                CityId = user.CityId,
                UserName = user.UserName,
                //Token = token,
                PhotoUrl = user.Photo?.Url,
                Email = user.Email,
                IsCompany = true,
                CompanyAddress = user.Company.Address,
            };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .Include(u => u.UserEducations)
                .Include(u => u.UserRoles)
                .SingleOrDefaultAsync(u => u.UserName == loginDto.UserNameOrEmail.ToLower() || u.Email == loginDto.UserNameOrEmail);

            if (user == null) return BadRequest("Pogrešan korisnik");

            if (!user.IsApproved)
            {
                return BadRequest("Račun vaše kompanije čeka odobrenje od strane admina.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded && loginDto.UserNameOrEmail != Constants.TestUser) return BadRequest("Pogrešan email ili password.");

            var token = await _tokenService.CreateToken(user);

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                UserName = user.UserName,
                Token = token,
                PhotoUrl = user.Photo?.Url,
                Email = user.Email,
                IsCompany = user.IsCompany,
                Credits = user.Credits
            };
        }

        [HttpGet("token-update")]
        public async Task<ActionResult<UserDto>> GetUpdatedToken()
        {
            var id = HttpContext.User.GetUserId();
            var user = await _userManager.Users
                .Include(u => u.Photo)
                .SingleAsync(u => u.Id == id);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = await _tokenService.CreateToken(user, accessToken);

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                UserName = user.UserName,
                Token = token,
                PhotoUrl = user.Photo?.Url,
                Email = user.Email,
                IsCompany = user.IsCompany
            };
        }

        [HttpGet("{userName}")]
        [AllowAnonymous]
        public async Task<bool> UserNameExist(string userName)
        {
            return await _uow.UserRepository.UserExist(userName);
        }

        [HttpGet("user/{userName}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetUserInfo(string userName)
        {
            return Ok(await _uow.UserRepository.GetUserInfo(userName));
        }

        [HttpGet("profile")]
        public async Task<ActionResult> GetUserProfile()
        {
            var id = HttpContext.User.GetUserId();
            var up = await _uow.UserRepository.GetProfile(id);
            return Ok(up);
        }

        [HttpPost("profile")]
        public async Task<ActionResult<UserProfileDto>> UpdateUserProfile([FromForm]UserProfileDto profileDto)
        {
            profileDto.Id = HttpContext.User.GetUserId();

            if (await _userManager.Users.AnyAsync(u => u.Id != profileDto.Id &&
                                                       u.NormalizedUserName == profileDto.UserName.ToUpper()))
                return BadRequest("Username is taken.");
            if (await _userManager.Users.AnyAsync(u => u.Id != profileDto.Id &&
                                                       u.NormalizedEmail == profileDto.Email.ToUpper()))
                return BadRequest("Email is already registered.");
            if (HttpContext.User.GetUserName() == Constants.TestUser && profileDto.UserName.ToLower() != Constants.TestUser)
                return BadRequest("Test User cannot change username.");

            var user = await _userManager.Users.SingleAsync(u => u.Id == profileDto.Id);
            if (profileDto.CvFile != null)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }
                var uniqueFileName = Helpers.GetUniqueFileName(uploadsDir, profileDto.CvFile.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileDto.CvFile.CopyToAsync(stream);
                }
                profileDto.CvFilePath = filePath;
            }

            _mapper.Map(profileDto, user);
            //user.CvFilePath = filePath;

            bool deleteUserEducations = await _uow.UserRepository.RemoveAllUserEducationsAsync(profileDto.Id);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to update.");

            _mapper.Map(user, profileDto);
            return profileDto;
        }

        [HttpPost("company-profile")]
        public async Task<ActionResult<UserProfileDto>> UpdateCompanyProfile([FromBody] CompanyProfileDto companyDto)
        {
            companyDto.UserId = HttpContext.User.GetUserId();

            if (await _userManager.Users.AnyAsync(u => u.Id != companyDto.UserId &&
                                                       u.NormalizedUserName == companyDto.UserName.ToUpper()))
                return BadRequest("Username is taken.");
            if (await _userManager.Users.AnyAsync(u => u.Id != companyDto.UserId &&
                                                       u.NormalizedEmail == companyDto.UserName.ToUpper()))
                return BadRequest("Email is already registered.");
            if (HttpContext.User.GetUserName() == Constants.TestUser && companyDto.UserName.ToLower() != Constants.TestUser)
                return BadRequest("Test User cannot change username.");

            var user = await _userManager.Users.Include(r => r.Company).SingleAsync(u => u.Id == companyDto.UserId);
            user.Company.Address = companyDto.CompanyAddress;
            user.Company.PhoneNumber = companyDto.CompanyPhone;
            user.Company.AboutUs = companyDto.AboutCompany;
            user.Company.CompanyName = companyDto.CompanyName;
            user.Company.Email = companyDto.UserName;
            user.Company.CityId = companyDto.CityId;
            user.Email = companyDto.UserName;
            user.UserName = companyDto.UserName;
            user.CityId = companyDto.CityId;
            user.PhoneNumber = companyDto.CompanyPhone;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to update.");

            UserProfileDto userProfile = _mapper.Map<UserProfileDto>(user);
            return userProfile;
        }

        [HttpPost("change-photo")]
        public async Task<ActionResult> UpdateUserPhoto([FromForm] PhotoUpdateDto updateDto)
        {
            var id = HttpContext.User.GetUserId();
            if (updateDto.Remove)
            {
                await _uow.UserRepository.DeleteUserPhoto(id);
                return await _uow.SaveChanges()
                    ? Ok(new { PhotoUrl = "" })
                    : BadRequest("Failed to update photo.");
            }

            var url = await _uow.UserRepository.UpdateUserPhoto(updateDto.File, id);
            return Ok(new { PhotoUrl = url });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("Korisnik sa proslijeđenim emailom ne postoji.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(user.Email);
            var resetLink = $"http://localhost:4200/reset-password?email={encodedEmail}&token={encodedToken}";
            var emailBody = $"<p>Za promjenu lozinke molimo vas da otvorite sljedeći link:</p><a href='{resetLink}'>Promjena lozinke</a>";

            await _emailService.SendEmailAsync("hetcompany24@gmail.com", "Zahtjev za promjenom lozinke", emailBody);

            return Ok();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Zahtjev za promjenom lozinke nije ispravan.");
            }

            var decodedToken = HttpUtility.UrlDecode(model.Token);

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (!resetResult.Succeeded)
            {
                return BadRequest(resetResult.Errors.Select(e => e.Description));
            }

            return Ok();
        }

    }
}