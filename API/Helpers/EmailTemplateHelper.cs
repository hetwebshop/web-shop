using Microsoft.Extensions.Configuration;
using System.Net;
using System;

namespace API.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string GenerateEmailTemplate(string subject, string htmlMessageBody, IConfiguration configuration)
        {
            string companyName = "POSLOVNIOGLASI";
            string companyLogoUrl = configuration.GetSection("CompanyLogoPng").Value;
            string companyWebsite =  configuration.GetSection("UIBaseUrl").Value;
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
                color: #66023C; /* Set text color */
                margin: 0;
                padding: 20px;
            }}
            .email-container {{
                max-width: 600px;
                margin: 0 auto;
                background: #ffffff;
                padding: 20px;
                border: 1px solid #e0e0e0;
                border-radius: 8px;
                border-color: #66023C;
            }}
            .header {{
                text-align: center;
                font-family: serif !important;
                font-size: 24px;
                font-weight: bold;
                color: #66023C; /* Logo text color */
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
                text-align: center;
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
                font-size: 14px;
                color: #66023C; /* Set footer text color */
                text-align: center;
                line-height: 1.5;
            }}
            .footer a {{
                text-decoration: none; /* Prevent underlining links */
                color: #66023C; /* Same color for all links */
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <!-- Essential Header Section -->
            <div class='header'>
                <a href='{companyWebsite}'>
                <img src='{companyLogoUrl}' alt='{companyName} Logo' style='max-width: 500px; height: auto;' />
            </a>
            </div>

            <!-- Main Content Section -->
            <div class='content'>
                <h3 style='text-align: center;'>{subject}</h3>
                {htmlMessageBody}
            </div>

            <!-- Footer Section -->
            <div class='footer'>
                <p>Srdačno,<br><a href={companyWebsite}><strong style='font-style: serif !important; pointer-events: none; text-decoration: none; color: #66023C; cursor: default;'>{companyName}</strong></a></p>
                <p>Kontaktirajte nas na: <a href='mailto:{supportEmail}'>{supportEmail}</a><br>ili pozovite {contactNumber}</p>
                <p>Posjetite nas: <a href='{companyWebsite}'>{companyWebsite}</a></p>
            </div>
        </div>
    </body>
    </html>
    ";
        }

    }
}
