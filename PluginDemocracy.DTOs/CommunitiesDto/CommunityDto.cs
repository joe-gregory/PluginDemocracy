﻿
using PluginDemocracy.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs
{
    public class CommunityDto : BaseCitizenDto
    {
        #region PROPERTIES
        public string? Name { get; set; }
        [JsonIgnore]
        public override string? FullName => string.Join(" ", Name, Address);
        public string OfficialCurrency { get; set; } = "USD";
        public List<string> _officialLanguagesCodes { get; set; } = [];
        [JsonIgnore]
        public List<CultureInfo> OfficialLanguages
        {
            get
            {
                List<CultureInfo> cultures = [];
                foreach (string code in _officialLanguagesCodes) cultures.Add(new CultureInfo(code));
                return cultures;
            }
        }
        public string? Description { get; set; } = string.Empty;
        [JsonIgnore]
        public override List<CommunityDto> Citizenships
        {
            get
            {
                List<CommunityDto> citizenships = [.. NonResidentialCitizenIn];
                return citizenships.Distinct().ToList();
            }
        }
        public bool CanHaveHomes { get; set; }
        public List<HomeDto> Homes { get; set; } = [];
        /// <summary>
        /// Can Citizens be added if they don't belong to a home
        /// </summary>
        public bool CanHaveNonResidentialCitizens { get; set; }
        public readonly List<CommunityDto> _communityNonResidentialCitizens = [];
        public readonly List<UserDto> _userNonResidentialCitizens = [];
        /// <summary>
        /// Citizens that don't live in a home
        /// </summary>
        [JsonIgnore]
        public virtual List<BaseCitizenDto> NonResidentialCitizens => _communityNonResidentialCitizens.Cast<BaseCitizenDto>().Concat(_userNonResidentialCitizens).ToList();

        [JsonIgnore]
        virtual public List<BaseCitizenDto> Citizens
        {
            get
            {
                List<BaseCitizenDto> homeOwners = Homes?.SelectMany(home => home.Owners.Keys).ToList() ?? [];
                List<UserDto> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? [];
                return NonResidentialCitizens.Union(homeOwners).Union(homeResidents).Distinct().ToList();
            }
        }
        /// <summary>
        /// Policy for how long a proposal remains open for after it publishes. It's an int representing days.
        /// </summary>
        public int ProposalsExpirationDays { get; set; }
        public BaseVotingStrategy? VotingStrategy { get; set; }
        /// <summary>
        /// CitizensVotingValue is an int to protect against rounding errors.
        /// Each inheriting class can override who gets to vote and how much each vote counts. 
        /// In BaseCommunity, each Citizens gets one vote. 
        /// </summary>
        [JsonIgnore]
        public Dictionary<BaseCitizenDto, double> CitizensVotingValue = [];
        public double TotalVotes;
        public Constitution? Constitution { get; set; }
        public List<Proposal>? Proposals { get; set; }
        [JsonIgnore]
        public List<Proposal>? AcceptedProposals => Proposals?.Where(proposal => proposal.Passed == true).ToList();
        [JsonIgnore]
        public List<Proposal>? RejectedProposals => Proposals?.Where(proposal => proposal.Passed == false).ToList();
        [JsonIgnore]
        public List<Proposal>? UndecidedProposals => Proposals?.Where(proposal => proposal.Open == true).ToList();
        public Accounting? Accounting { get; }
        public List<BaseDictamen>? Dictamens { get; set; }
        public List<Role>? Roles { get; set; }
        public List<Project>? Projects { get; }
        [NotMapped]
        public List<Project>? ActiveProjects => Projects?.Where(project => project.Active).ToList();
        public List<RedFlag>? RedFlags { get; }
        public List<Post>? Posts { get; }
        #endregion
        #region METHODS
        public CommunityDto()
        {
            Homes = [];
            Dictamens = [];
            Roles = [];
            Projects = [];
            RedFlags = [];
            Posts = [];
        }
        public CommunityDto(Community community)
        {
            //BaseCitizenDto Properties
            Id = community.Id;
            Address = community.Address;
            ProfilePicture = community.ProfilePicture;
            NonResidentialCitizenIn = community.NonResidentialCitizenIn.Select(c => ReturnSimpleCommunityDtoFromCommunity(c)).ToList();
            foreach (HomeOwnership homeOwnership in community.HomeOwnerships) HomeOwnerships.Add(HomeOwnershipDto.ReturnHomeOwnershipDtoFromHomeOwnership(homeOwnership));
            //CommunityDto Properties
            Name = community.Name;
            OfficialCurrency = community.OfficialCurrency;
            foreach (string language in community._officialLanguagesCodes) _officialLanguagesCodes.Add(language);
            Description = community.Description;
            CanHaveHomes = community.CanHaveHomes;
            foreach(Home home in community.Homes) Homes?.Add(HomeDto.ReturnHomeDtoFromHome(home));
            CanHaveNonResidentialCitizens = community.CanHaveNonResidentialCitizens;
            foreach (Community communityNonResidentialCitizen in community._communityNonResidentialCitizens) _communityNonResidentialCitizens.Add(ReturnSimpleCommunityDtoFromCommunity(communityNonResidentialCitizen));
            foreach (User userNonResidentialCitizen in community._userNonResidentialCitizens) _userNonResidentialCitizens.Add(UserDto.ReturnUserDtoFromUser(userNonResidentialCitizen));
            ProposalsExpirationDays = community.ProposalsExpirationDays;
            VotingStrategy = community.VotingStrategy;
            TotalVotes = community.TotalVotes;
            Constitution = community.Constitution;
            Proposals = community.Proposals;
            Accounting = community.Accounting;
            Dictamens = community.Dictamens;
            Roles = community.Roles;
            Projects = community.Projects;
            RedFlags = community.RedFlags;
            Posts = community.Posts;
        }
        public void AddOfficialLanguage(CultureInfo culture)
        {
            _officialLanguagesCodes.Add(culture.Name);
        }
        public void AddHome(HomeDto home)
        {
            if (!Homes.Contains(home)) 
            {
                home.ParentCommunity = this;
                Homes.Add(home);
            }
        }
        public void RemoveHome(HomeDto home)
        {
            Homes.Remove(home);
        }   
        /// <summary>
        /// This will return a CommunityDto basic properties. It is most likely used with Home.Ownerships reference outside communities for 
        /// which not a lot of extensive informaiton is needed. 
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        public static CommunityDto ReturnSimpleCommunityDtoFromCommunity(Community community)
        {
            return new CommunityDto()
            {
                Name = community.Name,
                OfficialCurrency = community.OfficialCurrency,
                Address = community.Address,
                _officialLanguagesCodes = community.OfficialLanguages.Select(culture => culture.Name).ToList(),
                Description = community.Description,
            };
        }   
        #endregion
    }
}
