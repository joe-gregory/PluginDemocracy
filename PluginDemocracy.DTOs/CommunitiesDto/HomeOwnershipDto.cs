using PluginDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class HomeOwnershipDTO
    {
        public int Id { get; set; }
        public double OwnershipPercentage { get; set; }
        public virtual HomeDTO? Home { get; set; }
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
        public static HomeOwnershipDTO ReturnHomeOwnershipDtoFromHomeOwnership(HomeOwnership homeOwnership)
        {
            HomeOwnershipDTO newHomeOwnershipDto = new()
            {
                Id = homeOwnership.Id,
                OwnershipPercentage = homeOwnership.OwnershipPercentage,
                _userOwner = homeOwnership._userOwner != null ? new UserDTO() 
                { 
                    Id = homeOwnership._userOwner.Id, 
                    FirstName = homeOwnership._userOwner.FirstName,
                    MiddleName = homeOwnership._userOwner.MiddleName,
                    LastName = homeOwnership._userOwner.LastName,
                    SecondLastName = homeOwnership._userOwner.SecondLastName,
                    ProfilePicture = homeOwnership._userOwner.ProfilePicture,
                } 
                : null,
                _communityOwner = homeOwnership._communityOwner != null ? CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(homeOwnership._communityOwner) : null,
            };
            HomeDTO homeDto = new()
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
