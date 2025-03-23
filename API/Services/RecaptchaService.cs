using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Services
{
    public class RecaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(HttpClient httpClient, IConfiguration configuration, ILogger<RecaptchaService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> VerifyCaptchaAsync(string captchaResponse)
        {
            try
            {
                var secretKey = _configuration["GoogleRecaptchaSecretKey"];
                var verificationUrl = _configuration["GoogleRecaptchaVerificationUrl"];

                var response = await _httpClient.PostAsync(verificationUrl, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", secretKey),
                    new KeyValuePair<string, string>("response", captchaResponse)
                }));

                var result = await response.Content.ReadAsStringAsync();
                var captchaResult = JsonConvert.DeserializeObject<RecaptchaVerificationResponse>(result);

                return captchaResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify reCAPTCHA.");
                throw; // Rethrow the original exception so the calling code can handle it if needed
            }
        }

    }

    public class RecaptchaVerificationResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
