using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs.CommunitiesDto
{
    public class JoinHomeRequestDto
    {
        public int CommunityId { get; set; }
        public int HomeId { get; set; }
        public int UserId { get; set; }
        public bool joiningAsOwner { get; private set }
        [JsonIgnore]
        public bool JoiningAsOwner { 
            get 
            {
                return joiningAsOwner;
            } 
            set 
            { 
            } 
        }
        public bool joiningAsResident;
        [JsonIgnore]
        public bool JoiningAsResident { get; set { } }
    }
}
