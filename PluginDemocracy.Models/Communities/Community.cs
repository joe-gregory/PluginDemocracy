using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Community : BaseCitizen
    {
        #region PROPERTIES
        //Basic information
        public string? Name { get; set; }
        [NotMapped]
        public override string? FullName => string.Join(" ", Name, Address);
        [NotMapped]
        public override string? Initials 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return null;

                // Split the name by spaces and take the first letter of each word
                var initials = Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word[0])
                    .ToArray();

                return new string(initials);
            }
        }
        public string OfficialCurrency { get; set; } = "USD";
        /// <summary>
        /// Ideally this property should be private. However, EFCore requires it to be public... for now. 
        /// </summary>
        public List<string> _officialLanguagesCodes { get; private set; }
        [NotMapped]
        public List<CultureInfo> OfficialLanguages 
        { 
            get 
            { 
                List<CultureInfo> cultures = [];
                foreach (string code in _officialLanguagesCodes) cultures.Add(new CultureInfo(code));
                return cultures;
            }
        }
        public string? Description { get; set; }
        /// <summary>
        /// The Communities that this Community belongs to. Like to which Home owners association or Privada does this belong to. 
        /// </summary>
        [NotMapped]
        public override List<Community> Citizenships
        {
            get
            {
                List<Community> citizenships = [.. NonResidentialCitizenIn];
                return citizenships.Distinct().ToList();
            }
        }
        public bool CanHaveHomes { get; set; }
        public List<Home> Homes { get; private set; }
        /// <summary>
        /// Can Citizens be added if they don't belong to a home
        /// </summary>
        public bool CanHaveNonResidentialCitizens { get; set; }
        public readonly List<Community> _communityNonResidentialCitizens;
        public readonly List<User> _userNonResidentialCitizens;
        /// <summary>
        /// Citizens that don't live in a home
        /// </summary>
        [NotMapped]
        public virtual List<BaseCitizen> NonResidentialCitizens { get => _communityNonResidentialCitizens.Cast<BaseCitizen>().Concat(_userNonResidentialCitizens).ToList(); }
        /// <summary>
        /// Represents all the individuals associated with a community regardless of voting ability
        /// </summary>
        [NotMapped]
        virtual public List<BaseCitizen> Citizens
        {
            get
            {
                List<BaseCitizen> homeOwners = Homes?.SelectMany(home => home.OwnersWithOwnership.Keys).ToList() ?? [];
                List<User> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? [];
                return NonResidentialCitizens.Union(homeOwners).Union(homeResidents).Distinct().ToList();
            }
        }
        public List<JoinCommunityRequest> JoinCommunityRequests { get; set; } = [];
        /// <summary>
        /// Policy for how long a proposal remains open for after it publishes. It's an int representing days.
        /// </summary>
        public int ProposalsExpirationDays { get; set; } = 30;
        public BaseVotingStrategy? VotingStrategy { get; set; }
        /// <summary>
        /// CitizensVotingValue is an int to protect against rounding errors.
        /// Each inheriting class can override who gets to vote and how much each vote counts. 
        /// In BaseCommunity, each Citizens gets one vote. 
        /// </summary>
        [NotMapped]
        public Dictionary<BaseCitizen, double> CitizensVotingValue
        {
            get
            {
                if (VotingStrategy == null) return [];
                else return VotingStrategy.ReturnVotingWeights(this);
            }
        }
        [NotMapped]
        public double TotalVotes => CitizensVotingValue.Values.Sum();
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        [NotMapped]
        public List<Proposal> AcceptedProposals => Proposals.Where(proposal => proposal.Passed == true).ToList();
        [NotMapped]
        public List<Proposal> RejectedProposals => Proposals.Where(proposal => proposal.Passed == false).ToList();
        [NotMapped]
        public List<Proposal> UndecidedProposals => Proposals.Where(proposal => proposal.Open == true).ToList();
        public List<Petition> Petitions { get; protected set; }
        public Accounting Accounting { get; }
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Project> Projects { get; }
        [NotMapped]
        public List<Project> ActiveProjects => Projects.Where(project => project.Active).ToList();
        public List<RedFlag> RedFlags { get; }
        public readonly List<Post> Posts;
        [NotMapped]
        public IReadOnlyList<Post> PostsByLatestActivity => Posts.OrderByDescending(post => post.LatestActivity ?? post.PublishedDate).ToList().AsReadOnly();
        [NotMapped]
        public IReadOnlyList<Post> PostsByPublishedDate => Posts.OrderByDescending(post => post.PublishedDate).ToList().AsReadOnly();
        #endregion
        #region METHODS
        public Community()
        {
            _officialLanguagesCodes = [];
            Homes = [];
            _communityNonResidentialCitizens = [];
            _userNonResidentialCitizens = [];
            Constitution = new();
            Proposals = [];
            Petitions = [];
            Accounting = new(this);
            Dictamens = [];
            Roles = [];
            Projects = [];
            RedFlags = [];
            Posts = [];
        }
        /// <summary>
        /// Adding a non residential citizen involves adding the citizen to this community's NonResidentialCitizens AND adding this community to the
        /// list of citizen.NonResidentialCitizenIn.
        /// </summary>
        public void AddOfficialLanguage(CultureInfo culture)
        {
            _officialLanguagesCodes.Add(culture.Name);
        }
        public void RemoveOfficialLanguage(CultureInfo culture)
        {
            _officialLanguagesCodes.Remove(culture.Name);
        }   
        public virtual void AddNonResidentialCitizen(BaseCitizen citizen)
        {
            if (!CanHaveNonResidentialCitizens) throw new InvalidOperationException("Unable to add NonResidentialCitizens when CanHaveNonResidentialCitizens is set to false.");
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            if (citizen is Community community) 
            { 
                _communityNonResidentialCitizens.Add(community);
                community.NonResidentialCitizenIn.Add(this);
            }

            if (citizen is User user) 
            {
                _userNonResidentialCitizens.Add(user);
                user.NonResidentialCitizenIn.Add(this);
            }
        }
        /// <summary>
        /// Remove from both Community list and BaseCitizen list
        /// </summary>
        /// <param name="citizen"></param>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveNonResidentialCitizen(BaseCitizen citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");
            
            if (citizen is Community community)
            {
                _communityNonResidentialCitizens.Remove(community);
                community.NonResidentialCitizenIn.Remove(this);
            }
            if (citizen is User user)
            {
                _userNonResidentialCitizens.Remove(user);
                user.NonResidentialCitizenIn.Remove(this);
            }
        }
        public void AddHome(Home home)
        {
            if (!CanHaveHomes) throw new InvalidOperationException("Cannot add Homes when CanHaveHomes is set to false");
            if (home == null) throw new ArgumentNullException("Home cannot be null");

            if (!Homes.Contains(home))
            {
                Homes.Add(home);
                home.ParentCommunity = this;
            }
        }
        public void RemoveHome(Home home)
        {
            if (Homes.Contains(home))
            {
                Homes.Remove(home);
                home.ParentCommunity = null;
            }
        }
        public void AddJoinCommunityRequest(JoinCommunityRequest request)
        {
            //Validate:
            //Does the home exist in the current community?
            if(!Homes.Any(h => h.Id == request.Home?.Id)) throw new ArgumentException("Home not found in this community");
            //If user is joining as owner, ensure that the ownership percentage is valid
            if(request.JoiningAsOwner)
            {
                if (request.OwnershipPercentage <= 0) throw new ArgumentException("Ownership cannot be less than 0.");
                if(request.OwnershipPercentage > 100) throw new ArgumentException("Ownership cannot be more than 100.");
                if(request.OwnershipPercentage > Homes.First(h => h.Id == request.Home?.Id).AvailableOwnershipPercentage) throw new ArgumentException("Ownership percentage exceeds available ownership percentage.");
            }
            //At this point, if I didn't throw any exceptions, the request must be good
            JoinCommunityRequests.Add(request);
        }
        /// <summary>
        /// Call to approve a JoinCommunityRequest.This joins the community to the home as owner or resident accordingly. 
        /// Pass the request that you want to approve. It should be located in the Community.JoinCommunityRequests list.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ApproveJoinCommunityRequest(JoinCommunityRequest request)
        {
            if(request.User == null) throw new ArgumentException("Request.User is null");
            if (!JoinCommunityRequests.Contains(request)) throw new ArgumentException("Request not found in Community.JoinCommunityRequests");
            //Make sure the home belongs to this community
            Home? home = Homes.FirstOrDefault(h => h.Id == request.Home.Id) ?? throw new ArgumentException("Home does not belong to this community");
            if (request.JoiningAsOwner)
            {
                home.AddOwner(request.User, request.OwnershipPercentage);
            }
            else if(request.JoiningAsResident)
            {
                home.AddResident(request.User);
            }
            else
            {
                throw new ArgumentException("JoiningAsOwner and JoiningAsResident are both false. At least one must be true.");
            }
            request.Approved = true;
        }
        public void PublishProposal(Proposal proposal, bool skipAssigningExpirationDate = false)
        {
            if (VotingStrategy == null) throw new InvalidOperationException("VotingStrategy is null");
            //Ensure this proposal is for this community
            proposal.Community = this;
            //Ensure it has a title
            if (proposal.Title == null) throw new ArgumentException("Proposal.Title is null");
            //Ensure it has a description
            if (proposal.Description == null) throw new ArgumentException("Proposal.Description is null");
            //I could ensure that the Author is either a Resident or a Dictamen of this Community, but perhaps in the future. 
            if (proposal.Author == null) throw new ArgumentException("Proposal.Author is null");

            proposal.PublishedDate = DateTime.Now;
            //Dictamen cannot be empty
            if (proposal.Dictamen == null) throw new ArgumentException("Proposal.Dictamen is null");
            //The Proposal's Dictamen should be pointing to this Community
            proposal.Dictamen.Community = this;
            //Votes should be empty
            if (proposal.Votes.Count != 0) throw new ArgumentException("Proposal.Votes is not empty");
            //If everything is Ok, add to add of list of Proposals and return True so that the proposal can set its PublishedDate
            proposal.Open = true;
            proposal.VotingStrategy ??= VotingStrategy;
            if(!skipAssigningExpirationDate) proposal.ExpirationDate = DateTime.Now.AddDays(ProposalsExpirationDays);
            Proposals.Add(proposal);
            if (VotingStrategy.ShouldProposalPropagate(proposal)) PropagateProposal(proposal);

        }
        public bool IssueDictamen(BaseDictamen dictamen)
        {
            //TODO: Make sure everything is good to go and no info is missing
            //if title is empty, throw exception
            if (dictamen.Community != this) throw new ArgumentException("Dictamen.Community does not point to this Community");
            //The Dictamen must either come from a Role or a Proposal. In the future ensure that the Role has the corresponding rights
            if (dictamen.IssueDate != null) throw new ArgumentException("Dictamen.IssueDate is not null");

            dictamen.Execute();
            Dictamens.Add(dictamen);
            return true;
        }
        public void RaiseRedFlag(User user, string description, BaseRedFlaggable itemFlagged)
        {
            RedFlag newRedFlag = new(this, user, description, itemFlagged);
            RedFlags.Add(newRedFlag);
        }
        public void AddPost(Post post)
        {
            Posts.Add(post);
        }
        public void RemovePost(Post post)
        {
            Posts.Remove(post);
        }
        public void Update()
        {
            Constitution.Update();
            foreach (var proposal in Proposals) proposal.Update();
            foreach (var role in Roles) role.Update();
        }

        #region UTILITY METHODS
        private protected virtual void PropagateProposal(Proposal parentProposal)
        {

            foreach (var citizen in Citizens)
            {
                if (citizen is Community propagatedCommunity)
                {
                    Proposal propagatedProposal = ReturnPropagatedProposal(parentProposal, propagatedCommunity);
                    //publish in its corresponding community which should call this method if there are more nested sub-communities
                    propagatedCommunity.PublishProposal(propagatedProposal, skipAssigningExpirationDate : true);
                }
            }
        }
        static private protected Proposal ReturnPropagatedProposal(Proposal parentProposal, Community propagatedCommunity)
        {
            Proposal propagatedProposal = new(propagatedCommunity, parentProposal.Author)
            {
                Title = parentProposal.Title + $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>",
                Description = parentProposal.Description + $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>",
                ExpirationDate = parentProposal.ExpirationDate,
            };

            propagatedProposal.Dictamen = new PropagatedVoteDictamen(parentProposal)
            {
                TitleKey = $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>" + (parentProposal.Dictamen?.TitleKey ?? "Error: missing."),
                DescriptionKey = $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>" + (parentProposal.Dictamen?.DescriptionKey ?? "Error: missing."),
                Proposal = propagatedProposal,
                Community = propagatedCommunity,
            };

            return propagatedProposal;
        }
        public List<User> ReturnAllNestedUsers()
        {
            HashSet<User> allUsers = new();
            GetAllNestedUsers(this, allUsers);
            return [.. allUsers];
        }

        static private void GetAllNestedUsers(Community community, HashSet<User> allUsers)
        {
            foreach (var citizen in community.Citizens)
            {
                if (citizen is User user) allUsers.Add(user);
                else if (citizen is Community nestedCommunity) GetAllNestedUsers(nestedCommunity, allUsers);
            }
        }
        #endregion
        #endregion
    }
}

