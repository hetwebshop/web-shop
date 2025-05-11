using API.Data;
using API.Data.ICompanyJobPostRepository;
using API.DTOs;
using API.Entities;
using API.Entities.Applications;
using API.Entities.CompanyJobPost;
using API.Entities.Payment;
using API.Extensions;
using API.Helpers;
using API.Mappers;
using API.PaginationEntities;
using API.Services;
using API.Services.CompanyJobPostServices;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace API.Controllers
{
    [Authorize]
    public class CompanyJobController : BaseController
    {
        private readonly ICompanyJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly ICompanyJobPostRepository _jobPostRepository;
        private readonly IUserApplicationsRepository _userApplicationsRepository;
        private readonly DataContext _dbContext;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ISendNotificationsQueueClient _sendNotificationsQueueClient;
        private readonly ILogger<CompanyJobController> _logger;
        private readonly IConfiguration _configuration;
        private readonly int CompanyAdActiveDays;
        private double AIAnalysisPricePerCandidate;
        private readonly string UIBaseUrl;

        public CompanyJobController(ICompanyJobPostService jobPostService, IUnitOfWork uow, ICompanyJobPostRepository companyJobPostRepository, IUserApplicationsRepository userApplicationsRepository, DataContext dbContext, IBlobStorageService blobStorageService, ISendNotificationsQueueClient sendNotificationsQueueClient, ILogger<CompanyJobController> logger, IConfiguration configuration)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _jobPostRepository = companyJobPostRepository;
            _userApplicationsRepository = userApplicationsRepository;
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
            _sendNotificationsQueueClient = sendNotificationsQueueClient;
            _logger = logger;
            _configuration = configuration;
            CompanyAdActiveDays = int.Parse(configuration.GetSection("CompanyActiveAdDays").Value);
            AIAnalysisPricePerCandidate = double.Parse(configuration.GetSection("AIAnalysisPricePerCandidate").Value);
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
        }


        [HttpPost("registeredcompanies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegisteredCompanies([FromBody] AdsPaginationParameters adsParameters)
        {
            var registeredCompanies = await _jobPostService.GetRegisteredCompaniesAsync(adsParameters);
            var pagedResponse = registeredCompanies.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpPost("adsprivate")]
        public async Task<IActionResult> GetAds([FromBody] AdsPaginationParameters adsParameters)
        {
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            var currentUserId = HttpContext.User.GetUserId();
            if(currentUserId != null && currentUserId != 0)
            {
                var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
                
                foreach (var item in pagedResponse.Items)
                {
                    if ((item.UsersThatAppliedOnJobPost.Any() && item.UsersThatAppliedOnJobPost.Contains(currentUserId)) || user.UserRoles.Select(r => r.Role.Name).Contains("Company"))
                        item.CanCurrentUserApplyOnAd = false;
                }
            }
            return Ok(pagedResponse);
        }

        [HttpPost("adspublic")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdsPublic([FromBody] AdsPaginationParameters adsParameters)
        {
            adsParameters.adStatus = JobPostStatus.Active;
            var jobPosts = await _jobPostService.GetJobPostsAsync(adsParameters);
            var pagedResponse = jobPosts.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("employmenttypes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmploymentTypes()
        {
            var employmentTypes = await _jobPostRepository.GetEmploymentTypesAsync();
            return Ok(employmentTypes);
        }

        [HttpGet("educationlevels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEducationLevels()
        {
            var educationLevels = await _jobPostRepository.GetEducationLevels();
            return Ok(educationLevels);
        }

        [HttpGet("employmentstatuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmploymentStatuses()
        {
            var employmentStatuses = await _jobPostRepository.GetEmploymentStatusesAsync();
            return Ok(employmentStatuses);
        }

        [HttpPost("company-ads")]
        public async Task<IActionResult> GetCompanyAds([FromBody] AdsPaginationParameters adsParameters)
        {
            var currentUserId = HttpContext.User.GetUserId();
            adsParameters.UserId = currentUserId;
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return NotFound("Ne pripadata niti jednoj kompaniji.");
            adsParameters.CompanyId = user.CompanyId;
            var myAds = await _jobPostService.GetCompanyJobPostsAsync(adsParameters);
            var pagedResponse = myAds.ToPagedResponse();
            return Ok(pagedResponse);
        }

        [HttpGet("company-job/{id}")]
        public async Task<IActionResult> GetCompanyJobById(int id)
        {
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (currentUserId != null && currentUserId != 0)
            {
                companyJob.CanCurrentUserApplyOnAd = companyJob.UsersThatAppliedOnJobPost.Contains(currentUserId) || user.UserRoles.Select(r => r.Role.Name).Contains("Company") ? false : true;
            }
            if (companyJob.IsDeleted || companyJob.JobPostStatusId != (int)JobPostStatus.Active || companyJob.AdEndDate < DateTime.UtcNow)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            return Ok(companyJob);
        }

        [HttpGet("company-job-public/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyJobByIdPublic(int id)
        {
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            if (companyJob.IsDeleted || companyJob.JobPostStatusId != (int)JobPostStatus.Active || companyJob.AdEndDate < DateTime.UtcNow)
                return NotFound("Oglas je obrisan, zatvoren, ili je istekao.");
            return Ok(companyJob);
        }

        [HttpGet("company-my-ad/{id}")]
        public async Task<IActionResult> GetCompanyMyJobById(int id)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            if (companyJob != null && companyJob.SubmittingUserId != currentUserId)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            return Ok(companyJob);
        }

        [HttpGet("company-active-jobs")]
        public async Task<IActionResult> GetCompanyActiveJobs()
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            
            
            var companyJobs = await _jobPostRepository.GetCompanyActiveAdsAsync((int)user.CompanyId);
            var dtos = companyJobs.ToDto();
            foreach(var item in dtos)
            {
                item.PricingPlanName = companyJobs.First(r => r.Id == item.Id).PricingPlan.Name;
            }
            return Ok(dtos);
        }

        [HttpPost("runaiforallcandidates/{jobId}")]
        public async Task<IActionResult> RunAIForAllCandidatesOfCompanyJobPost(int jobId)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || user.CompanyId == null)
                    return Unauthorized("Nemate pravo pristupa.");

                var companyId = user.CompanyId;
                var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
                if (companyJobPost == null)
                    return NotFound();
                if (companyJobPost.User.CompanyId != companyId)
                    return Unauthorized("Nemate pravo pristupa.");

                if (companyJobPost.PricingPlan.Name != "Plus")
                    return Unauthorized("Odabrani paket oglasa nema mogućnost pokretanja AI analize.");

                var userApplications = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
                //We take only items that are already calculated with no error
                var userApplicationsWithAIAnalysis = userApplications.Where(r => r.AIAnalysisStatus == "Success");

                if (userApplicationsWithAIAnalysis != null && userApplicationsWithAIAnalysis.Any())
                {
                    var totalAIAnalysisPrice = Math.Round(userApplicationsWithAIAnalysis.Count() * AIAnalysisPricePerCandidate, 1);
                    var userCredits = user.Credits;
                    if (userCredits < totalAIAnalysisPrice)
                        return BadRequest("Nemate dovoljno kredita za izvršavanje AI analize. Molimo Vas da dopunite kredite, te nakon toga pokrenete AI analizu.");

                    //go through all applications, doesn't matter if it is success or not
                    foreach(var item in userApplications)
                    {
                        item.AIFeatureUnlocked = true;
                    }

                    user.Credits -= totalAIAnalysisPrice;

                    var userTransaction = new UserTransaction()
                    {
                        Amount = totalAIAnalysisPrice,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        ChFullName = user.FirstName + " " + user.LastName,
                        IsProcessed = false,
                        TransactionType = TransactionType.RunningAi,
                        IsAddingCredits = false,
                        OrderInfo = OrderInfoMessages.RunningAiMessage
                    };

                    await _dbContext.UserTransactions.AddAsync(userTransaction);

                    await _dbContext.SaveChangesAsync();

                    return Ok(true);
                }
                return NotFound(false);
            }
            catch(Exception ex)
            {
                return StatusCode(500, false);
            }
        }

        //[HttpGet("aianalysisstatuspolling/{jobId}")]
        //public async Task<IActionResult> AIAnalysisForAllCandidatesStatusPolling(int jobId)
        //{
        //    try
        //    {
        //        var userId = HttpContext.User.GetUserId();
        //        var user = await _uow.UserRepository.GetUserByIdAsync(userId);
        //        if (user == null || user.CompanyId == null)
        //            return Unauthorized("You do not belong to the current company.");

        //        var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
        //        if (companyJobPost == null)
        //            return NotFound();

        //        var companyId = user.CompanyId;
        //        if (companyJobPost.User == null || companyJobPost.User.CompanyId != companyId)
        //            return Unauthorized("Not belong to current company");

        //        var aiAnalysisStartedOn = companyJobPost.AiAnalysisStartedOn;
        //        var now = DateTime.UtcNow;

        //        if (aiAnalysisStartedOn.HasValue && aiAnalysisStartedOn.Value.AddMinutes(15) < now
        //            && companyJobPost.AiAnalysisStatus == Entities.CompanyJobPost.AiAnalysisStatus.Running)
        //        {
        //            //Use transaction with isolation here to ensure that there is no race condition(2 users from different machines pings this endpoint)
        //            using (var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
        //            {
        //                try
        //                {
        //                    companyJobPost.AiAnalysisError = "Desila se greška prilikom izvršavanja AI analize. Unutar 15 minuta analiza se nije uspješno izvršila.";
        //                    companyJobPost.AiAnalysisHasError = true;
        //                    companyJobPost.AiAnalysisEndedOn = DateTime.UtcNow;
        //                    companyJobPost.AiAnalysisStatus = Entities.CompanyJobPost.AiAnalysisStatus.Completed;


        //                    var userTransaction = new UserTransaction()
        //                    {
        //                        UserId = user.Id,
        //                        CreatedAt = DateTime.UtcNow,
        //                        ChFullName = user.FirstName + " " + user.LastName,
        //                        IsProcessed = false,
        //                        TransactionType = TransactionType.RefundAiCredits,
        //                        IsAddingCredits = true,
        //                        OrderInfo = OrderInfoMessages.RefundAiCreditsMessage
        //                    };

        //                    //Ensure credits are only refunded once
        //                    if (companyJobPost.AiAnalysisReservedCredits.HasValue)
        //                    {
        //                        user.Credits += companyJobPost.AiAnalysisReservedCredits.Value;
        //                        userTransaction.Amount = companyJobPost.AiAnalysisReservedCredits.Value;

        //                        companyJobPost.AiAnalysisReservedCredits = null;
        //                    }

        //                    await _dbContext.UserTransactions.AddAsync(userTransaction);

        //                    await _dbContext.SaveChangesAsync();
        //                    await transaction.CommitAsync(); //Commit transaction only after all operations succeed
        //                }
        //                catch (Exception ex)
        //                {
        //                    await transaction.RollbackAsync(); //Rollback transaction if anything fails
        //                    return StatusCode(500, "An error occurred while processing AI analysis status.");
        //                }
        //            }
        //        }

        //        var res = new AIAnalysisPollingResponse()
        //        {
        //            Error = companyJobPost.AiAnalysisError,
        //            HasError = companyJobPost.AiAnalysisHasError ?? false,
        //            Status = companyJobPost.AiAnalysisStatus.HasValue ? companyJobPost.AiAnalysisStatus.Value : AiAnalysisStatus.Completed,
        //            UserCredits = user.Credits
        //        };
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "An error occurred while fetching AI analysis status.");
        //    }
        //}


        [HttpGet("company-job-candidates/{jobId}")]
        public async Task<IActionResult> GetCompanyJobCandidates(int jobId)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("Nemate pravo pristupa");

            var companyId = user.CompanyId;
            var companyJobPost = await _jobPostRepository.GetCompanyJobPostByIdAsync(jobId);
            if (companyJobPost == null)
                return NotFound();
            if (companyJobPost.User.CompanyId != companyId)
                return Unauthorized("Nemate pravo pristupa");
            if (companyJobPost.AdEndDate.AddDays(CompanyAdActiveDays) < DateTime.UtcNow)
                return BadRequest("Pristup aplikaciji je istekao");

            var userApplicationsForJobPost = await _userApplicationsRepository.GetApplicationsForSpecificCompanyJobPost(jobId);
            var conversations = await _dbContext.Conversations.Where(r => r.CompanyJobPostId == jobId).ToListAsync();

            var allPreviousCompanyJobPostsIds = await _dbContext.CompanyJobPosts.Where(r => r.SubmittingUserId == user.Id && r.Id != companyJobPost.Id).Select(r => r.Id).ToListAsync();
            List<int> getApplicantsForAllCompanyJobPosts = new List<int>();

            if (allPreviousCompanyJobPostsIds.Any())
            {
                // If there are other job posts, fetch the applicants who applied to those job posts
                getApplicantsForAllCompanyJobPosts = await _dbContext.UserApplications
                    .Where(r => allPreviousCompanyJobPostsIds.Contains(r.CompanyJobPostId))
                    .Select(r => r.SubmittingUserId)
                    .ToListAsync();
            }

            var candidatesTableData = new List<JobCandidatesTableDataDto>();
            foreach (var application in userApplicationsForJobPost)
            {
                var tableData = new JobCandidatesTableDataDto()
                {
                    UserId = application.SubmittingUserId,
                    CandidateEmail = application.Email,
                    ApplicationStatusId = application.ApplicationStatusId,
                    UserApplicationId = application.Id,
                    CandidateCity = application.City?.Name,
                    CandidateCoverLetter = application.CoverLetter,
                    CandidateDateOfBirth = application.DateOfBirth,
                    CandidateFullName = application.LastName + " " + application.FirstName,
                    CandidateGender = application.Gender,
                    CandidatePhoneNumber = application.PhoneNumber,
                    CvFilePath = application.CvFilePath,
                    ApplicationDate = application.CreatedAt,
                    MeetingDateTime = application.MeetingDateTime,
                    IsOnlineMeeting = application.IsOnlineMeeting,
                    AIMatchingResult = application.AIMatchingResult,
                    AIMatchingSkills = application.AIMatchingSkills,
                    AIMatchingDescription = application.AIMatchingDescription,
                    AIMatchingEducationLevel = application.AIMatchingEducationLevel,
                    AIMatchingExperience = application.AIMatchingExperience,
                    AIAnalysisStatus = application.AIAnalysisStatus,
                    AIFeatureUnlocked = application.AIFeatureUnlocked,
                    OnlineMeetingLink = application.OnlineMeetingLink,
                    Feedback = application.Feedback,
                    MeetingPlace = application.MeetingPlace,
                    ConversationId = conversations.FirstOrDefault(r => r.ToUserId == application.SubmittingUserId)?.Id,
                    DidUserApplyOnPreviousCompanyJobPosts = getApplicantsForAllCompanyJobPosts.Contains(application.SubmittingUserId)
                };
                candidatesTableData.Add(tableData);
            }
            return Ok(candidatesTableData);
        }

        private string SanitizeSheetName(string name)
        {
            var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            name = name.Trim();
            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '-');
            }
            return name.Length > 31 ? name.Substring(0, 31) : name;
        }

        [HttpPost("export-to-excel")]
        public async Task<IActionResult> ExportToExcel([FromBody] ExportCandidatesToExcelRequest req)
        {
            var userId = HttpContext.User.GetUserId();
            var userApplicationIds = req.UserApplicationIds;
            var applications = await _dbContext.UserApplications
                .Where(r => userApplicationIds.Contains(r.Id))
                .Include(r => r.City)
                .Include(a => a.User)
                .Include(a => a.Educations)
                .Include(a => a.PreviousCompanies)
                .Include(a => a.CompanyJobPost)
                .ToListAsync();

            if (applications[0].CompanyJobPost == null)
            {
                return BadRequest("Došlo je do greške, pokušajte ponovo");
            }

            foreach(var app in applications)
            {
                if(app.CompanyJobPost.SubmittingUserId != userId)
                {
                    return Unauthorized("Nemate pravo pristupa!");
                }
            }

            var position = applications[0].CompanyJobPost.Position;

            string sheetName = string.IsNullOrWhiteSpace(position)
            ? "Kandidati"
            : SanitizeSheetName(position);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            int row = 1;

            // Header
            worksheet.Cell(row, 1).Value = "Pregledajte kandidata putem poslovnioglasi.ba";
            worksheet.Cell(row, 2).Value = "CV file";
            worksheet.Cell(row, 3).Value = "Ime i prezime";
            worksheet.Cell(row, 4).Value = "Email";
            worksheet.Cell(row, 5).Value = "Broj telefona";
            worksheet.Cell(row, 6).Value = "Datum rođenja";
            worksheet.Cell(row, 7).Value = "Spol";
            worksheet.Cell(row, 8).Value = "Grad";
            worksheet.Cell(row, 9).Value = "Datum prijave";
            worksheet.Cell(row, 10).Value = "Datum i vrijeme sastanka";
            worksheet.Cell(row, 11).Value = "Mjesto sastanka/Online sastanak";
            worksheet.Cell(row, 12).Value = "Godine iskustva";
            worksheet.Cell(row, 13).Value = "Ukupan rezultat AI analize";
            worksheet.Cell(row, 14).Value = "AI analiza - vještine";
            worksheet.Cell(row, 15).Value = "AI analiza - iskustvo";
            worksheet.Cell(row, 16).Value = "AI analiza - obrazovanje";
            worksheet.Cell(row, 17).Value = "AI analiza - obrazloženje";
            worksheet.Cell(row, 18).Value = "Biografija";
            worksheet.Cell(row, 19).Value = "Jezici";
            worksheet.Cell(row, 20).Value = "Status";
            worksheet.Cell(row, 21).Value = "Vaš odgovor";
            worksheet.Cell(row, 21).Value = "Edukacije i prethodne kompanije";
            var headerRow = worksheet.Row(row);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightYellow;

            row++;

            worksheet.Cell(row, 1).Value = $"Ukupan broj kandidata: {applications.Count}";
            worksheet.Row(row).Style.Font.Italic = true;
            row++;

            var timeZone = TZConvert.GetTimeZoneInfo(req.Timezone);

            foreach (var app in applications)
            {
                int startRow = row;

                // Main application info
                worksheet.Cell(row, 1).Value = $"poslovnioglasi.ba - kandidat {app.Id}";
                worksheet.Cell(row, 1).SetHyperlink(new XLHyperlink($"{UIBaseUrl}company-settings/candidate-details/{app.Id}"));

                worksheet.Cell(row, 2).Value = string.IsNullOrEmpty(app.CvFilePath) ? "Nije priložen" : "Pregledajte klikom ovdje";
                worksheet.Cell(row, 2).SetHyperlink(new XLHyperlink($"{UIBaseUrl}company-settings/candidate-details/{app.Id}"));


                worksheet.Cell(row, 3).Value = $"{app.FirstName} {app.LastName}";
                worksheet.Cell(row, 4).Value = app.Email;
                worksheet.Cell(row, 5).Value = app.PhoneNumber;
                worksheet.Cell(row, 6).Value = app.DateOfBirth.ToString("dd.MM.yyyy");
                worksheet.Cell(row, 7).Value = app.Gender == Gender.Male ? "Muškarac" : app.Gender == Gender.Female ? "Žena" : "Ostalo";
                worksheet.Cell(row, 8).Value = app.City?.Name;

                var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(app.CreatedAt, timeZone);
                var localMeetingDateTime = app.MeetingDateTime.HasValue
                    ? TimeZoneInfo.ConvertTimeFromUtc(app.MeetingDateTime.Value, timeZone)
                    : (DateTime?)null;

                worksheet.Cell(row, 9).Value = localCreatedAt.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 10).Value = localMeetingDateTime?.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 11).Value = app.IsOnlineMeeting == true ? app.OnlineMeetingLink : app.MeetingPlace;
                worksheet.Cell(row, 12).Value = app.YearsOfExperience;
                worksheet.Cell(row, 13).Value = app.AIMatchingResult;
                worksheet.Cell(row, 14).Value = app.AIMatchingSkills;
                worksheet.Cell(row, 15).Value = app.AIMatchingExperience;
                worksheet.Cell(row, 16).Value = app.AIMatchingEducationLevel;
                worksheet.Cell(row, 17).Value = app.AIMatchingDescription;
                worksheet.Cell(row, 18).Value = RemoveHtmlTags(app.Biography);
                worksheet.Cell(row, 19).Value = app.Languages;
                worksheet.Cell(row, 20).Value = app.ApplicationStatusId == ApplicationStatus.MeetingScheduled ? "Zakazan sastanak" : app.ApplicationStatusId == ApplicationStatus.Rejected ? "Odbijen" : app.ApplicationStatusId == ApplicationStatus.WaitingForResponse ? "Čeka se na odgovor" : "Zaposlen";
                worksheet.Cell(row, 21).Value = app.Feedback;

                row++;

                // Edukacije
                foreach (var edu in app.Educations)
                {
                    worksheet.Cell(row, 22).Value = "Edukacija";
                    string endYear = edu.EducationEndYear.HasValue
                        ? edu.EducationEndYear.Value.ToString()
                        : "Korisnik još uvijek pohađa edukaciju";

                    worksheet.Cell(row, 23).Value =
                        $"{edu.InstitutionName}, {edu.Degree}, {edu.FieldOfStudy}, {edu.EducationStartYear} - {endYear}";

                    worksheet.Row(row).Style.Alignment.Indent = 1;
                    row++;
                }

                // Prethodne kompanije
                foreach (var job in app.PreviousCompanies)
                {
                    worksheet.Cell(row, 22).Value = "Prethodna kompanija";
                    string endYear = job.EndYear.HasValue
                        ? job.EndYear.Value.ToString()
                        : "Korisnik još uvijek radi za kompaniju";

                    worksheet.Cell(row, 23).Value =
                        $"{job.CompanyName}, {job.Position}, {job.StartYear} - {endYear}";

                    worksheet.Row(row).Style.Alignment.Indent = 1;
                    row++;
                }

                // Collapse grupisanje
                if (row - 1 > startRow)
                {
                    worksheet.Rows(startRow + 1, row - 1).Group();
                }

                // Prazan red između aplikacija
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Kandidati_{position}_{DateTime.UtcNow:yyyyMMdd}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public static string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty); // Uklanja sve HTML tagove
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCompanyJobPost([FromForm] CompanyJobPostDto companyJobPostDto)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Forbid("Niste autorizovani za ovu funkcionalnost.");
            if (companyJobPostDto.Logo != null && !FileHelper.IsValidImage(companyJobPostDto.Logo))
            {
                return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            var pricingPlan = await _dbContext.PricingPlanCompanies.FirstOrDefaultAsync(r => r.Name.Equals(companyJobPostDto.PricingPlanName) && r.AdActiveDays == companyJobPostDto.AdDuration);
            if (pricingPlan == null)
            {
                _logger.LogWarning($"UserId: {userId} selected an invalid pricing plan.");
                return BadRequest("Niste odabrali ispravan plan paketa oglasa.");
            }
               
            if (user.Credits < pricingPlan.PriceInCredits)
            {
                _logger.LogWarning($"UserId: {userId} does not have enough credits to create a job post.");
                return BadRequest("Nemate dovoljno kredita za objavu odabranog tipa oglasa. Molimo Vas da dopunite kredite ili odaberete neki drugi paket oglasa");
            }
            companyJobPostDto.SubmittingUserId = userId;
            companyJobPostDto.PricingPlanId = pricingPlan.Id;
            var sanitizer = new HtmlSanitizer();
            string sanitizedJobDescription = sanitizer.Sanitize(companyJobPostDto.JobDescription);
            string sanitizedBenefits = sanitizer.Sanitize(companyJobPostDto.Benefits);
            string sanitizedWorkEnv = sanitizer.Sanitize(companyJobPostDto.WorkEnvironmentDescription);
            companyJobPostDto.JobDescription = sanitizedJobDescription;
            companyJobPostDto.WorkEnvironmentDescription = sanitizedWorkEnv;
            companyJobPostDto.Benefits = sanitizedBenefits;

            try
            {
                var newItem = await _jobPostService.CreateCompanyJobPostAsync(companyJobPostDto);
                newItem.CurrentUserCredits = user.Credits;
                return Ok(newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create job post for UserId: {userId}. Error: {ex.Message}");
                return StatusCode(500, "Došlo je do greške prilikom kreiranja oglasa. Pokušajte ponovo kasnije.");
            }
        }

        [HttpPost("uploadLogo/{id}")]
        public async Task<IActionResult> UploadAdLogo(int id, IFormFile photo)
        {
            if(photo == null)
            {
                return BadRequest("Logo nije priložen.");
            }
            if (!FileHelper.IsValidImage(photo))
            {
                return BadRequest("Nevažeći format slike. Dozvoljeni formati: JPG, PNG, GIF, BMP, WEBP.");
            }
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            try
            {
                var existingLogoUrl = companyJob.PhotoUrl;
                if (existingLogoUrl != null && existingLogoUrl != user.PhotoUrl)
                {
                    var fileName = existingLogoUrl.Substring(existingLogoUrl.LastIndexOf('/') + 1);
                    await _blobStorageService.RemoveFileAsync(fileName);
                }
                var fileUrl = await _blobStorageService.UploadFileAsync(photo, userId);
                var decodedFileUrl = Uri.UnescapeDataString(fileUrl);
                companyJob.PhotoUrl = decodedFileUrl;
                var updateLogo = await _jobPostService.UpdateCompanyJobPostLogoAsync(id, decodedFileUrl);
                return Ok(companyJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("removeLogo/{id}")]
        public async Task<IActionResult> RemoveAdLogo(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            try
            {
                var existingLogoUrl = companyJob.PhotoUrl;
                if (existingLogoUrl != null && existingLogoUrl != user.PhotoUrl)
                {
                    var fileName = existingLogoUrl.Substring(existingLogoUrl.LastIndexOf('/') + 1);
                    await _blobStorageService.RemoveFileAsync(fileName);
                }
                    
                companyJob.PhotoUrl = null;
                var updateLogo = await _jobPostService.UpdateCompanyJobPostLogoAsync(id, null);
                return Ok(companyJob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCompanyJobPost(int id, [FromBody] CompanyJobPostDto companyJobPostDto)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var updatedItem = await _jobPostService.UpdateCompanyJobPostAsync(companyJobPostDto);
            return Ok(updatedItem);
        }

        [HttpPatch("adinfo/{id}")]
        public async Task<IActionResult> UpdateAdInfo(int id, [FromBody] CompanyUpdateAdInfoRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");

            var sanitizer = new HtmlSanitizer();
            string sanitizedDescription = sanitizer.Sanitize(companyJobPostDto.JobDescription);
            companyJob.JobDescription = sanitizedDescription;
            companyJob.CityId = companyJobPostDto.CityId;
            companyJob.JobCategoryId = companyJobPostDto.JobCategoryId;
            companyJob.JobTypeId = companyJobPostDto.JobTypeId;
            companyJob.EmailForReceivingApplications = companyJobPostDto.EmailForReceivingApplications;
            companyJob.Position = companyJobPostDto.Position;
            companyJob.CompanyName = companyJobPostDto.CompanyName;
            companyJob.ApplyViaExternalPlatform = companyJobPostDto.ApplyViaExternalPlatform;
            if(companyJob.ApplyViaExternalPlatform == true)
                companyJob.ExternalPlatformApplicationUrl = companyJobPostDto.ExternalPlatformApplicationUrl;
            var updatedItem = await _jobPostService.UpdateCompanyJobPostAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("compensation-and-environment/{id}")]
        public async Task<IActionResult> UpdateCompensationAndWorkEnvInfo(int id, [FromBody] CompanyUpdateCompensationAndWorkEnvRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.MinSalary = companyJobPostDto.MinSalary;
            companyJob.MaxSalary = companyJobPostDto.MaxSalary;
            var sanitizer = new HtmlSanitizer();
            string sanitizedBenefits = sanitizer.Sanitize(companyJobPostDto.Benefits);
            string sanitizedWorkEnv = sanitizer.Sanitize(companyJobPostDto.WorkEnvironmentDescription);

            companyJob.Benefits = sanitizedBenefits;
            companyJob.WorkEnvironmentDescription = sanitizedWorkEnv;
            var updatedItem = await _jobPostService.UpdateCompensationAndWorkEnvAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("qualifications-and-experience/{id}")]
        public async Task<IActionResult> UpdateQualificationsAndExperience(int id, [FromBody] CompanyUpdateQualificationsAndExperienceRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.RequiredSkills = companyJobPostDto.RequiredSkills;
            companyJob.EducationLevelId = companyJobPostDto.EducationLevelId;
            companyJob.Certifications = companyJobPostDto.Certifications;
            companyJob.RequiredExperience = companyJobPostDto.RequiredExperience;
            companyJob.Languages = companyJobPostDto.Languages;
            var updatedItem = await _jobPostService.UpdateQualificationsAndExpereinceAsync(companyJob);
            return Ok(updatedItem);
        }

        [HttpPatch("howtoapply/{id}")]
        public async Task<IActionResult> UpdateHowToApply(int id, [FromBody] CompanyUpdateHowToApplyRequest companyJobPostDto)
        {
            var currentUserId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, currentUserId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            companyJob.HowToApply = companyJobPostDto.HowToApply;
            companyJob.DocumentsRequired = companyJobPostDto.DocumentsRequired;
            var updatedItem = await _jobPostService.UpdateHowToApplyAsync(companyJob);
            return Ok(updatedItem);
        }

        private async Task<bool> CheckDoesAdBelongsToUser(CompanyJobPostDto companyJob, int userId)
        {
            if (companyJob != null && companyJob.SubmittingUserId != userId)
                return false;
            return true;
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var deleted = await _jobPostService.DeleteCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(deleted);
        }

        [HttpPatch("close/{id}")]
        public async Task<IActionResult> CloseCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var closed = await _jobPostService.CloseCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(closed);
        }

        [HttpPatch("reactivate/{id}")]
        public async Task<IActionResult> ReactivateCompanyJobPost(int id)
        {
            var userId = HttpContext.User.GetUserId();
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || user.CompanyId == null)
                return Unauthorized("You do not belong to the current company.");
            var companyJob = await _jobPostService.GetCompanyJobPostByIdAsync(id);
            var valid = await CheckDoesAdBelongsToUser(companyJob, userId);
            if (!valid)
                return Unauthorized("Nemate pravo pristupa ovom oglasu");
            var reactivated = await _jobPostService.ReactivateCompanyJobPostByIdAsync((int)user.CompanyId, id);
            return Ok(reactivated);
        }
    }
}
