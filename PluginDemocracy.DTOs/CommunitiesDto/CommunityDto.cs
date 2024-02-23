
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
        public List<string> OfficialLanguagesCodes { get; set; } = [];
        [JsonIgnore]
        public List<CultureInfo> OfficialLanguages
        {
            get
            {
                List<CultureInfo> cultures = [];
                foreach (string code in OfficialLanguagesCodes) cultures.Add(new CultureInfo(code));
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
        public List<HomeDto> Homes { get; set; }
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
            Name = community.Name;
            OfficialCurrency = community.OfficialCurrency;
            OfficialLanguagesCodes = community.OfficialLanguages.Select(culture => culture.Name).ToList();
            Description = community.Description;
            CanHaveHomes = community.CanHaveHomes;

            //HOMES
            //TODO ME FALTAN LAS BASECITIZEN STUFF LIKE ID AND PROFILE PICTURE
            Homes = [];
            foreach(Home home in community.Homes)
            {
                HomeDto newHomeDto = new()
                {
                    Id = home.Id,
                    ParentCommunity = this,
                    InternalAddress = home.InternalAddress,
                };
                //Ownerships
                foreach(HomeOwnership homeOwnership in home.Ownerships)
                {
                    HomeOwnershipDto newHomeOwnershipDto = new()
                    {
                        Id = homeOwnership.Id,
                        OwnershipPercentage = homeOwnership.OwnershipPercentage,
                        Home = newHomeDto,
                    };
                    if(homeOwnership.Owner is User userOwner) newHomeOwnershipDto._userOwner = UserDto.ReturnUserDtoFromUser(userOwner);
                    if(homeOwnership.Owner is Community communityOwner) 
                    {
                        if(communityOwner.Id == this.Id) newHomeOwnershipDto._communityOwner = this;
                        else newHomeOwnershipDto._communityOwner = CommunityDto.ReturnSimpleCommunityDtoFromCommunity(communityOwner);
                    }
                    newHomeDto.Ownerships.Add(newHomeOwnershipDto);
                }
                //Residents
                foreach (User resident in home.Residents) newHomeDto.Residents.Add(UserDto.ReturnUserDtoFromUser(resident));
                Homes.Add(newHomeDto);
            }
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
                OfficialLanguagesCodes = community.OfficialLanguages.Select(culture => culture.Name).ToList(),
                Description = community.Description,
            };
        }   
        #endregion
    }

}
