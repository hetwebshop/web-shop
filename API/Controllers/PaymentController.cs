using API.Data;
using API.DTOs;
using API.Entities.Payment;
using API.Extensions;
using API.Services.Payment;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        private IPaymentService _paymentService;
        private ILogger<PaymentController> _logger;
        private readonly DataContext _dbContext;
        private string MerchantKey;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, DataContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _paymentService = paymentService;
            _dbContext = dbContext;
            MerchantKey = configuration.GetSection("MonriMerchantKey").Value;
        }
        [HttpPost("initialize-payment")]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequestDto req)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Nemate pravo pristupa");
            try
            {
                if (req.Amount <= 0)
                {
                    _logger.LogError("Iznos mora biti veći od 0");
                    return BadRequest("Iznos mora biti veći od 0");
                }
                var paymentSession = new CreatePaymentSessionDto()
                {
                    order_info = "Dopuna kredita putem kartice",
                    order_number = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + "_" + userId,
                    currency = "BAM",
                    transaction_type = "purchase",
                    amount = req.Amount,
                    supported_payment_methods = new List<string>
                    {
                        "card"
                    }.ToArray(),
                    scenario = "charge"
                };
                var createPaymentSession = await _paymentService.CreatePaymentSession(paymentSession);
                return Ok(createPaymentSession);
            }
            catch(Exception ex)
            {
                _logger.LogError("Issue with creating new payment session");
                return StatusCode(500, "Došlo je do greške prilikom inicijalizacije plaćanja");
            }
        }

        [HttpPost("after-payment-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> CallbackAfterPayment()
        {
            try
            {
                // Enable buffering to allow reading the request body multiple times
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                var rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                // Log the raw body for debugging (ensure to avoid sensitive information)
                _logger.LogInformation("Received payment callback with body: {RawBody}", rawBody);

                // Extract and validate the Authorization header
                var authHeader = Request.Headers["authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("WP3-callback "))
                {
                    _logger.LogWarning("Invalid or missing Authorization header.");
                    return Unauthorized("Missing or invalid Authorization header");
                }

                var receivedDigest = authHeader.Replace("WP3-callback ", "").Trim();
                var calculatedDigest = CalculateSha512Hash(MerchantKey + rawBody);

                // Validate the digest
                if (!string.Equals(receivedDigest, calculatedDigest, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Digest mismatch. Potential spoofed request.");
                    return Unauthorized("Invalid digest");
                }

                // Deserialize the request body into the expected object
                var request = System.Text.Json.JsonSerializer.Deserialize<MonriCallbackRequest>(rawBody);

                if (request == null)
                {
                    _logger.LogWarning("Failed to deserialize the payment callback request.");
                    return BadRequest("Invalid request format");
                }

                // Extract user ID and validate
                var userId = int.Parse(request.order_number.Split("_")[1]);
                var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return NotFound("User not found");
                }

                // Process the payment and update the user's credits
                var amount = request.amount / 100.0;
                user.Credits += amount;
                _dbContext.Update(user);

                // Create the transaction record
                var userTransaction = new UserTransaction()
                {
                    Amount = amount,
                    UserId = userId,
                    CreatedAt = request.created_at,
                    ChFullName = request.ch_full_name,
                    IsProcessed = false,
                    OrderNumber = request.order_number,
                    ResponseCode = request.response_code,
                    ResponseMessage = request.response_message,
                    TransactionType = TransactionType.CreditsPurchase,
                    IsAddingCredits = true,
                    OrderInfo = OrderInfoMessages.CreditsPurchaseMessage,
                    MonriTransactionId = request.id
                };

                await _dbContext.UserTransactions.AddAsync(userTransaction);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Payment callback processed successfully for OrderNumber: {OrderNumber}, UserId: {UserId}", request.order_number, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the process
                _logger.LogError(ex, "An error occurred while processing the payment callback.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("user-transactions")]
        public async Task<IActionResult> GetAllUserTransactions([FromBody] PaginationRequest paginationRequest)
        {
            var userId = HttpContext.User.GetUserId();
            if (userId == null)
                return Unauthorized("Nemate pravo pristupa");

            try
            {
                if (paginationRequest.PageNumber <= 0 || paginationRequest.PageSize <= 0)
                    return BadRequest("Broj stranice i veličina stranice moraju biti veći od 0");

                var query = _dbContext.UserTransactions.Where(r => r.UserId == userId);

                var totalCount = await query.CountAsync();

                var userTransactions = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
                    .Take(paginationRequest.PageSize)
                    .ToListAsync();

                var response = new PaginatedResponse<UserTransaction>
                {
                    Data = userTransactions,
                    TotalCount = totalCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue with fetching all user transactions");
                return StatusCode(500, "Došlo je do greške prilikom dohvaćanja svih transakcija korisnika");
            }
        }

        private static string CalculateSha512Hash(string input)
        {
            using var sha512 = SHA512.Create();
            var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    public class PaginationRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
