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
        public HomeDTO? Home { get; set; }
        public UserDTO? Owner { get; set; }
        public static HomeOwnershipDTO ReturnSimpleHomeOwnershipDTOFromHomeOwnership(HomeOwnership homeOwnership)
        {
            HomeOwnershipDTO newHomeOwnershipDTO = new()
            {
                Id = homeOwnership.Id,
                OwnershipPercentage = homeOwnership.OwnershipPercentage,
                Owner = homeOwnership.Owner != null ? new UserDTO()
                {
                    Id = homeOwnership.Owner.Id,
                    FirstName = homeOwnership.Owner.FirstName,
                    MiddleName = homeOwnership.Owner.MiddleName,
                    LastName = homeOwnership.Owner.LastName,
                    SecondLastName = homeOwnership.Owner.SecondLastName,
                    ProfilePicture = homeOwnership.Owner.ProfilePicture,
                } : null,
            };
            HomeDTO homeDto = new()
            {
                Id = homeOwnership.Home.Id,
                InternalAddress = homeOwnership.Home.InternalAddress,
                FullAddress = homeOwnership.Home.FullAddress,
                Number = homeOwnership.Home.Number,
            };
            ResidentialCommunityDTO homeDTOCommunity = new()
            {
                Id = homeOwnership.Home?.ResidentialCommunity?.Id ?? 0,
                Name = homeOwnership.Home?.ResidentialCommunity?.Name ?? string.Empty,
                Address = homeOwnership.Home?.ResidentialCommunity?.Address ?? string.Empty
            };

            homeDto.Community = homeDTOCommunity;

            newHomeOwnershipDTO.Home = homeDto;
            return newHomeOwnershipDTO;
        }
        public override bool Equals(object? obj)
        {
            if (obj is HomeOwnershipDTO hoDTO) return Id == hoDTO.Id;
            else return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
