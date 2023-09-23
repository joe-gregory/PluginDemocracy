using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Community : BaseCitizen
    {
        //Basic information
        public string? Name { get; set; }
        public override string? FullName
        {
            get => string.Join(" ", Name, Address);
            set => throw new InvalidOperationException("Cannot set FullName directly in BaseCommunity class.");
        }
        override public string? Address { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// Represents all the individuals associated with a community regardless of voting ability
        /// </summary>
        virtual public List<BaseCitizen> Citizens { get; set; }
        /// <summary>
        /// Policy for how long a proposal remains open for after it publishes
        /// </summary>
        public int ProposalsExpirationDays { get; set; }
        public IVotingStrategy? VotingStrategy { get; set; }
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
                else return VotingStrategy.ReturnCitizensVotingValue(this);
            }
        }
        public int TotalVotes => CitizensVotingValue.Values.Sum();
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }

        public Community()
        {
            Citizenships = new();
            Citizens = new();
            Constitution = new();
            Proposals = new();
            Dictamens = new();
            Roles = new();
        }
        /// <summary>
        /// Adding a citizen needs to ensure that no citizen is repeated
        /// </summary>
        /// <param name="user"></param>
        virtual public void AddCitizen(BaseCitizen user)
        {
            if (user != null)
            {
                if (!Citizens.Contains(user))
                {
                    Citizens.Add(user);
                    user.AddCitizenship(this);
                }
                else throw new Exception("Community.Citizens already contains user");
            }
            else throw new Exception("User cannot be null when adding to Community.Citizens");
        }
        virtual public void RemoveCitizen(BaseCitizen user)
        {
            if (user != null)
            {
                if (Citizens.Contains(user))
                {
                    Citizens.Remove(user);
                    user.RemoveCitizenship(this);
                }
                else throw new Exception("Community.Citizens does not contain User");
            }
            else throw new Exception("User cannot be null when removing from Community.Citizens");
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
            Proposals.Add(proposal);
            PropagateProposal(proposal);

            return true;
        }
        public void PropagateProposal(Proposal parentProposal)
        {

            foreach (var citizen in Citizens)
            {
                if (citizen is Community propagatedCommunity)
                {
                    Proposal propagatedProposal = new(propagatedCommunity, parentProposal.Author)
                    {
                        Title = parentProposal.Title + $"\nfor/para: {parentProposal.Community.Name}",
                        Description = parentProposal.Description + $"\nfor/para: {parentProposal.Community.Name}",
                        ExpirationDate = parentProposal.ExpirationDate,

                    };

                    propagatedProposal.Dictamen = new PropagatedProposalDictamen(parentProposal)
                    {
                        Community = propagatedCommunity,
                        Proposal = propagatedProposal
                    };

                    //publish in its corresponding community which should call this method if there are more nested sub-communities
                    propagatedCommunity.PublishProposal(propagatedProposal);
                }
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
    }

}

