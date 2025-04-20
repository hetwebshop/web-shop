using Microsoft.Extensions.Configuration;
using System.Net;
using System;

namespace jobify_azure_function
{
    public static class EmailTemplateHelper
    {
        public static string GenerateEmailTemplate(string subject, string htmlMessageBody, IConfiguration configuration)
        {
            string companyName = "POSLOVNIOGLASI";
            string companyLogoUrl = configuration.GetSection("CompanyLogoPng").Value;
            string companyWebsite = configuration.GetSection("UIBaseUrl").Value;
            string supportEmail = configuration.GetSection("SupportEmail").Value;
            string contactNumber = configuration.GetSection("SupportPhoneNumber").Value;

            return $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            @import url('https://fonts.googleapis.com/css2?family=Lora:wght@400;700&display=swap');

            body {{
                font-family: Arial, sans-serif;
                background-color: #f8f9fa;
                color: black !important; /* Set text color */
                margin: 0;
                padding: 20px;
            }}
            .email-container {{
                max-width: 600px;
                background: #ffffff;
                padding: 20px;
            }}
            .header {{
                text-align: start;
                font-family: serif !important;
                font-size: 24px;
                font-weight: bold;
                color: black !important; /* Logo text color */
                margin-bottom: 20px;
                pointer-events: none;
    }}
            .header a {{
                text-decoration: none;
                color: #66023C; /* Prevent underlining of logo */
                pointer-events: none; /* Prevent behaving like a URL */
            }}
            .content {{
                margin-top: 20px;
                line-height: 1.6;
                text-align: start;
                color: black !important;
            }}
        span {{
            color: black !important;
        }}
            .button {{
                display: inline-block;
                padding: 10px 20px;
                background-color: #66023C;
                color: #ffffff;
                text-decoration: none;
                font-size: 16px;
                border-radius: 5px;
                text-align: center;
                margin-top: 20px;
            }}
            .footer {{
                margin-top: 20px;
                font-size: 13px;
                color: black !important; /* Set footer text color */
                text-align: start;
                line-height: 1.5;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <!-- Essential Header Section -->
            <div class='header'>
                <a href='{companyWebsite}'>
                <img src='{companyLogoUrl}' alt='' style='max-width: 400px; border-radius:8px;' />
            </a>
            </div>

            <!-- Main Content Section -->
            <div class='content'>
                <h3 style='text-align: start;'>{subject}</h3>
                {htmlMessageBody}
            </div>

            <!-- Footer Section -->
            <div class='footer'>
    <p>Za sva pitanja, tu smo za Vas! Pišite nam na <a href='mailto:{supportEmail}'>{supportEmail}</a>.</p>
                <p>Posjetite nas na našoj web stranici:  
                    <a href='{companyWebsite}' target='_blank' rel='noopener noreferrer'>poslovnioglasi.ba</a>.
                </p>
            </div>


        </div>
    </body>
    </html>
    ";
        }

    }
}
