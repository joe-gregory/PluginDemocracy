using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class ESignatureDTO
    {
        public int Id { get; set; }
        public UserDTO? Signer { get; set; }
        public string? IPAddress { get; set; }
        public PetitionDTO? PetitionDTO { get; set; }
        public string? SignatureImageBase64 { get; init; }
        public string? SignatureImage { get; set; }
        public string? DocumentHash { get; set; }
        public string? HashOfSignedDocument { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? Intent { get; set; }
        public ESignatureDTO()
        {

        }
        public ESignatureDTO(ESignature eSignature)
        {
            Id = eSignature.Id;
            if (eSignature.Signer != null) Signer = UserDTO.ReturnAvatarMinimumUserDTOFromUser(eSignature.Signer);
            IPAddress = eSignature.IPAddress;
            SignatureImage = eSignature.SignatureImage;
            SignatureImageBase64 = eSignature.SignatureImageBase64;
            DocumentHash = eSignature.DocumentHash;
            SignedDate = eSignature.SignedDate;
            Intent = eSignature.Intent;
        }
    }
}
