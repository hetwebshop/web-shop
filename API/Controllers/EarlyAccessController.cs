using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public EarlyAccessController(ILogger<EarlyAccessController> logger, DataContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                    return Ok(new { success = true, message = "Successfully registered for early access!" });

                var earlyAccess = new EarlyAccess()
                {
                    PhoneNumber = request.Phone,
                    Email = request.Email,
                    IsCompany = request.IsCompany
                };
                await _dbContext.EarlyAccesses.AddAsync(earlyAccess);
                await _dbContext.SaveChangesAsync();

                return Ok(new { success = true, message = "Successfully registered for early access!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering for early access.");
                return StatusCode(500, new { success = false, message = "An error occurred, please try again later." });
            }
        }
    }

    public class EarlyAccessRequestDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsCompany { get; set; }
    }
}
