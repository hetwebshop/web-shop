using API.Data.IUserOfferRepository;
using API.Data;
using API.Services.UserOfferServices;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using System;
using API.Entities.Chat;
using API.Entities.Notification;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using API.Entities.CompanyJobPost;
using API.Entities.JobPost;
using API.Entities;
using SendGrid.Helpers.Mail;
using API.Entities.Applications;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class ConversationDto
    {
        public int Id { get; set; }  // Unique conversation identifier
        public int FromUserId { get; set; }  // Sender
        public int ToUserId { get; set; }  // Receiver
        public int? UserJobPostId { get; set; }  // Job post by user
        public int? CompanyJobPostId { get; set; }  // Job post by company
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessageDto> Messages { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }
        public bool IsUnread { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }
        public string UserFullName { get; set; }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Message { get; set; }  // Message text
        public int FromUserId { get; set; }  // Sender
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Authorize]
    public class ConversationController : BaseController
    {
        private readonly IUserJobPostService _jobPostService;
        private readonly IUnitOfWork _uow;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUserJobPostRepository _userJobPostRepository;
        private readonly DataContext _dbContext;
        private readonly IEmailService emailService;
        private readonly string UIBaseUrl;
        private readonly IConfiguration _configuration;
        private IUserApplicationsRepository userApplicationsRepository;
        private readonly ILogger<ConversationController> _logger;
        private readonly int CompanyAdActiveDays;

        public ConversationController(IUserJobPostService jobPostService, IUnitOfWork uow, IBlobStorageService blobStorageService, IUserJobPostRepository userJobPostRepository, DataContext dbContext, IEmailService emailService, IConfiguration configuration, IUserApplicationsRepository userApplicationsRepository, ILogger<ConversationController> logger)
        {
            _jobPostService = jobPostService;
            _uow = uow;
            _blobStorageService = blobStorageService;
            _userJobPostRepository = userJobPostRepository;
            _dbContext = dbContext;
            this.emailService = emailService;
            UIBaseUrl = configuration.GetSection("UIBaseUrl").Value;
            _configuration = configuration;
            this.userApplicationsRepository = userApplicationsRepository;
            _logger = logger;
            CompanyAdActiveDays = int.Parse(configuration.GetSection("CompanyActiveAdDays").Value);
        }

        [HttpPost("contactuser/{userAdId}")]
        public async Task<IActionResult> SendContactMessage(int userAdId, [FromBody] ContactUserRequestDto request)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return BadRequest(new { message = "Korisnik je obavezan!" });
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Email i poruka su obavezni!" });
            }

            var userJobPost = await _jobPostService.GetUserJobPostByIdAsync(userAdId);
            var user = await _uow.UserRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsCompany)
                return Unauthorized("Nemate pravo pristupa");
            if (userJobPost == null)
                return BadRequest(new { message = "Oglas nije pronadjen!" });

            var now = DateTime.UtcNow;
            var conversation = new Conversation()
            {
                UserJobPostId = userAdId,
                FromUserId = userId,
                CreatedAt = now,
                ToUserId = userJobPost.SubmittingUserId
            };

            await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();

            var chatMessage = new ChatMessage()
            {
                ConversationId = conversation.Id,
                CreatedAt = now,
                FromUserId = userId,
                Message = request.Message,
            };

            await _dbContext.ChatMessages.AddAsync(chatMessage);
            await _dbContext.SaveChangesAsync();

            var notification = new Notification()
            {
                UserId = userJobPost.SubmittingUserId.ToString(),
                CreatedAt = now,
                IsRead = false,
                Link = UIBaseUrl + $"conversations/{conversation.Id}",
                Message = "Nova poslovna prilika! Poslodavac Vam je poslao poruku."
            };
            _dbContext.Notifications.Add(notification);

            await _dbContext.SaveChangesAsync();

            // Fire-and-forget email sending using Task.Run
            Task.Run(() => SendNewContactUserEmailAsync(userJobPost, conversation.Id, request.Message, request.Subject, request.Phone, request.Email));

            return Ok(new { message = "Poruka je uspešno poslana!" });
        }

        //This is triggered when company replys to user application
        [HttpPost("newconversationandmessage")]
        public async Task<IActionResult> CreateNewConversationAndSendMessage([FromBody] ReplyToUserApplicationRequest request)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                if (userId == null)
                    return BadRequest(new { message = "Korisnik je obavezan!" });
                var user = await _uow.UserRepository.GetUserByIdAsync(userId);
                if (user == null || !user.IsCompany)
                    return Unauthorized("Nemate pravo pristupa");

                var userApplication = await userApplicationsRepository.GetUserApplicationByIdAsync(request.UserApplicationId);
                if(userApplication.CreatedAt.AddDays(CompanyAdActiveDays + 20) < DateTime.UtcNow)//Added 20 more days because UserApplication can happen before company ad ended. This value can be changed to 10, or 5, or 25..
                {
                    return BadRequest("Aplikacija je istekla. Ne mozete pokrenuti konverzaciju.");
                }
                if (userApplication != null)
                {
                    var companyJobPost = await _dbContext.CompanyJobPosts.FirstOrDefaultAsync(r => r.Id == userApplication.CompanyJobPostId);
                    if (companyJobPost.SubmittingUserId != userId)
                    {
                        return Forbid("Zabranjen pristup");
                    }
                    var now = DateTime.UtcNow;
                    var conversation = new Conversation()
                    {
                        CompanyJobPostId = companyJobPost.Id,
                        FromUserId = userId,
                        CreatedAt = now,
                        ToUserId = userApplication.SubmittingUserId
                    };
                    await _dbContext.Conversations.AddAsync(conversation);
                    await _dbContext.SaveChangesAsync();

                    var chatMessage = new ChatMessage()
                    {
                        ConversationId = conversation.Id,
                        CreatedAt = now,
                        FromUserId = userId,
                        Message = request.Message,
                    };

                    await _dbContext.ChatMessages.AddAsync(chatMessage);
                    await _dbContext.SaveChangesAsync();

                    var notification = new Notification()
                    {
                        UserId = userApplication.SubmittingUserId.ToString(),
                        CreatedAt = now,
                        IsRead = false,
                        Link = UIBaseUrl + $"conversations/{conversation.Id}",
                        Message = "Nova poslovna prilika! Poslodavac Vam je poslao poruku."
                    };
                    _dbContext.Notifications.Add(notification);

                    await _dbContext.SaveChangesAsync();


                    // Fire-and-forget email sending using Task.Run
                    Task.Run(() => SendNewReplyToUserApplicationEmailAsync(userApplication, conversation.Id, request.Message, companyJobPost.EmailForReceivingApplications));

                    return Ok(conversation.Id);
                }
                return NotFound("Aplikacija nije pronadjena");
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("companyconversations")]
        public async Task<IActionResult> GetAllCompanyConversations(int pageNumber = 1, int pageSize = 10)
        {
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId == null)
                return Unauthorized("Nemate pravo pristupa");

            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (!user.IsCompany)
                return Unauthorized("Nemate pravo pristupa");
            // Optimize the query to only select necessary columns
            var userConversations = await _dbContext.Conversations
                .Include(r => r.FromUser)
                .ThenInclude(r => r.Company)
                .Include(r => r.ToUser)
                .Where(r => r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(conversation => new
                {
                    conversation.Id,
                    conversation.FromUserId,
                    conversation.ToUserId,
                    conversation.CreatedAt,
                    conversation.CompanyJobPostId,
                    conversation.UserJobPostId,
                    conversation.ToUser.FirstName,
                    conversation.ToUser.LastName,
                    MessagesToCurrentUser = conversation.Messages.Where(r => r.FromUserId != currentUserId),
                    messages = conversation.Messages
                })
                .ToListAsync();

            // Get the total count (this might still be necessary for pagination)
            var totalConversationsCount = await _dbContext.Conversations
                .Where(r => r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                .CountAsync();

            var conversationsDto = userConversations.Select(item => new ConversationDto
            {
                Id = item.Id,
                FromUserId = item.FromUserId,
                ToUserId = item.ToUserId,
                CreatedAt = item.CreatedAt,
                UserFullName = item.FirstName + " " + item.LastName,
                CompanyJobPostId = item.CompanyJobPostId,
                UserJobPostId = item.UserJobPostId,
                IsUnread = item.MessagesToCurrentUser.Count(r => !r.IsRead) > 0,
                LastMessage = item.messages.OrderByDescending(r => r.CreatedAt).FirstOrDefault()?.Message,
                LastMessageDate = item.messages.OrderByDescending(r => r.CreatedAt).FirstOrDefault()?.CreatedAt
            }).ToList();

            return Ok(new
            {
                conversations = conversationsDto,
                totalConversationsCount
            });
        }

        [HttpGet("userconversations")]
        public async Task<IActionResult> GetAllUserConversations(int pageNumber = 1, int pageSize = 10)
        {
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId == null)
                return Unauthorized("Nemate pravo pristupa");

            var user = await _uow.UserRepository.GetUserByIdAsync(currentUserId);
            if (user.IsCompany)
                return Unauthorized("Nemate pravo pristupa");
            // Optimize the query to only select necessary columns
            var userConversations = await _dbContext.Conversations
                .Include(r => r.FromUser)
                .ThenInclude(r => r.Company)
                .Where(r => r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(conversation => new
                {
                    conversation.Id,
                    conversation.FromUserId,
                    conversation.ToUserId,
                    conversation.CreatedAt,
                    conversation.CompanyJobPostId,
                    conversation.UserJobPostId,
                    conversation.FromUser.Company.CompanyName,
                    conversation.FromUser.Company.PhotoUrl,
                    MessagesToCurrentUser = conversation.Messages.Where(r => r.FromUserId != currentUserId),
                    messages = conversation.Messages
                })
                .ToListAsync();

            // Get the total count (this might still be necessary for pagination)
            var totalConversationsCount = await _dbContext.Conversations
                .Where(r => r.ToUserId == currentUserId)
                .CountAsync();

            var conversationsDto = userConversations.Select(item => new ConversationDto
            {
                Id = item.Id,
                FromUserId = item.FromUserId,
                ToUserId = item.ToUserId,
                CreatedAt = item.CreatedAt,
                CompanyName = item.CompanyName,
                CompanyLogoUrl = item.PhotoUrl,
                CompanyJobPostId = item.CompanyJobPostId,
                UserJobPostId = item.UserJobPostId,
                IsUnread = item.MessagesToCurrentUser.Count(r => !r.IsRead) > 0,
                LastMessage = item.messages.OrderByDescending(r => r.CreatedAt).FirstOrDefault()?.Message,
                LastMessageDate = item.messages.OrderByDescending(r => r.CreatedAt).FirstOrDefault()?.CreatedAt
            }).ToList();

            return Ok(new
            {
                conversations = conversationsDto,
                totalConversationsCount
            });
        }


        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(int conversationId, int pageNumber = 1, int pageSize = 50)
        {
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId == null)
                return Unauthorized("Nemate pravo pristupa");

            var messagesQuery = _dbContext.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt) // Newest messages first
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var totalMessagesCount = await _dbContext.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .CountAsync();

            var messages = await messagesQuery
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    FromUserId = m.FromUserId,
                    Message = m.Message,
                    CreatedAt = m.CreatedAt,
                    ConversationId = m.ConversationId
                })
                .ToListAsync();

            return Ok(new
            {
                messages,
                totalMessagesCount
            });
        }

        [HttpGet("messages/unread")]
        public async Task<IActionResult> GetAllUserUnreadMessages()
        {
            var currentUserId = HttpContext.User.GetUserId();
            if (currentUserId == null)
                return Unauthorized("Nemate pravo pristupa");

            var unreadMessageCounts = await _dbContext.Conversations
             .Where(c => c.ToUserId == currentUserId || c.FromUserId == currentUserId) 
             .Select(c => new
             {
                 ConversationId = c.Id,
                 UnreadMessages = c.Messages.Where(r => r.FromUserId != currentUserId).Count(m => !m.IsRead) 
             })
             .Where(c => c.UnreadMessages > 0)
             .ToListAsync();

            return Ok(unreadMessageCounts.Count());
        }

        [HttpPost("messages/{conversationId}")]
        public async Task<IActionResult> SendNewMessage(int conversationId, [FromBody]NewMessageRequest req)
        {
            try
            {
                var currentUserId = HttpContext.User.GetUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Nemate pravo pristupa");
                }

                if (string.IsNullOrWhiteSpace(req.Message?.Trim()))
                {
                    return BadRequest("Poruka ne može biti prazna");
                }

                var conversation = await _dbContext.Conversations.FirstOrDefaultAsync(r => r.Id == conversationId);
                if(conversation.FromUserId != currentUserId && conversation.ToUserId != conversationId)
                {
                    return Unauthorized("Nemate pravo pristupa");
                }

                var chatMessage = new ChatMessage()
                {
                    ConversationId = conversationId,
                    Message = req.Message,
                    CreatedAt = DateTime.UtcNow,
                    FromUserId = currentUserId,
                    IsRead = false
                };

                await _dbContext.ChatMessages.AddAsync(chatMessage);
                await _dbContext.SaveChangesAsync();

                var chatMessageDto = new ChatMessageDto()
                {
                    ConversationId = conversationId,
                    CreatedAt = chatMessage.CreatedAt,
                    FromUserId = chatMessage.FromUserId,
                    Message = chatMessage.Message,
                    Id = chatMessage.Id
                };
                return Ok(chatMessageDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost("messages/setread/{conversationId}")]
        public async Task<IActionResult> SetAllMessagesFromConversationAsRead(int conversationId)
        {
            try
            {
                var currentUserId = HttpContext.User.GetUserId();
                if (currentUserId == null)
                {
                    return Unauthorized("Nemate pravo pristupa");
                }

                var chatMessages = await _dbContext.ChatMessages.Include(r => r.Conversation)
                .Where(r => r.FromUserId != currentUserId && r.ConversationId == conversationId)
                .ToListAsync();
                
                if(chatMessages == null || !chatMessages.Any())
                {
                    return Ok();
                }
                var conversation = chatMessages.First().Conversation;
                if(conversation.ToUserId != currentUserId)
                {
                    return Unauthorized("Nemate pravo pristupa");
                }

                foreach (var chat in chatMessages)
                {
                    chat.IsRead = true;
                }
                _dbContext.ChatMessages.UpdateRange(chatMessages);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("userjoboffers")]
        public async Task<IActionResult> GetAllUserJobOffers()
        {
            try
            {
                var currentUserId = HttpContext.User.GetUserId();
                if (currentUserId == null)
                    return Unauthorized("Nemate pravo pristupa");
                var userJobOffers = await _dbContext.Conversations.Where(r => r.ToUserId == currentUserId).Include(e => e.FromUser).ThenInclude(r => r.Company).Include(r => r.UserJobPost).OrderByDescending(r => r.CreatedAt).ToListAsync();
                var userJobOffersTableData = new List<UserJobOffersTableData>();
                foreach (var jobOffer in userJobOffers)
                {
                    var tableData = new UserJobOffersTableData()
                    {
                        JobPosition = jobOffer.UserJobPost?.Position,
                        CreatedAt = jobOffer.CreatedAt,
                        CompanyName = jobOffer.FromUser?.Company?.CompanyName,
                        UserJobPostId = jobOffer.UserJobPostId.HasValue ? jobOffer.UserJobPostId.Value : 0,
                        Phone = jobOffer.FromUser?.Company?.PhoneNumber,
                        Email = jobOffer.FromUser?.Email,
                        //Message = jobOffer.Message,
                        //Subject = jobOffer.Subject,
                        Id = jobOffer.Id
                    };
                    userJobOffersTableData.Add(tableData);
                }
                return Ok(userJobOffersTableData);
            }
            catch(Exception ex)
            {
                _logger.LogError($"There was an error while trying to fetch all user job offers: {ex.Message}");
                return StatusCode(500, "Došlo je do greške na serveru. Molimo pokušajte ponovo kasnije.");
            }
        }

        private async Task SendNewContactUserEmailAsync(UserJobPostDto userAd, int conversationId, string message, string requestSubject, string phone, string email)
        {
            if (userAd != null)
            {
                var applicationUrl = UIBaseUrl + $"conversations/{conversationId}";

                string messageBody = $@"
            <h4 style='color: black;'>Naslov: {requestSubject}</h4>
            <p style='color: #66023C;'>Poruka: {message}</p>
            <p style='color: #66023C;'>Email poslodavca: {email}</p>
            <p style='color: #66023C;'>Broj telefona poslodavca: {phone}</p>
            <p style='text-align: center;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte oglas</a>
            </p>";

                var subject = "POSLOVNIOGLASI - Nova poslovna prilika! Poslodavac Vas želi kontaktirati";
                var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);

                try
                {
                    await emailService.SendEmailWithTemplateAsync(userAd.ApplicantEmail, subject, emailTemplate);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong while trying to send contact user email {ex.Message}");
                    throw;
                }
            }
        }

        private async Task SendNewReplyToUserApplicationEmailAsync(UserApplication userApplication, int conversationId, string message, string email)
        {
            if (userApplication != null)
            {
                var applicationUrl = UIBaseUrl + $"conversations/{conversationId}";

                string messageBody = $@"
            <h4 style='color: black;'>Naslov: Poslodavac je odgovorio na vašu aplikaciju za posao!</h4>
            <p style='color: #66023C;'>Poruka: {message}</p>
            <p style='color: #66023C;'>Email poslodavca: {email}</p>
            <p style='text-align: center;'>
                <a href='{applicationUrl}' style='display: inline-block; padding: 10px 20px; background-color: #66023C; color: #ffffff; text-decoration: none; border-radius: 5px;'>Pogledajte oglas</a>
            </p>";

                var subject = "POSLOVNIOGLASI - Nova poslovna prilika! Poslodavac Vam je odgovorio na aplikaciju";
                var emailTemplate = EmailTemplateHelper.GenerateEmailTemplate(subject, messageBody, _configuration);

                try
                {
                    await emailService.SendEmailWithTemplateAsync(userApplication.Email, subject, emailTemplate);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
