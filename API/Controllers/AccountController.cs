using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.Entities.Notification;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.Services;
using AutoMapper;
using Ganss.Xss;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private readonly IConfiguration configuration;
        private readonly string verificationEmailBaseAddress;
        private readonly IBlobStorageService _blobStorageService;
        private readonly DataContext _dbContext;
        private readonly string UIBaseUrl;
        private readonly string SupportEmail;
        private readonly string SupportPhone;
        private readonly string Environment;
        private readonly RecaptchaService _recaptchaService;
        private readonly ILogger<AccountController> _logger;
        private readonly bool addFreeUserCreditsEnabled;
        private readonly bool addFreeCompanyCreditsEnabled;
        private readonly int FreeUserCredits;
        private readonly int FreeCompanyCredits;
        private readonly string BackendUrl;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration,
            IUnitOfWork uow, IMapper mapper, ITokenService tokenService, IEmailService emailService, IBlobStorageService blobStorageService, DataContext dbContext, RecaptchaService recaptchaService, ILogger<AccountController> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _uow = uow;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            this.configuration = configuration;
            verificationEmailBaseAddress = configuration.GetSection("VerificationEmailBaseAddress").Value;
            _blobStorageService = blobStorageService;
            _dbContext = dbContext;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            SupportEmail = configuration.GetSection("SupportEmail").Value;
            SupportPhone = configuration.GetSection("SupportPhoneNumber").Value;
            Environment = configuration.GetSection("Environment").Value;
            _recaptchaService = recaptchaService;
            addFreeUserCreditsEnabled = bool.Parse(configuration.GetSection("AddFreeUserCreditsEnabled").Value);
            addFreeCompanyCreditsEnabled = bool.Parse(configuration.GetSection("AddFreeCompanyCreditsEnabled").Value);
            FreeCompanyCredits = int.Parse(configuration.GetSection("FreeCompanyCredits").Value);
            FreeUserCredits = int.Parse(configuration.GetSection("FreeUserCredits").Value);
            BackendUrl = configuration.GetSection("BackendUrl").Value;
        }

        private bool IsUserAdult(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        private async Task AddDefaultNotificationSettingsAsync(int userId)
        {
            var notificationsSettings = new List<UserNotificationSettings>
            {
                new UserNotificationSettings
                {
                    UserId = userId,
                    NotificationType = UserNotificationType.NewInterestingCompanyAdInApp,
                    IsEnabled = true
                },
                new UserNotificationSettings
                {
                    UserId = userId,
                    NotificationType = UserNotificationType.NewInterestingCompanyAdEmail,
                    IsEnabled = true
                }
            };

            _dbContext.UserNotificationSettings.AddRange(notificationsSettings);
            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            registerDto.UserName = registerDto.Email;
            if(registerDto.TermsAccepted == false)
            {
                return BadRequest("Morate prihvatiti uslove korištenja i politiku bezbjednosti.");
            }
            if (string.IsNullOrEmpty(registerDto.CaptchaToken))
            {
                return BadRequest("Morate prihvatiti reCAPTCHA.");
            }
            try
            {
                var isCaptchaValid = await _recaptchaService.VerifyCaptchaAsync(registerDto.CaptchaToken);
                if (!isCaptchaValid)
                {
                    return BadRequest("Neispravan reCAPTCHA odgovor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Captcha is not available.");
                return BadRequest($"reCaptcha nije dostupna. Molimo vas da kontaktirate podrsku na {SupportEmail} ili pozovite {SupportPhone}");
            }

            //if (!IsUserAdult(registerDto.DateOfBirth))
            //    return BadRequest("Morate imati najmanje 18 godina da biste koristili aplikaciju.");

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper());
            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                {
                    _logger.LogInformation("User already exists in database. Move user to login page: {Email}", existingUser.Email);
                    // Korisnik već postoji i njegov email je potvrđen
                    return BadRequest("Korisnik sa ovim emailom već postoji. Molimo vas da se prijavite na sistem. Ukoliko imate poteškoća, kontaktirajte nas na podrska@poslovnioglasi.ba.");
                }

                _logger.LogInformation("User already exists in database. We are sending new verification email to user: {Email}", existingUser.Email);

                // Resend confirmation email
                var newEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
                var newVerificationUrl = $"{verificationEmailBaseAddress}confirm-email?userId=" + existingUser.Id + "&token=" + Uri.EscapeDataString(newEmailToken);

                string userExistMessageBody = $@"
    <p style='color: black;'>Već ste se registrovali. Molimo vas da potvrdite vašu email adresu klikom na link ispod.</p>
    <p style='text-align: start;'>
        <a href='{newVerificationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Potvrdi email adresu</a>
    </p>
    <p>Ako dugme ne radi, kopirajte ovaj link i otvorite ga u pretraživaču:<br>{newVerificationUrl}</p>";

                string userExistSubject = "Potvrdite svoj račun na platformi Poslovnioglasi";
                string userExistEmailHtml = EmailTemplateHelper.GenerateEmailTemplate(userExistSubject, userExistMessageBody, configuration);

                try
                {
                    await _emailService.SendEmailWithTemplateAsync(existingUser.Email, userExistSubject, userExistEmailHtml);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to resend verification email to {Email}", existingUser.Email);
                }

                return new UserDto
                {
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    CityId = existingUser.CityId,
                    UserName = existingUser.UserName,
                    Email = existingUser.Email
                };
            }


            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            user.IsApproved = false;
            user.TermsAccepted = registerDto.TermsAccepted;
            user.EmailConfirmed = false;

            if(addFreeUserCreditsEnabled)
                user.Credits = FreeUserCredits;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());
            if (!result.Succeeded)
            {
                _logger.LogWarning("Greška pri kreiranju korisnika: {Errors}", result.Errors.ToStringError());
                return BadRequest(result.Errors.ToStringError());
            }
            var roleResult = await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Greška pri dodavanju korisničke uloge: {Errors}", roleResult.Errors.ToStringError());
                return BadRequest(roleResult.Errors.ToStringError());
            }

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationUrl = $"{verificationEmailBaseAddress}confirm-email?userId=" + user.Id + "&token=" + Uri.EscapeDataString(emailToken);

            await AddDefaultNotificationSettingsAsync(user.Id);


            // Slanje verifikacionog emaila (ovdje koristite svoju email uslugu)
            string messageBody = $@"
            <p style='color: black;'>Hvala vam što ste se registrirali na našu platformu. Molimo vas da potvrdite vašu email adresu kako biste aktivirali svoj račun.</p>
            <p style='text-align: start;'>
                <a href='{verificationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Potvrdi email adresu</a>
            </p>
            <p>Ako dugme ne radi, kopirajte ovaj link u pretraživač:<br>{verificationUrl}</p>";
            string subject = "Potvrdite svoj račun na platformi Poslovnioglasi";
            string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);

            try
            {

                Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailWithTemplateAsync(user.Email, subject, emailHtml);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neuspjelo slanje verifikacionog emaila korisniku {Email}", user.Email);
                return BadRequest($"Registracija uspješna, ali nismo uspjeli poslati verifikacioni email. Molimo vas da kontaktirate podrsku na {SupportEmail} ili pozovite {SupportPhone}");
            }

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        [HttpGet("test-email-template")]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmailTemplate()
        {
            string verificationUrl = "http://localhost:3000/";
            string subject = "🎉 Čestitamo! Osvojili ste besplatnu objavu oglasa!";

            string messageBody = $@"
    <p>Poštovani,</p>

    <p>Hvala što ste se prijavili za rani pristup na platformi Poslovnioglasi! 🚀<br>
    Uz to, čestitamo! 🎁 Kao jedna od prvih 30 kompanija, osvojili ste besplatnu objavu oglasa!</p>

    <p>
        🔜 Platforma kreće za samo nekoliko dana! 🔜
    </p>

    <p>Čim bude dostupna, dobit ćete obavještenje i moći ćete odmah da objavite svoj prvi oglas i zaprimite prve aplikacije od kvalifikovanih kandidata – brzo, jednostavno i efikasno!</p>

    <p>Šta dalje?</p>
    <ul>
        <li>✅ Očekujte našu poruku čim platforma bude aktivna.</li>
        <li>✅ U međuvremenu, pratite nas na Facebook stranici <a href='https://www.facebook.com/profile.php?id=61573110207228' target='_blank'>poslovnioglasi.ba</a>, Instagram stranici <a href='https://www.instagram.com/poslovnioglasi_ba/?next=%2F' target='_blank'>@poslovnioglasi_ba</a> i na našem web sajtu <a href='https://poslovnioglasi.ba' target='_blank'>poslovnioglasi.ba</a> za ekskluzivne novosti.</li>
    </ul>
    <p style='font-weight: bold;'>Radujemo se što ste dio ove revolucije u zapošljavanju! 🎯</p>
    ";


            string messageBody2 = $@"
    <p>Pozdrav dragi korisniče,</p>

    <p>Hvala što ste se prijavili za rani pristup na platformi Poslovnioglasi! 🚀<br>
    Uz to, čestitamo! 🎁 Kao jedan od prvih 100 korisnika, osvojili ste besplatnu objavu oglasa!</p>

    <p>
        🔜 Platforma kreće za samo nekoliko dana! 🔜
    </p>

    <p>Čim bude dostupna, dobit ćete obavještenje i moći ćete odmah da objavite oglas ili aplicirate na poslove – brzo, jednostavno i efikasno!</p>

    <p>Šta dalje?</p>
    <ul>
        <li>✅ Očekujte našu poruku čim platforma bude aktivna.</li>
        <li>✅ U međuvremenu, pratite nas na Facebook stranici <a href='https://www.facebook.com/profile.php?id=61573110207228' target='_blank'>poslovnioglasi.ba</a>, Instagram stranici <a href='https://www.instagram.com/poslovnioglasi_ba/?next=%2F' target='_blank'>@poslovnioglasi_ba</a> i na našem web sajtu <a href='https://poslovnioglasi.ba' target='_blank'>poslovnioglasi.ba</a> za ekskluzivne novosti.</li>
    </ul>
    <p style='font-weight: bold;'>Radujemo se što ste dio ove revolucije u zapošljavanju! 🎯</p>
    ";

            string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);

            // Send the email
            await _emailService.SendEmailWithTemplateAsync(
                "emindukic123@gmail.com",
                subject,
                emailHtml
            );
            return Ok();
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                _logger.LogInformation($"Starting confirm email, {userId}");
                if (userId == null || token == null)
                    return BadRequest("Neispravan zahtjev");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return BadRequest("Korisnik nije pronađen");

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    _logger.LogError($"There was an issue while trying to confirm email: User: {userId}, Token: {token}");
                    return BadRequest("Potvrda emaila nije uspjela");
                }

                // Nakon što je email potvrđen, možete aktivirati korisnički račun
                user.IsApproved = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                // Preusmjeri na stranicu za potvrdu ili prijavu
                return Ok();  // Ili na bilo koju drugu stranicu na koju želite preusmjeriti
            }
            catch(Exception ex)
            {
                _logger.LogError($"There was an issue in confirm email: {ex.Message}");
                return BadRequest("Došlo je do greške, pokušajte ponovo");
            }
        }


        [HttpPost("register-company")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> RegisterCompany([FromForm] CompanyRegisterDto registerDto)
        {
            registerDto.UserName = registerDto.Email;
            if (registerDto.TermsAccepted == false)
            {
                return BadRequest("Morate prihvatiti uslove korištenja i politiku bezbjednosti.");
            }
            if (await UserNameExist(registerDto.UserName))
                return BadRequest("Registracija nije uspjela. Provjerite unesene podatke.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Registracija nije uspjela. Provjerite unesene podatke.");
            if (string.IsNullOrEmpty(registerDto.CaptchaToken))
            {
                return BadRequest("Morate prihvatiti reCAPTCHA.");
            }
            try
            {
                var isCaptchaValid = await _recaptchaService.VerifyCaptchaAsync(registerDto.CaptchaToken);
                if (!isCaptchaValid)
                {
                    return BadRequest("Neispravan reCAPTCHA odgovor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Captcha is not available.");
                return BadRequest($"reCaptcha nije dostupna. Molimo vas da kontaktirate podrsku na {SupportEmail} ili pozovite {SupportPhone}");
            }
            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            
            user.IsApproved = false;
            user.EmailConfirmed = false;

            user.TermsAccepted = registerDto.TermsAccepted;
            if (addFreeCompanyCreditsEnabled)
                user.Credits = FreeCompanyCredits;
            if (registerDto.Photo != null)
            {
                if (!FileHelper.IsValidImage(registerDto.Photo))
                    return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Greška pri kreiranju korisnika: {Errors}", result.Errors.ToStringError());
                return BadRequest(result.Errors.ToStringError());
            }

            if (registerDto.Photo != null)
            {
                try
                {
                    var fileUrl = await _blobStorageService.UploadFileAsync(registerDto.Photo, user.Id);
                    var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                    user.PhotoUrl = decodedFileUrl;
                    user.Company.PhotoUrl = decodedFileUrl;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška prilikom upload-a slike na blob storage.");
                    return BadRequest("Došlo je do greške prilikom upload-a slike. Molimo pokušajte ponovo.");
                }
            }


            var roleResult = await _userManager.AddToRoleAsync(user, RoleType.Company.ToString());
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Greška pri dodavanju uloge kompanije: {Errors}", roleResult.Errors.ToStringError());
                return BadRequest(roleResult.Errors.ToStringError());
            }

            var notificationPreferences = new List<CompanyNotificationPreferences>
            {
                new() { UserId = user.Id, NotificationType = CompanyNotificationType.newApplicantInApp, IsEnabled = true },
                new() { UserId = user.Id, NotificationType = CompanyNotificationType.newApplicantEmail, IsEnabled = true },
                new() { UserId = user.Id, NotificationType = CompanyNotificationType.newInterestingUserAdInApp, IsEnabled = false },
                new() { UserId = user.Id, NotificationType = CompanyNotificationType.newInsterestingUserAdEmail, IsEnabled = false }
            };
            _dbContext.CompanyNotificationPreferences.AddRange(notificationPreferences);
            await _dbContext.SaveChangesAsync();
            //var token = await _tokenService.CreateToken(user);

            try
            {
                string messageBody = $@"
                <p style='color: black;'>Dragi <strong>{user.Company.CompanyName}</strong>,</p>
                <p style='color: black;'>Hvala što ste se registrovali na našoj platformi! Vaš korisnički račun je uspješno kreiran, međutim, još uvijek čeka odobrenje od strane administratora. Nakon verifikacije, moći ćete pristupiti svim funkcijama i resursima.</p>
                <p style='color: black;'>Vaš korisnički račun će biti aktiviran čim administrator izvrši verifikaciju.</p>
                <p style='color: black;'>Zahvaljujemo na strpljenju, i radujemo se što ćete postati dio naše zajednice!</p>";

                string subject = "Registracija uspješna - Čekate Verifikaciju";
                string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);



                Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendEmailWithTemplateAsync(
                            user.Email,
                            subject,
                            emailHtml
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
                    }
                });

                //Notify admin
                string approvalUrl = $"{BackendUrl}/account/approvecompany/{user.Id}";
                string emailBody = $@"
                <p>Dobili ste novi zahtjev za registraciju od kompanije:</p>
                <ul>
                    <li>Email: {registerDto.Email}</li>
                    <li>Adresa: {registerDto.Address}</li>
                    <li>Naziv kompanije: {registerDto.CompanyName}</li>
                    <li>Telefon: {registerDto.PhoneNumber}</li>
                </ul>
                <p>
                    <a href='{approvalUrl}' style='
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #66023C;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 5px;
                        font-weight: bold;
                    '>Odobri korisnika</a>
                </p>";
                _emailService.SendEmailWithTemplateAsync(configuration.GetSection("AdminRecipientEmailAddress").Value, "Registracija kompanije - novi zahtjev", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slanje email poruka nije uspjelo za korisnika {Email}.", user.Email);
            }
            return new UserDto
            {
                CompanyName = user.Company.CompanyName,
                CityId = user.CityId,
                UserName = user.UserName,
                //Token = token,
                PhotoUrl = user.PhotoUrl,
                Email = user.Email,
                CompanyAddress = user.Company.Address,
            };
        }

        [HttpGet("approvecompany/{id}")]
        [EnableCors("PublicCors")]
        [AllowAnonymous]
        public async Task<IActionResult> ApproveCompany(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(r => r.Id == id);
            if(user != null)
            {
                user.IsApproved = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var username = User.Identity.Name;
            var user = _dbContext.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpGet("me-info")]
        [Authorize]
        public async Task<IActionResult> GetMyInfoDetails()
        {
            var username = User.Identity.Name;
            var user = await FetchUserWithIncludesAsync(null, username);
            if (user == null)
            {
                return Unauthorized();
            }
            var dto = ConvertUserToUserDto(user);
            return Ok(dto);
        }

        [HttpPost("login-cookie")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> LoginCookie(LoginDto loginDto)
        {
            var user = await FetchUserWithIncludesAsync(null, loginDto.UserNameOrEmail);

            if (user == null) return BadRequest("Korisnik sa unešenom email adresom ne postoji.");

            if (!user.IsApproved)
            {
                if(user.IsCompany)
                    return BadRequest("Račun vaše kompanije čeka odobrenje od strane admina.");
                else
                    return BadRequest("Molimo vas da potvrdite email koji ste koristili prilikom registracije profila.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded && loginDto.UserNameOrEmail != API.Helpers.Constants.TestUser) return BadRequest("Pogrešan email ili password.");

            var token = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.LastActive = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _tokenService.SetTokenInsideCookie(token, refreshToken, HttpContext);

            loginDto.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var refreshTokenObj = new RefreshToken()
            {
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IPAddress = loginDto.IPAddress,
                UserAgent = loginDto.UserAgent,
                DeviceId = loginDto.DeviceId
            };
            var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == user.Id && r.DeviceId == loginDto.DeviceId);
            if (existingToken != null)
                _dbContext.RefreshTokens.Remove(existingToken);
            await _dbContext.RefreshTokens.AddAsync(refreshTokenObj);
            await _dbContext.SaveChangesAsync();

            var dto = ConvertUserToUserDto(user);

            return Ok(dto);
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GoogleLogin(GoogleLoginDto dto)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { configuration["Authentication:Google:ClientId"] } // Same as in frontend
                });
            }
            catch
            {
                return BadRequest("Invalid Google token.");
            }

            var user = await FetchUserWithIncludesAsync(null, payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailConfirmed = true,
                    IsApproved = true,
                };

                if (addFreeUserCreditsEnabled)
                    user.Credits = FreeUserCredits;

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Dogodila se greška prilikom kreiranja korisnika putem Googla.");
                    return BadRequest("Dogodila se greška prilikom kreiranja korisnika putem Googla.");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Greška pri dodavanju korisničke uloge: {Errors}", roleResult.Errors.ToStringError());
                    return BadRequest(roleResult.Errors.ToStringError());
                }
            }

            var token = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();
            _tokenService.SetTokenInsideCookie(token, refreshToken, HttpContext);

            user.LastActive = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var refreshTokenObj = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IPAddress = dto.IPAddress,
                UserAgent = dto.UserAgent,
                DeviceId = dto.DeviceId
            };

            var existingToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == user.Id && r.DeviceId == dto.DeviceId);
            if (existingToken != null)
                _dbContext.RefreshTokens.Remove(existingToken);
            await _dbContext.RefreshTokens.AddAsync(refreshTokenObj);
            await _dbContext.SaveChangesAsync();

            var userDto = ConvertUserToUserDto(user);
            return Ok(userDto);
        }

        //[HttpPost("login")]
        //[AllowAnonymous]
        //public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        //{
        //    var user = await FetchUserWithIncludesAsync(null, loginDto.UserNameOrEmail);

        //    if (user == null) return BadRequest("Korisnik sa unešenom email adresom ne postoji.");

        //    if (!user.IsApproved)
        //    {
        //        return BadRequest("Račun vaše kompanije čeka odobrenje od strane admina.");
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        //    if (!result.Succeeded && loginDto.UserNameOrEmail != API.Helpers.Constants.TestUser) return BadRequest("Pogrešan email ili password.");

        //    var token = await _tokenService.CreateToken(user);
        //    var refreshToken = _tokenService.CreateRefreshToken();

        //    user.LastActive = DateTime.UtcNow;
        //    user.RefreshToken = refreshToken;
        //    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        //    await _userManager.UpdateAsync(user);

        //    var dto = ConvertUserToUserDto(user);
        //    dto.AccessToken = token;
        //    dto.RefreshToken = refreshToken;

        //    return Ok(dto);
        //}

        [HttpGet("role-and-credits")]
        public async Task<ActionResult<UserDto>> GetUserRoleAndCredits()
        {
            var userId = HttpContext.User.GetUserId();

            // Fetch the user with roles in a single query and ensure the user exists and has roles
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Ensure the user has roles assigned
            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole == null)
            {
                return Unauthorized("User has no assigned roles.");
            }

            return Ok(new
            {
                Role = userRole.Role.Name,
                Credits = user.Credits
            });
        }

        [HttpGet("{userName}")]
        [AllowAnonymous]
        public async Task<bool> UserNameExist(string userName)
        {
            return await _uow.UserRepository.UserExist(userName);
        }

        [HttpPost("coverletter")]
        public async Task<ActionResult<UserDto>> UpdateUserCoverLetter([FromBody] UpdateUserCoverLetterRequest req)
        {
            try
            {
                var id = HttpContext.User.GetUserId();
                var user = await FetchUserWithIncludesAsync(id);

                if (user == null)
                {
                    return Unauthorized("Nemate pravo pristupa");
                }
                var sanitizer = new HtmlSanitizer();
                string sanitizedCoverLetter = sanitizer.Sanitize(req.Coverletter);

                user.Coverletter = sanitizedCoverLetter;

                await _dbContext.SaveChangesAsync();

                var dto = ConvertUserToUserDto(user);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the cover letter", error = ex.Message });
            }
        }

        [HttpPost("demo-request-private")]
        public async Task<ActionResult> SubmitDemoRequestPrivate([FromBody] DemoRequestBody req)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Nemate pravo pristupa");
            return await CheckAndCreateDemoRequest(req);
        }

        [HttpPost("demo-request")]
        [AllowAnonymous]
        public async Task<ActionResult> SubmitDemoRequest([FromBody] DemoRequestBody req)
        {
            if (string.IsNullOrEmpty(req.CaptchaToken))
            {
                return BadRequest("Morate prihvatiti reCAPTCHA.");
            }

            try
            {
                var isCaptchaValid = await _recaptchaService.VerifyCaptchaAsync(req.CaptchaToken);
                if (!isCaptchaValid)
                {
                    return BadRequest("Neispravan reCAPTCHA odgovor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Captcha is not available.");
                return BadRequest($"reCaptcha nije dostupna. Molimo vas da kontaktirate podrsku na {SupportEmail} ili pozovite {SupportPhone}");
            }
            return await CheckAndCreateDemoRequest(req);
        }

        private async Task<ActionResult> CheckAndCreateDemoRequest(DemoRequestBody req)
        {
            // Validate the request body
            if (string.IsNullOrEmpty(req.FirstName) ||
                string.IsNullOrEmpty(req.LastName) ||
                string.IsNullOrEmpty(req.Email) ||
                string.IsNullOrEmpty(req.Phone) ||
                string.IsNullOrEmpty(req.Message))
            {
                return BadRequest("All required fields must be provided: FirstName, LastName, Email, Message and Phone.");
            }

            var demoMeeting = new DemoMeetingRequest
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Phone = req.Phone,
                Email = req.Email,
                Message = req.Message,
                Company = req.Company,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _dbContext.AddAsync(demoMeeting);
                await _dbContext.SaveChangesAsync();
                //trigger sending email but do not wait
                _emailService.SendEmailAsync(configuration.GetSection("AdminRecipientEmailAddress").Value, "Novi zahtjev za demo sastankom", $"Dobili ste novi zahtjev za demo sastankom od korisnika sa Emailom: {req.Email}");
                return Ok("Demo request submitted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom kreiranja zahtjeva za demo: {ex.Message}");
                return StatusCode(500, "An error occurred while submitting the request. Please try again later.");
            }
        }

        [HttpPost("uploadProfilePhoto")]
        public async Task<ActionResult<UserDto>> UploadProfilePhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (!FileHelper.IsValidImage(photo))
            {
                return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Niste autorizovani za ovu funkcionalnost.");
            try
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(photo, userId);
                var user =  await _uow.UserRepository.GetUserByIdAsync(userId);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                await _uow.UserRepository.UpdateUserPhotoUrl(user, decodedFileUrl);
                var updatedUser = await FetchUserWithIncludesAsync(userId);
                var dto = ConvertCompanyUserToUserDto(user);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom upload-a profilne slike: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("removeProfilePhoto")]
        public async Task<ActionResult<UserDto>> RemoveProfilePhoto()
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Forbid("Niste autorizovani za ovu funkcionalnost.");
            try
            {
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                await _uow.UserRepository.UpdateUserPhotoUrl(user, null);
                var updatedUser = await FetchUserWithIncludesAsync(userId);
                var dto = ConvertCompanyUserToUserDto(user);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do greške prilikom brisanja profilne slike: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpGet("user/{userName}")]
        //[AllowAnonymous]
        //public async Task<ActionResult> GetUserInfo(string userName)
        //{
        //    return Ok(await _uow.UserRepository.GetUserInfo(userName));
        //}

        //[HttpGet("profile")]
        //public async Task<ActionResult> GetUserProfile()
        //{
        //    var id = HttpContext.User.GetUserId();
        //    var up = await _uow.UserRepository.GetProfile(id);
        //    return Ok(up);
        //}

        [HttpPost("updatebaseinfo")]
        public async Task<ActionResult<UserDto>> UpdateUserBaseInfo([FromBody] UserBaseInfoRequest req)
        {
            if(HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            req.UserId = HttpContext.User.GetUserId();

            if (await _userManager.Users.AnyAsync(u => u.Id != req.UserId &&
                                                       u.NormalizedUserName == req.Email.ToUpper()))
                return BadRequest("Username nije dostupan.");
            if (await _userManager.Users.AnyAsync(u => u.Id != req.UserId &&
                                                       u.NormalizedEmail == req.Email.ToUpper()))
                return BadRequest("Email nije dostupan.");

            var user = await FetchUserWithIncludesAsync((int)req.UserId);

            //user.Email = req.Email;
            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
            user.PhoneNumber = req.PhoneNumber;
            user.CityId = req.CityId;
            user.DateOfBirth = req.DateOfBirth;
            user.Gender = req.Gender;

            var result = await _userManager.UpdateAsync(user);

            user = await FetchUserWithIncludesAsync((int)req.UserId);

            if (!result.Succeeded) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            var dto = ConvertUserToUserDto(user);
            return dto;
        }

        [HttpPost("updatecompanybaseinfo")]
        public async Task<ActionResult<UserDto>> UpdateCompanyBaseInfo([FromBody] CompanyBaseInfoRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa");
            }
            var userId = HttpContext.User.GetUserId();
            var user = await FetchUserWithIncludesAsync(userId);
            if (user.IsCompany == false)
                return Unauthorized("Nemate pravo pristupa");
            //user.Email = req.Email;

            var sanitizer = new HtmlSanitizer();
            string sanitizedAboutUs = sanitizer.Sanitize(req.AboutUs);
            user.Company.AboutUs = sanitizedAboutUs;
            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
            user.PhoneNumber = req.PhoneNumber;
            user.CityId = req.CityId;
            user.Company.CompanyName = req.CompanyName;
            user.Company.Address = req.Address;
            user.Company.PhoneNumber = req.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            user = await FetchUserWithIncludesAsync(userId);

            if (!result.Succeeded) return BadRequest("Desila se greška prilikom ažuriranja kompanije.");
            var dto = ConvertCompanyUserToUserDto(user);
            return dto;
        }

        private async Task<User> FetchUserWithIncludesAsync(int? userId, string? email = null)
        {

            var userQuery = _userManager.Users
                .Include(u => u.UserEducations)
                .Include(u => u.EducationLevel)
                .Include(u => u.EmploymentType)
                .Include(u => u.UserPreviousCompanies)
                .Include(u => u.Company)
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .Include(u => u.City)
                .Include(u => u.EmploymentStatus)
                .Include(u => u.JobType)
                .Include(u => u.JobCategory);

            var user = await userQuery
                .SingleOrDefaultAsync(u =>
                    (string.IsNullOrEmpty(email) && u.Id == userId) ||
                    (!string.IsNullOrEmpty(email) &&
                        (u.UserName == email.ToLower() || u.Email == email)));
            if (user != null)
            {
                if (user.UserEducations != null)
                {
                    user.UserEducations = user.UserEducations
                        .OrderBy(r => r.EducationStartYear)
                        .ToList();
                }

                if (user.UserPreviousCompanies != null)
                {
                    user.UserPreviousCompanies = user.UserPreviousCompanies
                        .OrderBy(r => r.StartYear)
                        .ToList();
                }
            }

            return user;
        }

        [HttpPost("updateuserjobpreferences")]
        public async Task<ActionResult<UserDto>> UpdateUserJobPreferences([FromBody] UserJobPreferencesRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            req.UserId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync((int)req.UserId);

            var sanitizer = new HtmlSanitizer();
            string sanitizedBiography = sanitizer.Sanitize(req.Biography);
            
            user.Biography = sanitizedBiography;
            user.Position = req.Position;
            user.JobCategoryId = req.JobCategoryId;
            user.JobTypeId = req.JobTypeId;
            user.EmploymentTypeId = req.EmploymentTypeId;
            user.YearsOfExperience = req.YearsOfExperience;
            user.EducationLevelId = req.EducationLevelId;
            user.EmploymentStatusId = req.EmploymentStatusId;
            user.Languages = req.Languages;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            
            user = await FetchUserWithIncludesAsync((int)req.UserId);

            var dto = ConvertUserToUserDto(user);
            return dto;
        }

        [HttpPost("upsertusereducation")]
        public async Task<ActionResult<UserDto>> UpsertUserEducation([FromBody] UserEducationRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            req.UserId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync((int)req.UserId);

            if (req.UserEducationId == null)
            {
                var userEducation = new UserEducation()
                {
                    Degree = req.Degree,
                    EducationEndYear = req.EducationEndYear,
                    EducationStartYear = req.EducationStartYear,
                    FieldOfStudy = req.FieldOfStudy,
                    InstitutionName = req.InstitutionName,
                    University = req.University,
                };
                user.UserEducations.Add(userEducation);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            }
            else
            {
                var result = await _uow.UserRepository.UpdateUserEducationAsync(req);
                if (!result) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            }
            var updatedUser = await FetchUserWithIncludesAsync((int)req.UserId);
            var dto = ConvertUserToUserDto(updatedUser);
            return dto;
        }

        [HttpPost("upsertusercompany")]
        public async Task<ActionResult<UserDto>> UpserUserCompany([FromBody] UserCompanyRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            req.UserId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync((int)req.UserId);

            if (req.UserCompanyId == null)
            {
                var userCompany = new UserPreviousCompanies()
                {
                    Position = req.Position,
                    Description = req.Description,
                    CompanyName = req.CompanyName,
                    StartYear = req.StartYear,
                    EndYear = req.EndYear,
                };
                user.UserPreviousCompanies.Add(userCompany);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            }
            else
            {
                var result = await _uow.UserRepository.UpdateUserPreviousCompaniesAsync(req);
                if (!result) return BadRequest("Desila se greška prilikom ažuriranja korisnika.");
            }
            var updatedUser = await FetchUserWithIncludesAsync((int)req.UserId);
            var dto = ConvertUserToUserDto(updatedUser);
            return dto;
        }


        [HttpPost("deleteusercompany")]
        public async Task<ActionResult<UserDto>> DeleteUserCompany([FromBody] int userCompanyId)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            var userId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync(userId);

            var itemToDelete = user.UserPreviousCompanies.FirstOrDefault(e => e.Id == userCompanyId);
            if (itemToDelete != null)
            {
                user.UserPreviousCompanies.Remove(itemToDelete);
                var result = await _userManager.UpdateAsync(user); // Update the user with the removed education
                if (!result.Succeeded)
                {
                    return BadRequest("Greška prilikom brisanja kompanije korisnika.");
                }
            }
            else
            {
                return NotFound("Edukacija korisnika ne postoji.");
            }
            var dto = ConvertUserToUserDto(user);
            return dto;
        }

        [HttpPost("deleteusereducation")]
        public async Task<ActionResult<UserDto>> DeleteUserEducation([FromBody] int userEducationId)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa");
            }
            var userId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync(userId);

            var educationToDelete = user.UserEducations.FirstOrDefault(e => e.Id == userEducationId);
            if (educationToDelete != null)
            {
                user.UserEducations.Remove(educationToDelete);
                var result = await _userManager.UpdateAsync(user); // Update the user with the removed education
                if (!result.Succeeded)
                {
                    return BadRequest("Greška prilikom brisanja edukacije korisnika.");
                }
            }
            else
            {
                return NotFound("Edukacija korisnika ne postoji.");
            }
            var dto = ConvertUserToUserDto(user);
            return dto;
        }
        [HttpPost("useruploadresume")]
        public async Task<ActionResult<UserDto>> UserUploadResume([FromForm] UserUploadResumeRequest req)
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Nemate pravo pristupa.");
            }
            var userId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync(userId);

            if (req.CvFile == null)
            {
                return BadRequest("CV nije uploadovan.");
            }
            if (!FileHelper.IsValidPdf(req.CvFile))
            {
                return BadRequest("Nevažeći format datoteke. Dozvoljen je samo PDF format.");
            }
            if (req.CvFile != null)
            {
                try
                {
                    var fileUrl = await _blobStorageService.UploadFileAsync(req.CvFile, userId);
                    var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                    user.CvFilePath = decodedFileUrl;
                    user.CvFileName = req.CvFile.FileName;
                    await _userManager.UpdateAsync(user);
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Došlo je do greške prilikom upload-a file-a(CV): {ex.Message}");
                    return StatusCode(500, "Došlo je do greške prilikom upload-a file-a");
                }
            }

            var dto = ConvertUserToUserDto(user);
            return dto;
        }

       
        [HttpGet("userdeleteresume")]
        public async Task<ActionResult<UserDto>> UserDeleteResume()
        {
            if (HttpContext.User.GetUserId() == null)
            {
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync(userId);

            try
            {
                await _blobStorageService.RemoveFileAsync(user.CvFileName);
            }
            catch(Exception ex)
            {
                _logger.LogError("Desila se greška prilikom brisanja cv-a korisnika.");
                return StatusCode(500, "Desila se greška prilikom brisanja cv-a korisnika.");
            }
            user.CvFileName = null;
            user.CvFilePath = null;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Desila se greška prilikom brisanja cv-a korisnika.");

            var dto = ConvertUserToUserDto(user);
            return dto;
        }

        [HttpGet("cvfile")]
        public async Task<IActionResult> GetCVFile()
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _userManager.Users
               .SingleAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized("Niste autorizovani za pristup.");
            }
            try
            {
                var fileName = Path.GetFileName(user.CvFilePath);
                var fileDto = await _blobStorageService.GetFileAsync(fileName);
                if (fileDto.FileContent == null)
                {
                    return NotFound("Cv nije pronađen.");
                }

                return File(fileDto.FileContent, fileDto.MimeType, fileName);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Desila se greška prilikom dohvaćanja cv-a korisnika.");
            }
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
            if (HttpContext.User.GetUserName() == API.Helpers.Constants.TestUser && companyDto.UserName.ToLower() != API.Helpers.Constants.TestUser)
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

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("Korisnik ne postoji.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(user.Email);
            var resetLink = $"{UIBaseUrl}reset-password?email={HttpUtility.UrlEncode(encodedEmail)}&token={HttpUtility.UrlEncode(encodedToken)}";

            string emailBody = $@"
            <p style='color: #66023C;'>Za promjenu lozinke, molimo vas da otvorite sljedeći link:</p>
            <p style='text-align: center;'>
                <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Promjena lozinke</a>
            </p>";
            var subject = "Zahtjev za promjenom lozinke";

            var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, emailBody, configuration);
            try
            {
                await _emailService.SendEmailWithTemplateAsync(request.Email, subject, emailTemplate);
            }
            catch(Exception ex)
            {
                _logger.LogError("Došlo je do greške prilikom slanja verifikacijskog emaila za promjenu lozinke");
                return StatusCode(500, "Došlo je do greške prilikom slanja verifikacijskog emaila za promjenu lozinke");
            }

            return Ok();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var decodedToken = HttpUtility.UrlDecode(model.Token);
            var decodedEmail = HttpUtility.UrlDecode(model.Email);
            var user = await _userManager.FindByEmailAsync(decodedEmail);
            if (user == null)
            {
                return BadRequest("Zahtjev za promjenom lozinke nije ispravan.");
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (!resetResult.Succeeded)
            {
                return BadRequest(resetResult.Errors.Select(e => e.Description));
            }

            return Ok();
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null)
            {
                return NotFound("Korisnik ne postoji.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded || result.Errors.Any())
                return BadRequest("Lozinka koju ste unijeli kao trenutnu nije ispravna.");
            return Ok();
        }


        private UserDto ConvertUserToUserDto(User user)
        {
            try
            {
                var dto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CityId = user.CityId,
                    City = user.City?.Name,
                    ZipCode = user.City?.PostalCode,
                    UserName = user.UserName,
                    PhotoUrl = user.PhotoUrl,
                    Email = user.Email,
                    CompanyAddress = user.Company?.Address,
                    CompanyId = user.Company?.Id,
                    CompanyName = user.Company?.CompanyName,
                    CompanyPhone = user.Company?.PhoneNumber,
                    AboutCompany = user.Company?.AboutUs,
                    Credits = user.Credits,
                    PhoneNumber = user.PhoneNumber,
                    YearsOfExperience = user.YearsOfExperience,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Biography = user.Biography,
                    JobCategoryId = user.JobCategoryId,
                    JobTypeId = user.JobTypeId,
                    JobCategory = user.JobCategory?.Name,
                    JobType = user.JobType?.Name,
                    CvFilePath = user.CvFilePath, // Assuming this is the correct field
                    CvFileName = user.CvFileName,
                    Position = user.Position,
                    EmploymentType = user.EmploymentType?.Name,
                    EmploymentTypeId = user.EmploymentType?.Id,
                    EmploymentStatusId = user.EmploymentStatusId,
                    EmploymentStatus = user.EmploymentStatus?.Name,
                    EducationLevel = user.EducationLevel?.Name,
                    EducationLevelId = user.EducationLevel?.Id,
                    Coverletter = user.Coverletter,
                    Role = user.UserRoles.First().Role.Name,
                    Languages = user.Languages,
                    UserPreviousCompanies = user.UserPreviousCompanies?.Select(userPreviousCompany => new UserPreviousCompaniesDto()
                    {
                        CompanyName = userPreviousCompany.CompanyName,
                        Position = userPreviousCompany.Position,
                        Description = userPreviousCompany.Description,
                        StartYear = userPreviousCompany.StartYear,
                        EndYear = userPreviousCompany.EndYear,
                        UserCompanyId = userPreviousCompany.Id
                    }).ToList() ?? new List<UserPreviousCompaniesDto>(),
                    UserEducations = user.UserEducations?.Select(userEducation => new UserEducationDto
                    {
                        University = userEducation.University,
                        UserId = userEducation.UserId,
                        EducationEndYear = userEducation.EducationEndYear,
                        EducationStartYear = userEducation.EducationStartYear,
                        Degree = userEducation.Degree,
                        FieldOfStudy = userEducation.FieldOfStudy,
                        UserEducationId = userEducation.Id,
                        InstitutionName = userEducation.InstitutionName // Fixed this field assignment
                    }).ToList() ?? new List<UserEducationDto>() // In case user.UserEducations is null, return an empty list
                };

                return dto;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Desila se greška prilikom mapiranja user u userDto: {ex.Message}");
                throw;
            }
        }

        private UserDto ConvertCompanyUserToUserDto(User user)
        {
            var dto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                City = user.City?.Name,
                UserName = user.UserName,
                PhotoUrl = user.PhotoUrl,
                Email = user.Email,
                CompanyAddress = user.Company?.Address,
                CompanyId = user.Company?.Id,
                CompanyName = user.Company?.CompanyName,
                CompanyPhone = user.Company?.PhoneNumber,
                AboutCompany = user.Company?.AboutUs,
                Credits = user.Credits,
                PhoneNumber = user.PhoneNumber,
            };

            return dto;
        }

    }
}