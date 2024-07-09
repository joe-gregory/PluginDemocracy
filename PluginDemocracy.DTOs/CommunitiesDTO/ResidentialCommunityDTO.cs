
using PluginDemocracy.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs
{
    public class ResidentialCommunityDTO : IAvatar
    {
        #region PROPERTIES
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public string FullName { get; set; }
        public string Initials { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? OfficialCurrency { get; set; } = "USD";
        public List<string> OfficialLanguagesCodes { get; set; } = [];
        [JsonIgnore]
        public List<CultureInfo> OfficialLanguages
        {
            get
            {
                List<CultureInfo> languages = [];
                foreach (string language in OfficialLanguagesCodes) languages.Add(new CultureInfo(language));
                return languages;
            }
        }
        public List<HomeDTO> Homes { get; set; }
        [JsonIgnore]
        public IReadOnlyList<HomeOwnershipDTO> HomeOwnerships
        {
            get
            {
                return Homes.SelectMany(home => home.Ownerships).ToList().AsReadOnly();
            }
        }
        [JsonIgnore]
        virtual public List<UserDTO> Citizens
        {
            get
            {
                List<UserDTO> homeOwners = Homes?.SelectMany(home => home.OwnersOwnerships.Keys).ToList() ?? [];
                List<UserDTO> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? [];
                return homeOwners.Union(homeResidents).Distinct().ToList();
            }
        }
        public List<PostDTO>? Posts { get; set; } = [];
        #endregion
        #region METHODS
        public ResidentialCommunityDTO()
        {
            FullName = string.Empty;
            Initials = string.Empty;
            Homes = [];
            Posts = [];
        }
        public ResidentialCommunityDTO(ResidentialCommunity community)
        {
            //BaseCitizenDto Properties
            Id = community.Id;
            FullName = community.FullName;
            Initials = community.Initials;
            Address = community.Address;
            ProfilePicture = community.ProfilePicture;
            //CommunityDto Properties
            Name = community.Name;
            OfficialCurrency = community.OfficialCurrency;
            foreach (CultureInfo language in community.OfficialLanguages) AddLanguage(language);
            Description = community.Description;
            Homes = [];
            foreach (Home home in community.Homes)
            {
                HomeDTO homeDTO = new()
                {
                    Id = home.Id,
                    Number = home.Number,
                    InternalAddress = home.InternalAddress,
                    Community = this,
                };
                Homes.Add(homeDTO);
            }
            foreach (Post post in community.Posts) Posts.Add(new PostDTO(post));
        }
        public void AddHome(HomeDTO home)
        {
            if (!Homes.Contains(home)) 
            {
                home.Community = this;
                Homes.Add(home);
            }
        }
        public void RemoveHome(HomeDTO home)
        {
            Homes.Remove(home);
        }   
        public void AddLanguage(CultureInfo culture)
        {
            OfficialLanguagesCodes.Add(culture.Name);
        }
        public void RemoveLanguage(CultureInfo culture)
        {
            OfficialLanguagesCodes.Remove(culture.Name);
        }
        /// <summary>
        /// This will return a CommunityDto basic properties. It is most likely used with Home.Ownerships reference outside communities for 
        /// which not a lot of extensive informaiton is needed. 
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        public static ResidentialCommunityDTO ReturnSimpleCommunityDTOFromCommunity(ResidentialCommunity community)
        {
            ResidentialCommunityDTO communityDTO = new()
            {
                Id = community.Id,
                Name = community.Name,
                Description = community.Description,
                OfficialCurrency = community.OfficialCurrency,
                Address = community.Address,
            };
            foreach (CultureInfo language in community.OfficialLanguages) communityDTO.AddLanguage(language);

            return communityDTO;
        }
        public override string ToString()
        {
            return FullName;
        }
        #endregion
    }
}
