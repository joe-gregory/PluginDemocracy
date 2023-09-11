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
        /// <summary>
        /// When a proposal is Published(), a PublishedDate is set and Open status is set to true.
        /// </summary>
        public DateTime? PublishedDate { get; private set; }
        public Dictamen Dictamen { get; }
        /// <summary>
        /// These are set by the community. The only exception is if a passed IDictamen creates a proposal then these options can be different. 
        /// </summary>
        /// <summary>
        /// Gets a value indicating whether the proposal is currently open for voting.
        /// This takes into account both the community's open status strategy and whether the proposal has been published.
        /// </summary>
        public bool Open;
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
            Passed = false;
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
                PublishedDate = DateTime.Now;
                Open = true;
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
            Dictamen.IssueDate = DateTime.Now;
        }
    }
}