using PluginDemocracy.Models;
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
        public static HomeOwnershipDto ReturnHomeOwnershipDtoFromHomeOwnership(HomeOwnership homeOwnership)
        {
            HomeOwnershipDto newHomeOwnershipDto = new()
            {
                Id = homeOwnership.Id,
                OwnershipPercentage = homeOwnership.OwnershipPercentage,
                Home = HomeDto.ReturnHomeDtoFromHome(homeOwnership.Home),
                _userOwner = homeOwnership._userOwner != null ? UserDto.ReturnUserDtoFromUser(homeOwnership._userOwner) : null,
                _communityOwner = homeOwnership._communityOwner != null ? CommunityDto.ReturnSimpleCommunityDtoFromCommunity(homeOwnership._communityOwner) : null,
            };
            return newHomeOwnershipDto;
        }
    }
}
