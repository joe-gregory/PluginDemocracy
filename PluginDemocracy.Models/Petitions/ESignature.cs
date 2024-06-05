using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class ESignature
    {
        public int Id { get; set; }
        public User Signer { get; protected set; } 
        public string IPAddress { get; protected set; }
        public Petition Petition { get; protected set; } 
        /// <summary>
        /// Base64 encoded image of the signature
        /// </summary>
        public string SignatureImage { get; protected set; } 
        /// <summary>
        /// This is used to ensure that the signature has not been tampered with. You can use <see cref=""/>
        /// </summary>
        public string DocumentHash { get; protected set; } 
        /// <summary>
        /// To ensure that the petition signed has not been tampered with. 
        /// This is the hash of the petition at time of signing.
        /// </summary>
        public string HashOfSignedDocument { get; protected set; }
        public DateTime SignedDate { get; protected set; } 
        public string Intent { get; protected set; } 
        /// <summary>
        /// This constructor is for use by EFC
        /// </summary>
        protected ESignature()
        {
            Signer = new();
            IPAddress = string.Empty;
            Petition = new(Signer);
            SignatureImage = string.Empty;
            DocumentHash = string.Empty;
            HashOfSignedDocument = string.Empty;
            SignedDate = DateTime.Now;
            Intent = string.Empty;

        }
        public ESignature(User signer, string ipAddress, Petition petition, string hashOfSignedDocument, string signatureImage, string intent)
        {
            Signer = signer;
            IPAddress = ipAddress;
            Petition = petition;
            HashOfSignedDocument = hashOfSignedDocument;
            SignatureImage = signatureImage;
            Intent = intent;
            DocumentHash = ReturnHash();
        }
        public string ReturnHash()
        {
            var hashInput = $"{Signer.FullName}{Signer.Email}{IPAddress}{DocumentHash}{SignedDate}{Intent}";
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
