using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using PluginDemocracy.API.Translations;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;

namespace PluginDemocracy.API
{
    /// <summary>
    /// This is a scoped service so it cannot hold state data.
    /// </summary>
    public class ApiUtilityClass
    {
        private readonly IConfiguration _configuration;
        private readonly string mailJetApiKey;
        private readonly string mailJetSecretKey;
        private readonly string smtpHost;
        private readonly int smtpPort = 587;
        public readonly string WebAppBaseUrl;
        public List<string> SupportedLanguages = ["en-US", "es-MX"]; 
        public readonly PluginDemocracyContext _context;
        public ApiUtilityClass(IConfiguration configuration, PluginDemocracyContext context)
        {
            _configuration = configuration;
            _context = context;
            
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
            try
            {
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                apiResponse.AddAlert("error", "Error saving changes to database: " + e.Message);
            }

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
        public string CreateJsonWebToken(int userId)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            
            string secret = Environment.GetEnvironmentVariable("JsonWebTokenSecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secret)) throw new Exception("JsonWebTokenSecretKey is null or empty");
            byte[] key = Encoding.ASCII.GetBytes(secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public int? ReturnUserIdFromJsonWebToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            string secret = Environment.GetEnvironmentVariable("JsonWebTokenSecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secret)) throw new Exception("JsonWebTokenSecretKey is null or empty");
            byte[] key = Encoding.ASCII.GetBytes(secret);

            //Determines if the string is a well formed Json Web Token (JWT).
            if (!tokenHandler.CanReadToken(token)) return null;
            
            //Ensure I sent this token using the security key
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, //Issuer is the server that created the token
                ValidateAudience = false, //Audience is the intended recipient of the token
                ClockSkew = TimeSpan.Zero //ClockSkew is used to determine if a token is valid or not
            };
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            //Get the user Id
            JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(token);
            //string userId = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            // Instead of using ClaimTypes.Name, I'm using the string I found using a website. Not sure why I can't use ClaimTypes.Name
            string userId = jwtSecurityToken.Claims.First(claim => claim.Type == "unique_name").Value;

            return int.Parse(userId);
        }
        /// <summary>
        /// Will return true when the token has expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>k
        /// 
        public bool HasJWTExpired(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken jwtToken;

            jwtToken = tokenHandler.ReadJwtToken(token);

            if(jwtToken == null) throw new Exception("JWT token is null");

            //Extract expiration claim
            var expClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
            if(expClaim == null) throw new Exception("JWT token does not have an expiration claim");

            var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));

            return expTime < DateTimeOffset.UtcNow;
        }
    }
}
