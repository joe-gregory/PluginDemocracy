using PluginDemocracy.DTOs;
using PluginDemocracy.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs
{
    public class HomeDTO
    {
        #region PROPERTIES
        public int Id { get; set; }
        public int Number { get; set; }
        public string InternalAddress { get; set; }
        public ResidentialCommunityDTO? Community { get; set; }
        public string? FullAddress { get; set; }
        public List<HomeOwnershipDTO> Ownerships { get; set; } = [];
        [JsonIgnore]
        public Dictionary<UserDTO, double> OwnersOwnerships
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }
        [JsonIgnore]
        public List<UserDTO> Owners
        {
            get
            {
                return [.. OwnersOwnerships.Keys];
            }
        }
        [JsonIgnore]
        public double CurrentlyOwnedPercentage
        {
            get
            {
                return OwnersOwnerships.Values.Sum();
            }
        }
        /// <summary>
        /// The percentage of ownership that is available to be owned by new owners. This is calculated by subtracting the sum of the ownership percentages of the current owners from 100.
        /// </summary>
        [JsonIgnore]
        public double AvailableOwnershipPercentage 
        { 
            get
            {
                return 100 - OwnersOwnerships.Values.Sum();
            } 
        }
        public List<UserDTO> Residents { get; set; } = [];
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        [JsonIgnore]
        public List<UserDTO> Citizens
        {
            get 
            { 
                return Owners.Union(Residents).ToList(); 
            }
        }
        #endregion
        #region METHODS
        public HomeDTO() 
        {
            InternalAddress = string.Empty;
        }
        public HomeDTO(Home home)
        {
            Id = home.Id;
            Number = home.Number;
            InternalAddress = home.InternalAddress;
            FullAddress = home.FullAddress;
            foreach (HomeOwnership homeOwnership in home.Ownerships)
            {
                HomeOwnershipDTO homeOwnershipDTO = new()
                {
                    Id = homeOwnership.Id,
                    OwnershipPercentage = homeOwnership.OwnershipPercentage,
                    Home = this,
                    Owner = homeOwnership.Owner != null ? UserDTO.ReturnAvatarMinimumUserDTOFromUser(homeOwnership.Owner) : null,
                };
                Ownerships.Add(homeOwnershipDTO);
            }
            //HomeDto Properties
            Community = home.ResidentialCommunity != null ? ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(home.ResidentialCommunity) : null;
            Residents = home.Residents.Select(r => UserDTO.ReturnSimpleUserDTOFromUser(r)).ToList();
        }
        /// <summary>
        /// Overriding DTO Equals method to compare Ids because more than one DTO object may be created for the same entity.
        /// </summary>
        /// <param name="obj">The object being compared to</param>
        /// <returns>true if both are HomeDto type and Id matches</returns>
        public override bool Equals(object? obj)
        {

            if (GetType() == obj?.GetType()) 
            { 
                return Id == ((HomeDTO)obj).Id && Number == ((HomeDTO)obj).Number && InternalAddress == ((HomeDTO)obj).InternalAddress;
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
        public JoinCommunityRequestDTO JoinHome(UserDTO user, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            if (joiningAsOwner)
            {
                if (ownershipPercentage > AvailableOwnershipPercentage || ownershipPercentage <= 0 || ownershipPercentage > 100) throw new Exception("ErrorMessageJoinHomeWrongPercentage");
            }
            JoinCommunityRequestDTO request = new()
            {
                CommunityDTO = Community,
                HomeDTO = this,
                UserDTO = user,
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
        [Obsolete("There is a new constructor that takes Home type as argument. No need for this static method anymore.")]
        public static HomeDTO ReturnHomeDTOFromHome(Home home)
        {
            HomeDTO newHomeDto = new()
            {
                Id = home.Id,
                Number = home.Number,
                InternalAddress = home.InternalAddress,
                FullAddress = home.FullAddress,
                Ownerships = home.Ownerships.Select(ho => HomeOwnershipDTO.ReturnSimpleHomeOwnershipDTOFromHomeOwnership(ho)).ToList(),
                //HomeDto Properties
                Community = home.ResidentialCommunity != null ? ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(home.ResidentialCommunity) : null,
                Residents = home.Residents.Select(r => UserDTO.ReturnSimpleUserDTOFromUser(r)).ToList(),
            };
            return newHomeDto;
        }
        #endregion
    }
}
