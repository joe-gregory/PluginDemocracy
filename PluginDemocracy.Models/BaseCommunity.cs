using PluginDemocracy.Models.Interfaces;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public abstract class BaseCommunity : ICitizen, IProposalStrategies
    {
        //Basic information
        public Guid Guid { get; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public IReadOnlyDictionary<ICitizen, decimal>? Owners => _owners?.AsReadOnly();
        protected Dictionary<ICitizen, decimal>? _owners;
        /// <summary>
        /// Those who can vote
        /// </summary>
        public IReadOnlyList<ICitizen> Citizens { get { return _citizens.AsReadOnly(); } }
        private List<ICitizen> _citizens;
        public IReadOnlyDictionary<ICitizen, decimal> CitizensVotingWeights { get { return CommunitysCitizensVotingWeightsStrategy.ReturnCitizensVotingWeights(this); } }
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        public List<BaseDictamen> Dictamens { get; private set; }
        public List<Role> Roles { get; private set; }
        /// <summary>
        /// Strategies for the community
        /// </summary>
        public IProposalPassStrategy ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy CommunitysCitizensVotingWeightsStrategy { get; set; }



        public IReadOnlyList<BaseCommunity> Communities { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BaseCommunity()
        {
            Guid = new Guid();
        }
        public void AddCommunity(BaseCommunity community)
        {
            throw new NotImplementedException();
        }
        public void RemoveCommunity(BaseCommunity community)
        {
            throw new NotImplementedException();
        }
        public void JoinCitizen(ICitizen citizen)
        {
            if (!Citizens.Contains(citizen))
            {
                _citizens.Add(citizen);
                citizen.AddCommunity(this);
            }
        }
        public void RemoveCitizen(ICitizen citizen)
        {
            _citizens.Remove(citizen);
            citizen.RemoveCommunity(this);
        }
        public void ReturnProposal()
        {
            //return a proposal with the default strategies... cannot undo those. 
        }
        public bool PublishProposal(Proposal proposal)
        {
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
            //It needs to have all the strategies
            if (proposal.ProposalPassStrategy == null) throw new ArgumentException("Proposal.ProposalPassStrategy is null");
            if (proposal.ProposalOpenStatusStrategy == null) throw new ArgumentException("Proposal.ProposalOpenStatusStrategy is null");
            if (proposal.CitizenVotingEligibilityStrategy == null) throw new ArgumentException("Proposal.CitizenVotingEligibilityStrategy is null");
            if (proposal.CitizenVotingChangeStrategy == null) throw new ArgumentException("Proposal.CitizenVotingChangeStrategy is null");
            if (proposal.CommunitysCitizensVotingWeightsStrategy == null) throw new ArgumentException("Proposal.CommunitysCitizensVotingWeightsStrategy is null");
            //If everything is Ok, add to add of list of Proposals and return True so that the proposal can set its PublishedDate
            Proposals.Add(proposal);
            return true;
        }
        public bool IssueDictamen(BaseDictamen dictamen)
        {
            //if title is empty, throw exception
            if (dictamen.Title == null) throw new ArgumentException("Dictamen.Title is null");
            //if description is empty, throw exception
            if (dictamen.Description == null) throw new ArgumentException("Dictamen.Description is null");
            //The Dictamen's Community should be pointing to this Community
            if (dictamen.Community != this) throw new ArgumentException("Dictamen.Community does not point to this Community");
            //if author is empty, throw exception
            if (dictamen.Origin == null) throw new ArgumentException("Dictamen.Author is null");
            //if the author is either not a Role or a current Proposal, do not allow to run
            if(dictamen.Origin is Role role)
            {
                if (!Roles.Contains(role)) throw new ArgumentException("Dictamen.Author is not in Roles.");
            }
            if(dictamen.Origin is Proposal proposal)
            {
                if (!Proposals.Contains(proposal)) throw new ArgumentException("Dictamen.Author is not in Proposals");
                //Does this author have the right powers to do this?
            }
            if (dictamen.IssueDate != null) throw new ArgumentException("Dictamen.IssueDate is not null");
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
