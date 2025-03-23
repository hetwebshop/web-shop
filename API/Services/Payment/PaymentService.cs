using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using API.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace API.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private string _baseUrl;
        private string _authenticityToken;
        private string _key;
        private ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _baseUrl = configuration.GetSection("MonriBaseUrl").Value;
            _authenticityToken = configuration.GetSection("MonriAuthKey").Value;
            _key = configuration.GetSection("MonriMerchantKey").Value;
        }
        public async Task<PaymentSessionResponseDto> CreatePaymentSession(CreatePaymentSessionDto req)
        {
            try
            {
                var bodyAsString = JsonConvert.SerializeObject(req);
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Generate the digest
                var digest = GenerateDigest(bodyAsString, timestamp);

                var authorization = $"WP3-v2 {_authenticityToken} {timestamp} {digest}";

                using (var client = new HttpClient())
                {
                    // Set request headers
                    client.DefaultRequestHeaders.Add("Authorization", authorization);

                    // Create the HTTP request content
                    var content = new StringContent(bodyAsString, Encoding.UTF8, "application/json");

                    try
                    {
                        // Send POST request
                        var response = await client.PostAsync($"{_baseUrl}/v2/payment/new", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                            return new PaymentSessionResponseDto { Status = "approved", ClientSecret = jsonResponse["client_secret"].ToString() };
                        }
                        else
                        {
                            return new PaymentSessionResponseDto { Status = "declined", Error = response.ReasonPhrase };
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"There was an issue while trying to create new payment: {ex.Message}");
                        return new PaymentSessionResponseDto  { ClientSecret = null, Status = "declined", Error = ex.Message };
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"There was an in creating new payment session: {ex.Message}");
                throw;
            }
        }

        private string GenerateDigest(string bodyAsString, long timestamp)
        {
            using (var sha512 = SHA512.Create())
            {
                var rawData = $"{_key}{timestamp}{_authenticityToken}{bodyAsString}";
                var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
