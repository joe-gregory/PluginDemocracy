using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class JoinCommunityRequestUploadDTO
    {
        public int CommunityId { get; set; }
        public int HomeId { get; set; }
        public bool JoiningAsOwner { get; set; } = false;
        public bool JoiningAsResident { get; set; } = false;
        public double OwnershipPercentage { get; set; } = 0;
        public List<IFormFile> SupportingDocumentsToAdd { get; set; } = [];
        public JoinCommunityRequestUploadDTO() { }
    }
}
