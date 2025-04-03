using API.Data;
using API.Entities;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class EarlyAccessController : BaseController
    {
        private readonly ILogger<EarlyAccessController> _logger;
        private readonly DataContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration configuration;

        public EarlyAccessController(ILogger<EarlyAccessController> logger, DataContext dbContext, IEmailService emailService, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _emailService = emailService;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterForEarlyAccess([FromBody] EarlyAccessRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Request body is null.");
            }
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest("Email and Phone are required.");
            }

            try
            {
                var check = await _dbContext.EarlyAccesses.FirstOrDefaultAsync(r =>
                    (string.IsNullOrEmpty(request.Email) || r.Email == request.Email) &&
                    (string.IsNullOrEmpty(request.Phone) || r.PhoneNumber == request.Phone)
                );
                if(check != null)
                {
                    if (!string.IsNullOrEmpty(check.Email))
                        SendEmailAync(request.IsCompany, check.Email);
                    else if (!string.IsNullOrEmpty(request.Email))
                        SendEmailAync(request.IsCompany, request.Email);
                        return Ok(new { success = true, message = "Successfully registered for early access!" });
                }

                var earlyAccess = new EarlyAccess()
                {
                    PhoneNumber = request.Phone,
                    Email = request.Email,
                    IsCompany = request.IsCompany,
                    SubmittedOn = DateTime.UtcNow
                };
                await _dbContext.EarlyAccesses.AddAsync(earlyAccess);
                await _dbContext.SaveChangesAsync();
                if(!string.IsNullOrWhiteSpace(request.Email))
                    SendEmailAync(request.IsCompany, request.Email);
                return Ok(new { success = true, message = "Successfully registered for early access!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering for early access.");
                return StatusCode(500, new { success = false, message = "An error occurred, please try again later." });
            }
        }

        private async Task SendEmailAync(bool isCompany, string recipientEmail)
        {
            string subject = "🎉 Čestitamo! Osvojili ste besplatnu objavu oglasa!";

            string messageCompany = $@"
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


            string messageUser = $@"
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

            string emailHtml = EmailTemplateHelper.GenerateEmailTemplate(subject, isCompany ? messageCompany : messageUser, configuration);

            // Send the email
            await _emailService.SendEmailWithTemplateAsync(
                recipientEmail,
                subject,
                emailHtml
            );
        }
    }

    public class EarlyAccessRequestDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsCompany { get; set; }
    }
}
