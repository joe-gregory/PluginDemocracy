using System.Globalization;
using System.Net;
using System.Net.Mail;
using Azure.Core;
using PluginDemocracy.API.Translations;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;


namespace PluginDemocracy.API
{
    public class UtilityClass
    {
        private readonly IConfiguration _configuration;
        private readonly string mailJetApiKey;
        private readonly string mailJetSecretKey;
        private readonly string smtpHost;
        private readonly int smtpPort = 587;
        public readonly string WebAppBaseUrl;
        public List<string> SupportedLanguages = ["en-US", "es-MX"]; 
        public UtilityClass(IConfiguration configuration)
        {
            _configuration = configuration;
            
            mailJetApiKey = _configuration["MailJet:ApiKey"]?? string.Empty;
            mailJetSecretKey = _configuration["MailJet:SecretKey"]?? string.Empty;
            smtpHost = _configuration["MailJet:SmtpServer"]?? string.Empty;

            if (string.IsNullOrEmpty(mailJetApiKey)) throw new InvalidOperationException("Mailjet API key is not configured properly.");
            if (string.IsNullOrEmpty(mailJetSecretKey)) throw new InvalidOperationException("Mailjet secret key is not configured properly.");
            if (string.IsNullOrEmpty(smtpHost)) throw new InvalidOperationException("Mailjet Smtp Server is not configured properly.");

            WebAppBaseUrl = _configuration["WebAppBaseUrl"] ?? throw new Exception("WebAppBaseUrl not in appsettings file");
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.Credentials = new NetworkCredential(mailJetApiKey, mailJetSecretKey);
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress("info@plugindemocracy.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public string Translate(string text, CultureInfo culture)
        {
            return TranslationResources.ResourceManager.GetObject(text, culture) as string ?? "No matching translation found";
        }
        public string GetAllTranslationsInNewLines(string text)
        {
            string textToReturn = string.Empty;

            for(int i = 0; i < SupportedLanguages.Count; i++)
            {
                CultureInfo culture = new(SupportedLanguages[i]);
                textToReturn += TranslationResources.ResourceManager.GetObject(text, culture);
                if (i != SupportedLanguages.Count - 1) textToReturn += "/n";
            }

            return textToReturn;
        }
        public async Task SendConfirmationEmail(User user, PDAPIResponse apiResponse)
        {
            //Create new email confirmation token
            user.EmailConfirmationToken = Guid.NewGuid().ToString();

            //Build confirmation link
            string emailConfirmationLink = $"{WebAppBaseUrl}{FrontEndPages.ConfirmEmail}?userId={user.Id}&token={user.EmailConfirmationToken}";

            string emailBody = $"<h1 style=\"text-align: center; color:darkgreen\">{Translate(ResourceKeys.ConfirmEmailTitle, user.Culture)}</h1>\r\n<img src=\"https://pdstorageaccountname.blob.core.windows.net/pdblobcontainer/PluginDemocracyImage.png\" style=\"max-height: 200px; margin-left: auto; margin-right: auto; display:block;\"/>\r\n<p style=\"text-align: center;\">{Translate(ResourceKeys.ConfirmEmailP1, user.Culture)}</p>\r\n<p style=\"text-align: center;\">{Translate(ResourceKeys.ConfirmEmailP2, user.Culture)}:</p>\r\n<p style=\"text-align: center;\"><a href={emailConfirmationLink}>{Translate(ResourceKeys.ConfirmEmailLink, user.Culture)}</a></p>";
            //Send Email
            try
            {
                await SendEmailAsync(toEmail: user.Email, subject: Translate(ResourceKeys.ConfirmEmailTitle, user.Culture), body: emailBody);
                //Redirect to generic message page and add a message
                apiResponse.RedirectTo = FrontEndPages.GenericMessage;
                apiResponse.RedirectParameters["Title"] = Translate(ResourceKeys.ConfirmEmailTitle, user.Culture);
                apiResponse.RedirectParameters["Body"] = Translate(ResourceKeys.ConfirmEmailCheckInbox, user.Culture);
            }
            catch (Exception ex)
            {
                apiResponse.AddAlert("error", $"Error sending confirmation email: {ex.Message}");
            }
        }
    }
}
