using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Community : Citizen
    {
        //Basic information
        public string? Name { get; set; }
        public override string? FullName
        {
            get => string.Join(" ", Name, Address);
            set => throw new InvalidOperationException("Cannot set FullName directly in Community class.");
        }
        override public string? Address { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// Represents all the individuals associated with a community regardless of voting ability
        /// </summary>
        public List<Citizen> Citizens { get 
            {
                List<Citizen> homeOwners = Homes?.SelectMany(home => home.Owners.Keys).ToList() ?? new List<Citizen>();
                List<Citizen> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? new List<Citizen>();
                return NonResidentialCitizens.Union(homeOwners).Union(homeResidents).Distinct().ToList();
            }
        }
        
        public bool CanHaveHomes { get; set; }
        public List<Home> Homes { get; private set; }
       
        /// <summary>
        /// Can Citizens be added if they don't belong to a home
        /// </summary>
        public bool CanHaveNonResidentialCitizens { get; set; }
        public List<Citizen> NonResidentialCitizens { get; private set; }
        /// <summary>
        /// Policy for how long a proposal remains open for after it publishes. It's an int representing days.
        /// </summary>
        public int ProposalsExpirationDays { get; set; }
        public IVotingStrategy? VotingStrategy { get; set; }
        /// <summary>
        /// CitizensVotingValue is an int to protect against rounding errors.
        /// Each inheriting class can override who gets to vote and how much each vote counts. 
        /// In BaseCommunity, each Citizens gets one vote. 
        /// </summary>
        public Dictionary<Citizen, int> CitizensVotingValue
        {
            get
            {
                if (VotingStrategy == null) return new Dictionary<Citizen, int>();
                else return VotingStrategy.ReturnCitizensVotingValue(this);
            }
        }
        public int TotalVotes => CitizensVotingValue.Values.Sum();
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        public List<Proposal> AcceptedProposals => Proposals.Where(proposal => proposal.Passed == true).ToList();
        public List<Proposal> RejectedProposals => Proposals.Where(proposal => proposal.Passed == false).ToList();
        public List<Proposal> UndecidedProposals => Proposals.Where(proposal => proposal.Open == true).ToList();
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }

        public Community()
        {
            Citizenships = new();
            Homes = new();
            NonResidentialCitizens = new();
            Constitution = new();
            Proposals = new();
            ProposalsExpirationDays = 30;
            Dictamens = new();
            Roles = new();
        }
        /// <summary>
        /// Adding a citizen needs to ensure that no citizen is repeated
        /// </summary>
        /// <param name="user"></param>
        public void AddNonResidentialCitizen(Citizen citizen)
        {
            if (citizen != null && CanHaveNonResidentialCitizens)
            {
                NonResidentialCitizens.Add(citizen);
                if (NonResidentialCitizens.Contains(citizen)) citizen.AddCitizenship(this);
            }
        }
        public void RemoveNonResidentialCitizen(Citizen citizen)
        {
            if(citizen != null)
            {
                if (NonResidentialCitizens.Contains(citizen)) NonResidentialCitizens.Remove(citizen);
                //if after this the citizen doesn't appear in Citizens, remove his/her citizenship
                if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
            }
        }
        public void AddResidentToHome(Home home, Citizen citizen)
        {
            home.AddResident(citizen);
            if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
        }
        public void RemoveResidentFromHome(Home home, Citizen citizen)
        {
            home.RemoveResident(citizen);
            //if no longer appears in Citizens, remove
            if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
        }
        public void AddOwnerToHome(Home home, Citizen citizen, int percentage)
        {
            home.AddOwner(citizen, percentage);
            if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
        }
        public void RemoveOwnerFromHome(Home home, Citizen citizen)
        {
            home.RemoveOwner(citizen);
            if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
        }
        public bool PublishProposal(Proposal proposal)
        {
            //TODO: make sure the Proposal has all the correct info ValidateProposal() and return messages accordingly of what you are missing.
            //Ensure this proposal is for this community
            if (proposal.Community != this) throw new ArgumentException("Proposal.Community does not point to this Community");
            //Ensure it has a title
            if (proposal.Title == null) throw new ArgumentException("Proposal.Title is null");
            //Ensure it has a description
            if (proposal.Description == null) throw new ArgumentException("Proposal.Description is null");
            //I could ensure that the Author is either a Resident or a Dictamen of this Community, but perhaps in the future. 
            if (proposal.Author == null) throw new ArgumentException("Proposal.Author is null");
            //Publish date needs to be null or overriden
            if (proposal.PublishedDate != null) throw new ArgumentException("Proposal.PublishedDate is not null");
            //Dictamen cannot be empty
            if (proposal.Dictamen == null) throw new ArgumentException("Proposal.Dictamen is null");
            //The Proposal's Dictamen should be pointing to this Community
            if (proposal.Dictamen.Community != this) throw new ArgumentException("Proposal.Dictamen.Community does not point to this Community");
            //Votes should be empty
            if (proposal.Votes.Count != 0) throw new ArgumentException("Proposal.Votes is not empty");
            //If everything is Ok, add to add of list of Proposals and return True so that the proposal can set its PublishedDate
            proposal.Open = true;
            proposal.PublishedDate = DateTime.Now;
            proposal.ExpirationDate ??= proposal.PublishedDate?.AddDays(ProposalsExpirationDays);
            Proposals.Add(proposal);
            PropagateProposal(proposal);

            return true;
        }
        private protected virtual void PropagateProposal(Proposal parentProposal)
        {

            foreach (var citizen in Citizens)
            {
                if (citizen is Community propagatedCommunity)
                {
                    Proposal propagatedProposal = ReturnPropagatedProposal(propagatedCommunity, parentProposal);
                    //publish in its corresponding community which should call this method if there are more nested sub-communities
                    propagatedCommunity.PublishProposal(propagatedProposal);
                }
            }
        }
        static private protected Proposal ReturnPropagatedProposal(Community propagatedCommunity, Proposal parentProposal)
        {
            Proposal propagatedProposal = new(propagatedCommunity, parentProposal.Author)
            {
                Title = parentProposal.Title + $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>",
                Description = parentProposal.Description + $"<br>for community/para comunidad: {parentProposal.Community.Name}.<br>",
                ExpirationDate = parentProposal.ExpirationDate,
            };

            propagatedProposal.Dictamen = new PropagatedVoteDictamen(parentProposal)
            {
                Community = propagatedCommunity,
                Proposal = propagatedProposal
            };

            return propagatedProposal;
        }
        public void AddHome(Home home)
        {
            if (CanHaveHomes && !Homes.Contains(home))
            {
                home.Owners = new Dictionary<Citizen, int>();
                home.Residents = new List<Citizen>();
                Homes.Add(home);
            }
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
    }
}

