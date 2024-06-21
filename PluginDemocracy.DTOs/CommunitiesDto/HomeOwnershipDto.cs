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
        public UserDTO? _userOwner;
        public CommunityDTO? _communityOwner;
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
                _userOwner = homeOwnership._userOwner != null ? UserDTO.ReturnSimpleUserDTOFromUser(homeOwnership._userOwner) : null,
                _communityOwner = homeOwnership._communityOwner != null ? CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(homeOwnership._communityOwner) : null,
            };
            HomeDto homeDto = new()
            {
                Id = homeOwnership.Home.Id,
                Address = homeOwnership.Home.Address,
                ProfilePicture = homeOwnership.Home.ProfilePicture,
                ParentCommunity = homeOwnership.Home.ParentCommunity != null ? CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(homeOwnership.Home.ParentCommunity) : null,
                Number = homeOwnership.Home.Number,
                InternalAddress = homeOwnership.Home.InternalAddress,
            };
            newHomeOwnershipDto.Home = homeDto;
            return newHomeOwnershipDto;
        }
    }
}
