using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class HomeOwnershipDto
    {
        public int Id { get; set; }
        public double OwnershipPercentage { get; set; }
        public virtual HomeDto? Home { get; set; }
        public UserDto? _userOwner;
        public CommunityDto? _communityOwner;
        [JsonIgnore]
        public virtual BaseCitizenDto? Owner
        {
            get
            {
                if(_userOwner != null) return _userOwner;
                if(_communityOwner != null) return _communityOwner;
                return null;
            }
        }
    }
}
