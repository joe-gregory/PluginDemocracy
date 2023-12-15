﻿using System.Globalization;
using System.Net;
using System.Net.Mail;
using PluginDemocracy.API.Translations;


namespace PluginDemocracy.API
{
    public class UtilityClass
    {
        private readonly IConfiguration _configuration;
        private readonly string mailJetApiKey;
        private readonly string mailJetSecretKey;
        private readonly string smtpHost;
        private readonly int smtpPort = 587;
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
                CultureInfo culture = new CultureInfo(SupportedLanguages[i]);
                textToReturn += TranslationResources.ResourceManager.GetObject(text, culture);
                if (i != SupportedLanguages.Count - 1) textToReturn += "/n";
            }

            return textToReturn;
        }
    }
}