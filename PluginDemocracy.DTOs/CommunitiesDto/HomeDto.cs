using PluginDemocracy.DTOs;
using PluginDemocracy.Models;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs
{
    public class HomeDto : BaseCitizenDto
    {
        #region PROPERTIES
        public CommunityDto? ParentCommunity { get; set; }
        public HashSet<HomeOwnershipDto> Ownerships { get; set; } = [];
        [JsonIgnore]
        public override string? FullName
        {
            get
            {
                string fullName = string.Empty;
                foreach(HomeOwnershipDto ho in Ownerships) fullName += ho.Owner?.FullName + ", ";
                return fullName;
            }
        }
        [JsonIgnore]
        public override List<CommunityDto> Citizenships
        {
            get
            {
                if (ParentCommunity != null) return [ParentCommunity];
                else return [];
            }
        }
        public int Number { get; set; }
        public string InternalAddress { get; set; } = string.Empty;
        [JsonIgnore]
        public override string Address => Number + " " + InternalAddress + " " + ParentCommunity?.Address;
        [JsonIgnore]
        public Dictionary<BaseCitizenDto, double> Owners
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }
        /// <summary>
        /// The percentage of ownership that is available to be owned by new owners. This is calculated by subtracting the sum of the ownership percentages of the current owners from 100.
        /// </summary>
        [JsonIgnore]
        public double AvailableOwnershipPercentage 
        { 
            get
            {
                return 100 - Owners.Values.Sum();
            } 
        }
        public List<UserDto> Residents { get; set; } = [];
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        [JsonIgnore]
        public List<BaseCitizenDto> Citizens
        {
            get => Owners.Keys.Union(Residents).ToList();
        }
        #endregion
        #region METHODS
        /// <summary>
        /// Overriding DTO Equals method to compare Ids because more than one DTO object may be created for the same entity.
        /// </summary>
        /// <param name="obj">The object being compared to</param>
        /// <returns>true if both are HomeDto type and Id matches</returns>
        public override bool Equals(object? obj)
        {

            if (GetType() == obj?.GetType()) 
            { 
                return Id == ((HomeDto)obj).Id && Number == ((HomeDto)obj).Number && InternalAddress == ((HomeDto)obj).InternalAddress;
            }
            else return false;
        }
        /// <summary>
        /// Get hash code is overridden to match the overridden Equals method because some collection types rely on hash codes such as dictionaries and hash sets.
        /// </summary>
        /// <returns>Id as a hash code</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public JoinCommunityRequestDto JoinHome(UserDto user, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            if (joiningAsOwner)
            {
                if (ownershipPercentage > AvailableOwnershipPercentage || ownershipPercentage <= 0 || ownershipPercentage > 100) throw new Exception("ErrorMessageJoinHomeWrongPercentage");
            }
            JoinCommunityRequestDto request = new()
            {
                CommunityDto = ParentCommunity,
                HomeDto = this,
                UserDto = user,
            };
            if (joiningAsOwner)
            {
                request.JoiningAsResident = false;
                request.JoiningAsOwner = true;
                request.OwnershipPercentage = ownershipPercentage;
            }
            else 
            { 
                request.JoiningAsResident = true; 
                request.JoiningAsOwner = false;
            }
            return request;
        }
        public static HomeDto ReturnHomeDtoFromHome(Home home)
        {
            HomeDto newHomeDto = new()
            {
                //BaseCitizenDto Properties
                Id = home.Id,
                Address = home.Address,
                ProfilePicture = home.ProfilePicture,
                NonResidentialCitizenIn = home.NonResidentialCitizenIn.Select(c => CommunityDto.ReturnSimpleCommunityDtoFromCommunity(c)).ToList(),
                HomeOwnerships = home.HomeOwnerships.Select(ho => HomeOwnershipDto.ReturnHomeOwnershipDtoFromHomeOwnership(ho)).ToList(),
                //HomeDto Properties
                ParentCommunity = home.ParentCommunity != null ? CommunityDto.ReturnSimpleCommunityDtoFromCommunity(home.ParentCommunity) : null,
                Ownerships = home.Ownerships.Select(ho => HomeOwnershipDto.ReturnHomeOwnershipDtoFromHomeOwnership(ho)).ToHashSet(),
                Number = home.Number,
                InternalAddress = home.InternalAddress,
                Residents = home.Residents.Select(r => UserDto.ReturnUserDtoFromUser(r)).ToList(),
            };
            return newHomeDto;
        }
        #endregion
    }
}
