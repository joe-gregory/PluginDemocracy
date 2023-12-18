using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Community : BaseCitizen
    {
        //Basic information
        public string? Name { get; set; }
        public override string? FullName => string.Join(" ", Name, Address);
        public string OfficialCurrency { get; set; } = "USD";
        public List<string> OfficialLanguages { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// The Communities that this Home belongs to. Like to which Home owners association or Privada does this belong to. 
        /// </summary>
        public List<Community> Communities { get; private set; }
        [NotMapped]
        public override List<Community> Citizenships
        {
            get
            {
                List<Community> citizenships = new();
                citizenships.AddRange(HomeOwnerships.Select(o => o.Home));
                citizenships.AddRange(NonResidentialCitizenIn);
                citizenships.AddRange(Communities);
                return citizenships.Distinct().ToList();
            }
        }
        /// <summary>
        /// Represents all the individuals associated with a community regardless of voting ability
        /// </summary>
        [NotMapped]
        virtual public List<BaseCitizen> Citizens
        {
            get
            {
                List<BaseCitizen> homeOwners = Homes?.SelectMany(home => home.Owners.Keys).ToList() ?? new List<BaseCitizen>();
                List<User> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? new List<User>();
                List<Home> homes = Homes ?? new List<Home>();  
                return NonResidentialCitizens.Union(homeOwners).Union(homeResidents).Union(homes).Distinct().ToList();
            }
        }
        public bool CanHaveHomes { get; set; }
        public List<Home> Homes { get; private set; }
        /// <summary>
        /// Can Citizens be added if they don't belong to a home
        /// </summary>
        public bool CanHaveNonResidentialCitizens { get; set; }
        /// <summary>
        /// Citizens that don't live in a home
        /// </summary>
        public virtual List<BaseCitizen> NonResidentialCitizens { get; protected set; }
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
        public Dictionary<BaseCitizen, double> CitizensVotingValue
        {
            get
            {
                if (VotingStrategy == null) return new Dictionary<BaseCitizen, double>();
                else return VotingStrategy.ReturnVotingWeights(this);
            }
        }
        public double TotalVotes => CitizensVotingValue.Values.Sum();
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        [NotMapped]
        public List<Proposal> AcceptedProposals => Proposals.Where(proposal => proposal.Passed == true).ToList();
        [NotMapped]
        public List<Proposal> RejectedProposals => Proposals.Where(proposal => proposal.Passed == false).ToList();
        [NotMapped]
        public List<Proposal> UndecidedProposals => Proposals.Where(proposal => proposal.Open == true).ToList();
        public Accounting Accounting { get; }
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Project> Projects { get; }
        [NotMapped]
        public List<Project> ActiveProjects => Projects.Where(project => project.Active).ToList();
        public List<RedFlag> RedFlags { get; }
        public List<Post> Posts { get; }
        public Community()
        {
            Communities = new();
            OfficialLanguages = new();
            Homes = new();
            NonResidentialCitizens = new();
            Constitution = new();
            Proposals = new();
            ProposalsExpirationDays = 30;
            Accounting = new(this);
            Dictamens = new();
            Roles = new();
            Projects = new();
            RedFlags = new();
            Posts = new();
        }
        /// <summary>
        /// Adding a citizen needs to ensure that no citizen is repeated
        /// </summary>
        /// <param name="user"></param>
        public void AddNonResidentialCitizen(BaseCitizen citizen)
        {
            if (!CanHaveNonResidentialCitizens) throw new InvalidOperationException("Unable to add NonResidentialCitizens when CanHaveNonResidentialCitizens is set to false.");
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");
            if (!NonResidentialCitizens.Contains(citizen)) NonResidentialCitizens.Add(citizen);
            if (!citizen.NonResidentialCitizenIn.Contains(this)) citizen.NonResidentialCitizenIn.Add(this);
        }
        public void RemoveNonResidentialCitizen(BaseCitizen citizen)
        {
            if (NonResidentialCitizens.Contains(citizen)) NonResidentialCitizens.Remove(citizen);
            if (citizen.NonResidentialCitizenIn.Contains(this)) citizen.NonResidentialCitizenIn.Remove(this);
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

        public void AddHome(Home home)
        {
            if (!CanHaveHomes) throw new InvalidOperationException("Cannot add Homes when CanHaveHomes is set to false");
            if (home == null) throw new ArgumentNullException("Home cannot be null");

            if (!Homes.Contains(home)) 
            {
                Homes.Add(home);
                home.Communities.Add(this);
            }
        }
        public void RemoveHome(Home home)
        {
            if (Homes.Contains(home)) 
            {
                Homes.Remove(home);
                home.Communities.Remove(this);
            }
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
        public void Update()
        {
            Constitution.Update();
            foreach (var proposal in Proposals) proposal.Update();
            foreach (var role in Roles) role.Update();
        }
        //UTILITY METHODS: 
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
                Title = $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>" + (parentProposal.Dictamen?.Title ?? "Error: missing."),
                Description = $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>" + (parentProposal.Dictamen?.Description ?? "Error: missing."),
                Proposal = propagatedProposal,
                Community = propagatedCommunity,
            };

            return propagatedProposal;
        }
        public List<User> ReturnAllNestedUsers()
        {
            HashSet<User> allUsers = new();
            GetAllNestedUsers(this, allUsers);
            return allUsers.ToList();
        }
        public void RaiseRedFlag(User user, string description, BaseRedFlaggable itemFlagged)
        {
            RedFlag newRedFlag = new(this, user, description, itemFlagged);
            RedFlags.Add(newRedFlag);
        }
        public void CreatePost(User user, string body)
        {
            Post newPost = new(user, body);
            Posts.Add(newPost);
        }
        public void RemovePost(Post post)
        {
            if (Posts.Contains(post)) Posts.Remove(post);
        }
        static private void GetAllNestedUsers(Community community, HashSet<User> allUsers)
        {
            foreach (var citizen in community.Citizens)
            {
                if (citizen is User user) allUsers.Add(user);
                else if (citizen is Community nestedCommunity) GetAllNestedUsers(nestedCommunity, allUsers);
            }
        }
    }
}

