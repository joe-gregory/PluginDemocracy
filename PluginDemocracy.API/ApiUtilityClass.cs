using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PluginDemocracy.API.Translations;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.Data;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

namespace PluginDemocracy.API
{
    /// <summary>
    /// This is a scoped service so it cannot hold state data.
    /// </summary>
    public class APIUtilityClass
    {
        private readonly IConfiguration _configuration;
        private readonly string mailJetApiKey;
        private readonly string mailJetSecretKey;
        private readonly string smtpHost;
        private readonly int smtpPort = 587;
        public readonly string WebAppBaseUrl;
        public List<string> SupportedLanguages = ["en-US", "es-MX"];
        public readonly PluginDemocracyContext _context;
        public APIUtilityClass(IConfiguration configuration, PluginDemocracyContext context)
        {
            _configuration = configuration;
            _context = context;

            mailJetApiKey = _configuration["MailJet:ApiKey"] ?? string.Empty;
            mailJetSecretKey = _configuration["MailJet:SecretKey"] ?? string.Empty;
            smtpHost = _configuration["MailJet:SmtpServer"] ?? string.Empty;

            if (string.IsNullOrEmpty(mailJetApiKey)) throw new InvalidOperationException("Mailjet API key is not configured properly.");
            if (string.IsNullOrEmpty(mailJetSecretKey)) throw new InvalidOperationException("Mailjet secret key is not configured properly.");
            if (string.IsNullOrEmpty(smtpHost)) throw new InvalidOperationException("Mailjet Smtp Server is not configured properly.");

            WebAppBaseUrl = _configuration["WebAppBaseUrl"] ?? throw new Exception("WebAppBaseUrl not in appsettings file");
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using SmtpClient smtpClient = new(smtpHost, smtpPort);
            smtpClient.Credentials = new NetworkCredential(mailJetApiKey, mailJetSecretKey);
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new()
            {
                From = new MailAddress("info@plugindemocracy.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public string Translate(string text, CultureInfo? culture = null)
        {
            culture ??= new CultureInfo("en-US");
            return TranslationResources.ResourceManager.GetObject(text, culture) as string ?? "No matching translation found";
        }
        public string GetAllTranslationsInNewLines(string text)
        {
            string textToReturn = string.Empty;

            for (int i = 0; i < SupportedLanguages.Count; i++)
            {
                CultureInfo culture = new(SupportedLanguages[i]);
                textToReturn += TranslationResources.ResourceManager.GetObject(text, culture);
                if (i != SupportedLanguages.Count - 1) textToReturn += "/n";
            }

            return textToReturn;
        }
        /// <summary>
        /// This method will send an email to the users account with a link to confirm their email address. This method is used when the user's email
        /// is not confirmed and sends an email to allow confirmation. Examples of locations include when a user registers for the first time or when the user updates their email address.
        /// The email is sent to user.Email and the email body contains a link to confirm the email address. The link is built using the user's Id and a new email confirmation token which
        /// is stored in User.EmailConfirmationToken. This current implementation assigns a new Guid to User.EmailConfirmationToken. Future implementations could use a 
        /// JWT in order to set things such as expiration time and such.
        /// The email's title and body is built using the user's culture to and stored translation resources in TranslationResources.resx and their culture specific files.
        /// </summary>
        /// <param name="user">This is the user to which the email will be sent to at User.Email</param>
        /// <param name="apiResponse">This is the response the controller will send back to the frontend. It is passed to this method so that messages can be added to the response such as 
        /// if there were errors sending the email and such.</param>
        /// <returns></returns>
        public async Task SendConfirmationEmail(User user, PDAPIResponse apiResponse)
        {
            //Create new email confirmation token
            user.EmailConfirmationToken = Guid.NewGuid().ToString();
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                apiResponse.AddAlert("error", "Error saving User.EmailConfirmationToken to database: " + e.Message);
            }

            //Build confirmation link
            string emailConfirmationLink = $"{WebAppBaseUrl}{FrontEndPages.ConfirmEmail}?userId={user.Id}&token={user.EmailConfirmationToken}";

            string emailBody = $"<h1 style=\"text-align: center; color:darkgreen\">{Translate(ResourceKeys.ConfirmEmailTitle, user.Culture)}</h1>\r\n<img src=\"https://pdstorageaccountname.blob.core.windows.net/pdblobcontainer/UIAssets/PluginDemocracyImage.png\" style=\"max-height: 200px; margin-left: auto; margin-right: auto; display:block;\"/>\r\n<p style=\"text-align: center;\">{Translate(ResourceKeys.ConfirmEmailP1, user.Culture)}</p>\r\n<p style=\"text-align: center;\">{Translate(ResourceKeys.ConfirmEmailP2, user.Culture)}:</p>\r\n<p style=\"text-align: center;\"><a href={emailConfirmationLink}>{Translate(ResourceKeys.ConfirmEmailLink, user.Culture)}</a></p>";
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
        /// <summary>
        /// Creates a Json Web Token (JWT) with the user's Id as the payload and it uses an environment variable to set the signature. The token will expire in expirationDays days.
        /// </summary>
        /// <param name="userId">User.Id</param>
        /// <param name="expirationDays">Days from today when it expires</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string CreateJWT(int userId, int expirationDays)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            string secret = Environment.GetEnvironmentVariable("JsonWebTokenSecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secret)) throw new Exception("JsonWebTokenSecretKey is null or empty");
            byte[] key = Encoding.ASCII.GetBytes(secret);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, userId.ToString())
                ]),
                Expires = DateTime.UtcNow.AddDays(expirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Returns the userId stored in the JWT. This method uses IsJWTValid to check if the token is valid. If invalid, it returns null.
        /// </summary>
        /// <param name="token">The JWT</param>
        /// <param name="response">Optional PDAPIResponse object to add error messages.</param>
        /// <returns></returns>
        public int? ReturnUserIdFromJWT(string token, PDAPIResponse? response = null)
        {
            if (!IsJWTValid(token, response)) return null;

            //Get the user Id
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(token);
            string userIdString = jwtSecurityToken.Claims.First(claim => claim.Type == "unique_name").Value; // Instead of using ClaimTypes.Name, I'm using the string I found using a website. Not sure why I can't use ClaimTypes.Name

            try
            {
                int userId = int.Parse(userIdString);
                return userId;
            }
            catch (Exception e)
            {
                response?.AddAlert("error", $"Error parsing userId to int from JWT: {e.Message}");
                return null;
            }
        }
        /// <summary>
        /// Returns true if a JWT is valid. It checks the expiration date and that the signature match.  
        /// </summary>
        /// <param name="token">String token being checked</param>
        /// <param name="response">This is an optional parameter in case alert messages will be added that will later be passed to the frontend when the response is sent</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool IsJWTValid(string token, PDAPIResponse? response = null)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            // Check if the token is in a valid JWT format
            if (!tokenHandler.CanReadToken(token))
            {
                response?.AddAlert("error", "Invalid JWT format");
                return false;
            }

            string secret = Environment.GetEnvironmentVariable("JsonWebTokenSecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secret)) throw new Exception("JsonWebTokenSecretKey is null or empty");
            byte[] key = Encoding.ASCII.GetBytes(secret);
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, //Issuer is the server that created the token
                ValidateAudience = false, //Audience is the intended recipient of the token
                ClockSkew = TimeSpan.Zero //ClockSkew is used to determine if a token is valid or not
            };
            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            // Catch more specific exceptions related to token validation
            catch (SecurityTokenExpiredException e)
            {
                // Specific handling for expired tokens
                response?.AddAlert("error", $"Token expired: {e.Message}");
                return false;
            }
            catch (SecurityTokenException e)
            {
                response?.AddAlert("error", $"Error validating token: {e.Message}");
                return false;
            }
            catch (Exception e) // General exceptions could be logged or handled differently
            {
                // Consider logging the exception details for debugging purposes
                response?.AddAlert("error", $"Unexpected error during token validation: {e.Message}");
                return false;
            }
        }
        /// <summary>
        /// This method will extract the user from the claims in the userPrincipal and then fetch it from database. 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<User?> ReturnUserFromClaims(System.Security.Claims.ClaimsPrincipal userPrincipal, PDAPIResponse? response = null)
        {
            //Extract User from claims
            System.Security.Claims.Claim? userIdClaim = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (userIdClaim == null)
            {
                response?.AddAlert("error", "Unable to find userId in claims");
                return null;
            }
            // Now I can use this userId to fetch user details from my DbContext
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                response?.AddAlert("error", "Unable to parse userId from claims to int");
                return null;
            }
            try
            {
                User? user = await _context.Users
                    .Include(u => u.Notifications)
                    .Include(u => u.Roles)
                        .ThenInclude(r => r.Community)
                    .Include(u => u.PetitionDrafts)
                    .Include(u => u.ResidentOfHomes)
                        .ThenInclude(h => h.ResidentialCommunity)
                    .Include(u => u.HomeOwnerships)
                        .ThenInclude(ho => ho.Home)
                            .ThenInclude(h => h.ResidentialCommunity)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    response?.AddAlert("error", "User extracted from claims not found in database");
                    return null;
                }
                return user;
            }
            catch (Exception e)
            {
                response?.AddAlert("error", "Error fetching user from database: " + e.Message);
                return null;
            }
        }
        public byte[] GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20);
            }
        }
        internal byte[] ConvertPngToArgb(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            using (var image = new Bitmap(ms))
            using (var newImage = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb))
            using (var newMs = new MemoryStream())
            {
                newImage.Save(newMs, ImageFormat.Png);
                return newMs.ToArray();
            }
        }
        internal static string SerializeUserDTOWithNewtonSoft(UserDTO userDTO)
        {
            string userJson = JsonConvert.SerializeObject(userDTO, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return userJson;
        }
        internal static string SerializePDAPIResponseWithNewtonSoft(PDAPIResponse apiResponse)
        {
            string apiResponseJson = JsonConvert.SerializeObject(apiResponse, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return apiResponseJson;
        }
        /// <summary>
        /// This uses the Newtonsoft.Json library to serialize PDAPIResponse which is the only way to currently preserve
        /// <see cref="PDAPIResponse.User.Roles"/>
        /// </summary>
        /// <param name="apiResponse">The PDAPIResponse that will be returned to client.</param>
        /// <param name="statusCode">Status code of content, such as 200, 300, 400, 500, etc.</param>
        /// <returns></returns>
        internal static ContentResult ReturnPDAPIResponseContentResult(PDAPIResponse apiResponse, int statusCode = 200)
        {
            string apiResponseJson = SerializePDAPIResponseWithNewtonSoft(apiResponse);
            return new ContentResult
            {
                Content = apiResponseJson,
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }
    }
}
