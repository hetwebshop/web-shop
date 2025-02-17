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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration,
            IUnitOfWork uow, IMapper mapper, ITokenService tokenService, IEmailService emailService, IBlobStorageService blobStorageService, DataContext dbContext)
        {
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
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            registerDto.UserName = registerDto.Email;
            if (await UserNameExist(registerDto.UserName))
                return BadRequest("Ovaj email je već povezan s postojećim korisničkim nalogom.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Ovaj email je već povezan s postojećim korisničkim nalogom.");

            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            user.IsApproved = false;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            result = await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationUrl = $"{verificationEmailBaseAddress}confirm-email?userId=" + user.Id + "&token=" + Uri.EscapeDataString(emailToken);


            var notificationsSettings = new List<UserNotificationSettings>();
            if(user != null)
            {
                notificationsSettings.Add(new UserNotificationSettings()
                {
                    UserId = user.Id,
                    NotificationType = UserNotificationType.NewInterestingCompanyAdInApp,
                    IsEnabled = true
                });
                notificationsSettings.Add(new UserNotificationSettings()
                {
                    UserId = user.Id,
                    NotificationType = UserNotificationType.NewInterestingCompanyAdEmail,
                    IsEnabled = true
                });
                _dbContext.UserNotificationSettings.AddRange(notificationsSettings);
                await _dbContext.SaveChangesAsync();
            }

            // Slanje verifikacionog emaila (ovdje koristite svoju email uslugu)
            string messageBody = $@"
            <p style='color: #66023C;'>Hvala vam što ste se registrirali na našu platformu. Molimo vas da potvrdite vašu email adresu kako biste aktivirali svoj račun.</p>
            <p style='text-align: center;'>
                <a href='{verificationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Kliknite ovdje</a>
            </p>";
            string subject = "Verifikujte svoju email adresu";
            string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);

            await _emailService.SendEmailWithTemplateAsync(
                user.Email,
                subject,
                emailHtml
            );

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
            string subject = "Verifikujte svoju email adresu";
            string verificationUrl = "http://localhost:3000/";
            string messageBody = $@"
            <p style='color: #66023C;'>Hvala vam što ste se registrirali na našu platformu. Molimo vas da potvrdite vašu email adresu kako biste aktivirali svoj račun.</p>
            <p style='text-align: center;'>
                <a href='{verificationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Kliknite ovdje</a>
            </p>";
            string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);

            // Send the email
            await _emailService.SendEmailWithTemplateAsync(
                "ai.jobify@gmail.com",
                subject,
                emailHtml
            );
            return Ok();
        }


        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
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
        public async Task<ActionResult<UserDto>> RegisterCompany([FromForm] CompanyRegisterDto registerDto)
        {
            registerDto.UserName = registerDto.Email;
            if (await UserNameExist(registerDto.UserName))
                return BadRequest("Ovaj email je već povezan s postojećim korisničkim nalogom.");
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerDto.Email.ToUpper()))
                return BadRequest("Ovaj email je već povezan s postojećim korisničkim nalogom.");

            var user = _mapper.Map<User>(registerDto);
            user.LastActive = DateTime.UtcNow;
            if (registerDto.Photo != null)
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(registerDto.Photo);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                user.PhotoUrl = decodedFileUrl;
                user.Company.PhotoUrl = decodedFileUrl;
            }
            user.IsApproved = false;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());

            result = await _userManager.AddToRoleAsync(user, RoleType.Company.ToString());
            if (!result.Succeeded) return BadRequest(result.Errors.ToStringError());


            var companyNotifications = new List<CompanyNotificationPreferences>();
            companyNotifications.Add(new CompanyNotificationPreferences()
            {
                UserId = user.Id,
                NotificationType = Entities.Notification.CompanyNotificationType.newApplicantInApp,
                IsEnabled = true
            });
            companyNotifications.Add(new CompanyNotificationPreferences()
            {
                UserId = user.Id,
                NotificationType = Entities.Notification.CompanyNotificationType.newApplicantEmail,
                IsEnabled = true
            });
            companyNotifications.Add(new CompanyNotificationPreferences()
            {
                UserId = user.Id,
                NotificationType = Entities.Notification.CompanyNotificationType.newInterestingUserAdInApp,
                IsEnabled = false
            });
            companyNotifications.Add(new CompanyNotificationPreferences()
            {
                UserId = user.Id,
                NotificationType = Entities.Notification.CompanyNotificationType.newInsterestingUserAdEmail,
                IsEnabled = false
            });
            _dbContext.CompanyNotificationPreferences.AddRange(companyNotifications);
            await _dbContext.SaveChangesAsync();
            //var token = await _tokenService.CreateToken(user);

            try
            {
                string messageBody = $@"
                <p style='color: #66023C;'>Dragi <strong>{user.Company.CompanyName}</strong>,</p>
                <p style='color: #66023C;'>Hvala što ste se registrovali na našoj platformi! Vaš korisnički račun je uspješno kreiran, međutim, još uvijek čeka odobrenje od strane administratora. Nakon verifikacije, moći ćete pristupiti svim funkcijama i resursima.</p>
                <p style='color: #66023C;'>Vaš korisnički račun će biti aktiviran čim administrator izvrši verifikaciju.</p>
                <p style='color: #66023C;'>Zahvaljujemo na strpljenju, i radujemo se što ćete postati dio naše zajednice!</p>";

                string subject = "Registracija uspješna - Čekate Verifikaciju";
                string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, configuration);

                await _emailService.SendEmailWithTemplateAsync(
                    user.Email,
                    subject,
                    emailHtml
                );
                //Notify admin
                await _emailService.SendEmailAsync(configuration.GetSection("AdminRecipientEmailAddress").Value, "Registracija kompanije - novi zahtjev", $"Dobili ste novi zahtjev za registracijom od kompanije: {registerDto.Email}, {registerDto.CompanyName}");
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
                PhotoUrl = user.PhotoUrl,
                Email = user.Email,
                IsCompany = true,
                CompanyAddress = user.Company.Address,
            };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await FetchUserWithIncludesAsync(null, loginDto.UserNameOrEmail);

            if (user == null) return BadRequest("Korisnik sa unešenom email adresom ne postoji.");

            if (!user.IsApproved)
            {
                return BadRequest("Račun vaše kompanije čeka odobrenje od strane admina.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded && loginDto.UserNameOrEmail != Constants.TestUser) return BadRequest("Pogrešan email ili password.");

            var token = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var dto = ConvertUserToUserDto(user, token);

            return dto;
        }

        [HttpGet("token-update")]
        public async Task<ActionResult<UserDto>> GetUpdatedToken()
        {
            var id = HttpContext.User.GetUserId();
            var user = await FetchUserWithIncludesAsync(id);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var token = await _tokenService.CreateToken(user);

            var dto = ConvertUserToUserDto(user, token);
            return dto;
        }

        [HttpGet("{userName}")]
        [AllowAnonymous]
        public async Task<bool> UserNameExist(string userName)
        {
            return await _uow.UserRepository.UserExist(userName);
        }

        [HttpPost("demo-request")]
        [AllowAnonymous]
        public async Task<ActionResult> SubmitDemoRequest([FromBody] DemoRequestBody req)
        {
            // Validate the request body
            if (string.IsNullOrEmpty(req.FirstName) ||
                string.IsNullOrEmpty(req.LastName) ||
                string.IsNullOrEmpty(req.Email) ||
                string.IsNullOrEmpty(req.Phone))
            {
                return BadRequest("All required fields must be provided: FirstName, LastName, Email, and Phone.");
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
                await _emailService.SendEmailAsync(configuration.GetSection("AdminRecipientEmailAddress").Value, "Novi zahtjev za demo sastankom", $"Dobili ste novi zahtjev za demo sastankom od korisnika sa Emailom: {req.Email}");
                return Ok("Demo request submitted successfully.");
            }
            catch (Exception ex)
            {
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
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Forbid("Niste autorizovani za ovu funkcionalnost.");
            try
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(photo);
                var user =  await _uow.UserRepository.GetUserByIdAsync(userId);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                await _uow.UserRepository.UpdateUserPhotoUrl(user, decodedFileUrl);
                var updatedUser = await FetchUserWithIncludesAsync(userId);
                var dto = ConvertCompanyUserToUserDto(user);
                return dto;
            }
            catch (Exception ex)
            {
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
                return Unauthorized("Korisnik ne postoji!");
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
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();
            var user = await FetchUserWithIncludesAsync(userId);
            if (user.IsCompany == false)
                return BadRequest("Korisnik ne pripada kompaniji!");
            //user.Email = req.Email;
            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
            user.PhoneNumber = req.PhoneNumber;
            user.CityId = req.CityId;
            user.Company.CompanyName = req.CompanyName;
            user.Company.Address = req.Address;
            user.Company.AboutUs = req.AboutUs;
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
                return Unauthorized("Korisnik ne postoji!");
            }
            req.UserId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync((int)req.UserId);

            user.Biography = req.Biography;
            user.Position = req.Position;
            user.JobCategoryId = req.JobCategoryId;
            user.JobTypeId = req.JobTypeId;
            user.EmploymentTypeId = req.EmploymentTypeId;
            user.YearsOfExperience = req.YearsOfExperience;
            user.EducationLevelId = req.EducationLevelId;
            user.EmploymentStatusId = req.EmploymentStatusId;

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
                return Unauthorized("Korisnik ne postoji!");
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
                return Unauthorized("Korisnik ne postoji!");
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
                return Unauthorized("Korisnik ne postoji!");
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
                return Unauthorized("Korisnik ne postoji!");
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
                return Unauthorized("Korisnik ne postoji!");
            }
            var userId = HttpContext.User.GetUserId();

            var user = await FetchUserWithIncludesAsync(userId);

            if (req.CvFile == null)
            {
                return BadRequest("CV nije uploadovan.");
            }

            if (req.CvFile != null)
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(req.CvFile);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                user.CvFilePath = decodedFileUrl;
                user.CvFileName = req.CvFile.FileName;
                await _userManager.UpdateAsync(user);
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

            user.CvFileName = null;
            user.CvFilePath = null;
            //IMPLEMENTE DELETE FILE FROM BLOB
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
                return Forbid("Niste autorizovani za pristup.");
            }
            var fileName = Path.GetFileName(user.CvFilePath);
            var fileDto = await _blobStorageService.GetFileAsync(fileName);

            if (fileDto.FileContent == null)
            {
                return NotFound("Cv nije pronađen.");
            }

            return File(fileDto.FileContent, fileDto.MimeType, fileName);
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
                var fileUrl = await _blobStorageService.UploadFileAsync(profileDto.CvFile);
                profileDto.CvFilePath = fileUrl;
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
            var resetLink = $"{UIBaseUrl}reset-password?email={HttpUtility.UrlEncode(encodedEmail)}&token={HttpUtility.UrlEncode(encodedToken)}";

            string emailBody = $@"
            <p style='color: #66023C;'>Za promjenu lozinke, molimo vas da otvorite sljedeći link:</p>
            <p style='text-align: center;'>
                <a href='{resetLink}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Promjena lozinke</a>
            </p>";
            var subject = "Zahtjev za promjenom lozinke";

            var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, emailBody, configuration);
            await _emailService.SendEmailWithTemplateAsync(request.Email, subject, emailTemplate);

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


        private UserDto ConvertUserToUserDto(User user, string token = null)
        {
            var dto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CityId = user.CityId,
                City = user.City?.Name,
                UserName = user.UserName,
                PhotoUrl = user.PhotoUrl,
                Email = user.Email,
                IsCompany = user.IsCompany,
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
                Roles = user.UserRoles.Select(r => r.Role.Name).ToList(),
                EmploymentType = user.EmploymentType?.Name,
                EmploymentTypeId = user.EmploymentType?.Id,
                EmploymentStatusId = user.EmploymentStatusId,
                EmploymentStatus = user.EmploymentStatus?.Name,
                EducationLevel = user.EducationLevel?.Name,
                EducationLevelId = user.EducationLevel?.Id,
                RefreshToken = user.RefreshToken,
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
            if(!string.IsNullOrEmpty(token))
                dto.Token = token;
            return dto;
        }

        private UserDto ConvertCompanyUserToUserDto(User user, string token = null)
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
                IsCompany = user.IsCompany,
                CompanyAddress = user.Company?.Address,
                CompanyId = user.Company?.Id,
                CompanyName = user.Company?.CompanyName,
                CompanyPhone = user.Company?.PhoneNumber,
                AboutCompany = user.Company?.AboutUs,
                Credits = user.Credits,
                PhoneNumber = user.PhoneNumber,
                Roles = user.UserRoles.Select(r => r.Role.Name).ToList(),
            };
            if (!string.IsNullOrEmpty(token))
                dto.Token = token;
            return dto;
        }

    }
}