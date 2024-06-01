using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class ESignatureDTO
    {
        public int Id { get; set; }
        public UserDTO? Signer { get; set; }
        public string? IPAddress { get; set; }
        public PetitionDTO? PetitionDTO { get; set; }
        public string? SignatureImage { get; set; }
        public string? DocumentHash { get; set; }
        public string? HashOfSignedDocument { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? Intent { get; set; }
    }
}
