using PluginDemocracy.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    public class Proposal : Dictamen.DictamenIssuer
    {
        /// <summary>
        /// Basic information about the proposal
        /// </summary>
        public Guid Guid { get; }
        public string? Title { get; }
        public string? Description { get; }
        public IProposalAuthor Author { get; }
        public bool Published { get { return DateTime.Now > PublishedDate; } }
        /// <summary>
        /// When a proposal is Published(), a PublishedDate is set and Open status is set to true.
        /// </summary>
        public DateTime? PublishedDate { get; private set; }
        public BaseDictamen? Dictamen { get; }
        /// <summary>
        /// These are set by the community. The only exception is if a passed IDictamen creates a proposal then these options can be different. 
        /// </summary>
        /// <summary>
        /// Gets a value indicating whether the proposal is currently open for voting.
        /// This takes into account both the community's open status strategy and whether the proposal has been published.
        /// </summary>
        public bool Open { get; private set; }
        /// <summary>
        /// Indicates if the proposal has passed according to the passing strategy
        /// </summary>
        public bool Passed;
        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();
        private List<Vote> _votes;
        /// <summary>
        /// Strategies about the behavior of the proposal
        /// </summary>
        public IProposalPassStrategy ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy CommunitysCitizensVotingWeightsStrategy { get; set; }

        //Methods
        /// <summary>
        /// There are 2 overload constructors. One for when a proposal is created by a User and one for when a proposal is created by a Dictamen.
        /// When it is created by the user, we need to know to what Community this applies to and ensure the Author gets stored. 
        /// </summary>
        /// <param name="community"></param>
        /// <param name="user"></param>
        public Proposal(BaseCommunity community, User user) : base (community)
        {
            Guid = Guid.NewGuid();
            Author = user;
            ProposalPassStrategy = community.ProposalPassStrategy;
            ProposalOpenStatusStrategy = community.ProposalOpenStatusStrategy;
            CitizenVotingEligibilityStrategy = community.CitizenVotingEligibilityStrategy;
            CitizenVotingChangeStrategy = community.CitizenVotingChangeStrategy;
            CommunitysCitizensVotingWeightsStrategy = community.CommunitysCitizensVotingWeightsStrategy;
            _votes = new();
            Passed = false;
        }
        public Proposal(Dictamen dictamen) : base(dictamen.Community) 
        {
            if (dictamen.ProposalPassStrategy == null || dictamen.ProposalOpenStatusStrategy == null || dictamen.CitizenVotingEligibilityStrategy == null || dictamen.CitizenVotingChangeStrategy == null || dictamen.CommunitysCitizensVotingWeightsStrategy == null) throw new ArgumentException("Dictamen is missing IProposalStrategies parameters");
            Guid = Guid.NewGuid();
            Author = dictamen;
            ProposalPassStrategy = dictamen.ProposalPassStrategy;
            ProposalOpenStatusStrategy = dictamen.ProposalOpenStatusStrategy;
            CitizenVotingEligibilityStrategy = dictamen.CitizenVotingEligibilityStrategy;
            CitizenVotingChangeStrategy = dictamen.CitizenVotingChangeStrategy;
            CommunitysCitizensVotingWeightsStrategy = dictamen.CommunitysCitizensVotingWeightsStrategy;
            _votes = new();
            Passed = false;
        }
        public void Vote(BaseCitizen citizen, bool voteValue)
        {
            //Checking if proposal can be voted on
            if (Open)
            {
                //Checking if citizen can vote:
                if (CitizenVotingEligibilityStrategy.CanVote(citizen, this) && CitizenVotingChangeStrategy.CanVote(citizen, this))
                {
                    _votes.Add(new Vote(citizen, voteValue));
                    Update();
                }
            }
            else throw new Exception("Unable to vote");
        }
        /// <summary>
        /// The fields are nullable so that it can exist in draft form as a user works on it. Once it is published, the fields cannot be changed. 
        /// </summary>
        public void Publish()
        {
            //Check that it has everything to pass and if so published, otherwise throw an error: 
            if (PublishedDate == null)
            {
                if (Community.PublishProposal(this))
                {
                    PublishedDate = DateTime.Now;
                    Open = true;
                }
            }
            else throw new Exception("Proposal had already been published");
        }
        public void Update()
        {
            Open = ProposalOpenStatusStrategy.IsOpen(this) && Published;
            Passed = ProposalPassStrategy.HasItPassed(this);
        }
        public void IssueDictamen()
        {
            if (Dictamen == null) throw new Exception("Proposal.Dictamen is null");
            Dictamen.Issue();
        }
        public void AddAuthor(User user)
        {
            throw new NotImplementedException();
            //Also implemented in User
        }
        public void RemoveAuthor(User user)
        {
            throw new NotImplementedException();
        }
    }
}