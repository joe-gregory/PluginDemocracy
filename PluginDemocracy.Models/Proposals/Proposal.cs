using PluginDemocracy.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    public class Proposal
    {
        /// <summary>
        /// Basic information about the proposal
        /// </summary>
        public Guid Guid { get; }
        public Community Community { get; }
        public string? Title { get; }
        public string? Description { get; }
        public List<User> Authors { get; }
        public bool Published { get { return DateTime.Now > PublishedDate; } }
        public DateTime? PublishedDate { get; private set; }
        public Dictamen? Dictamen { get; set; }

        /// <summary>
        /// These are set by the community. The only exception is if a passed IDictamen creates a proposal then these options can be different. 
        /// </summary>
        public IProposalPassStrategy ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy CommunitysCitizensVotingWeightsStrategy { get; set; }

        /// <summary>
        /// Gets a value indicating whether the proposal is currently open for voting.
        /// This takes into account both the community's open status strategy and whether the proposal has been published.
        /// </summary>
        public bool Open => ProposalOpenStatusStrategy.IsOpen(this) && Published;
        /// <summary>
        /// Indicates if the proposal has passed according to the passing strategy
        /// </summary>
        public bool Passed => ProposalPassStrategy.HasItPassed(this);

        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();
        private List<Vote> _votes;

        //Methods
        public Proposal(Community community, User user)
        {
            Guid = Guid.NewGuid();
            Community = community;
            Authors = new()
            {
                user
            };
            ProposalPassStrategy = community.ProposalPassStrategy;
            ProposalOpenStatusStrategy = community.ProposalOpenStatusStrategy;
            CitizenVotingEligibilityStrategy = community.CitizenVotingEligibilityStrategy;
            CitizenVotingChangeStrategy = community.CitizenVotingChangeStrategy;
            CommunitysCitizensVotingWeightsStrategy = community.CommunitysCitizensVotingWeightsStrategy;
            _votes = new();
        }
        public void Vote(Citizen citizen, bool voteValue)
        {
            //Checking if proposal can be voted on
            if (Open)
            {
                //Checking if citizen can vote:
                if (CitizenVotingEligibilityStrategy.CanVote(citizen, this) && CitizenVotingChangeStrategy.CanVote(citizen, this))
                {
                    _votes.Add(new Vote(citizen, voteValue));
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
            if (PublishedDate == null) PublishedDate = DateTime.Now;
            else throw new Exception("Proposal had already published");
        }
    }
}