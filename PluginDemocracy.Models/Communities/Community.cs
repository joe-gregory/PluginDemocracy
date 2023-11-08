using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Community : BaseCitizen
    {
        //Basic information
        public int Id { get; set; }
        public string? Name { get; set; }
        public override string? FullName => string.Join(" ", Name, Address);
        public CultureInfo? OfficialCurrency { get; set; }
        public List<CultureInfo> OfficialLanguages { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// Represents all the individuals associated with a community regardless of voting ability
        /// </summary>
        virtual public List<BaseCitizen> Citizens
        {
            get
            {
                List<BaseCitizen> homeOwners = Homes?.SelectMany(home => home.Owners.Keys).ToList() ?? new List<BaseCitizen>();
                List<BaseCitizen> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? new List<BaseCitizen>();
                return NonResidentialCitizens.Union(homeOwners).Union(homeResidents).Distinct().ToList();
            }
        }

        public bool CanHaveHomes { get; set; }
        public List<Home> Homes { get; private set; }

        /// <summary>
        /// Can Citizens be added if they don't belong to a home
        /// </summary>
        public bool CanHaveNonResidentialCitizens { get; set; }
        public List<BaseCitizen> NonResidentialCitizens { get; private set; }
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
        public Dictionary<BaseCitizen, int> CitizensVotingValue
        {
            get
            {
                if (VotingStrategy == null) return new Dictionary<BaseCitizen, int>();
                else return VotingStrategy.ReturnVotingWeights(this);
            }
        }
        public int TotalVotes => CitizensVotingValue.Values.Sum();
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        public List<Proposal> AcceptedProposals => Proposals.Where(proposal => proposal.Passed == true).ToList();
        public List<Proposal> RejectedProposals => Proposals.Where(proposal => proposal.Passed == false).ToList();
        public List<Proposal> UndecidedProposals => Proposals.Where(proposal => proposal.Open == true).ToList();
        public Accounting Accounting { get; }
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }
        public List<Project> Projects { get; }
        public List<Project> ActiveProjects => Projects.Where(project => project.Active).ToList();
        public List<RedFlag> RedFlags { get; }
        public List<Post> Posts { get; }
        public Community()
        {
            Citizenships = new();
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
            if (citizen != null && CanHaveNonResidentialCitizens)
            {
                if (CanHaveNonResidentialCitizens)
                {
                    NonResidentialCitizens.Add(citizen);
                    if (NonResidentialCitizens.Contains(citizen)) citizen.AddCitizenship(this);
                }
                else throw new InvalidOperationException("Cannot add NonResidential Citizens when CanHaveNonResidentialCitizens is set to false.");

            }
            else throw new ArgumentException("Citizen cannot be null when adding to NonResidentialCitizens.");
        }
        public void RemoveNonResidentialCitizen(BaseCitizen citizen)
        {
            if (citizen != null)
            {
                if (NonResidentialCitizens.Contains(citizen)) NonResidentialCitizens.Remove(citizen);
                //if after this the citizen doesn't appear in Citizens, remove his/her citizenship
                if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
            }
        }
        public void AddResidentToHome(BaseCitizen citizen, Home home)
        {
            home.AddResident(citizen);
            if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
        }
        public void RemoveResidentFromHome(BaseCitizen citizen, Home home)
        {
            home.RemoveResident(citizen);
            //if no longer appears in Citizens, remove
            if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
        }
        public void AddOwnerToHome(BaseCitizen citizen, int percentage, Home home)
        {
            home.AddOwner(citizen, percentage);
            if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
        }
        public void RemoveOwnerFromHome(BaseCitizen citizen, Home home)
        {
            home.RemoveOwner(citizen);
            if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
        }
        public bool PublishProposal(Proposal proposal)
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
            proposal.ExpirationDate ??= proposal.PublishedDate?.AddDays(ProposalsExpirationDays);
            Proposals.Add(proposal);
            if (VotingStrategy.ShouldProposalPropagate(proposal)) PropagateProposal(proposal);

            return true;
        }

        public void AddHome(Home home)
        {
            if (home == null) throw new ArgumentException("Home cannot be null");
            if (CanHaveHomes)
            {
                if (!Homes.Contains(home))
                {
                    home.Owners = new Dictionary<BaseCitizen, int>();
                    home.Residents = new List<BaseCitizen>();
                    Homes.Add(home);
                }
                else throw new InvalidOperationException("Cannot add a home twice to the list");
            }
            else throw new InvalidOperationException("Cannot add Homs when CanHaveHomes is set to false");
        }
        public void RemoveHome(Home home)
        {
            if (Homes.Contains(home)) Homes.Remove(home);
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
                    propagatedCommunity.PublishProposal(propagatedProposal);
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
        public void RaiseRedFlag(User user, string description, IRedFlaggable itemFlagged)
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
            if(Posts.Contains(post)) Posts.Remove(post);
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

