using System.Security.Cryptography;
using System.Text;

namespace PluginDemocracy.Models
{
    public class ESignature
    {
        public int Id { get; init; }
        public User Signer { get; init; } 
        public string IPAddress { get; init; }
        public Petition Petition { get; init; } 
        /// <summary>
        /// Base64 encoded image of the signature
        /// </summary>
        public string SignatureImageBase64 { get; init; }
        public string SignatureImage { get; init; }
        /// <summary>
        /// To ensure that the petition signed has not been tampered with. 
        /// This is the hash of the petition at time of signing.
        /// </summary>
        public string DocumentHash { get; init; } 
        public DateTime SignedDate { get; init; } 
        public string Intent { get; init; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private ESignature() { }
        #pragma warning restore CS8618
        public ESignature(User signer, string ipAddress, Petition petition, string signatureImage, string signatureImageBase64, string intent)
        {
            Signer = signer;
            IPAddress = ipAddress;
            Petition = petition;
            SignatureImage = signatureImage;
            SignatureImageBase64 = signatureImageBase64;
            Intent = intent;
            DocumentHash = ReturnHash();
            SignedDate = DateTime.UtcNow;
        }
        public string ReturnHash()
        {
            var hashInput = $"{Signer.FullName}{Signer.Email}{IPAddress}{SignedDate}{Intent}{Petition.Id}{SignatureImage}";
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
