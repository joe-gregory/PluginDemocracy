using System;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    public class Proposal
    {
        public Guid Guid { get; }
        public ICommunity Community { get; }
        public ProposalDraft Origin { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime PublishedDate { get; }
        public IDictamen Dictamen { get; }

        //Schemas
        public IProposalOpenStatusSchema ProposalOpenStatusSchema { get; }
        public IProposalPassingSchema ProposalPassingSchema { get; }

        public IVotingEligibilitySchema VotingEligibilitySchema { get; }
        public IVotingChangeSchema VotingChangeSchema { get; }

        //Properties
        public bool OpenStatus
        {
            public get
            {
                return ProposalOpenStatusSchema.IsOpen(this)
            }
        }
        public bool PassedStatus { 
            public get
            {
                return ProposalPassingSchema.DidPass(this);
            }
        }

        public List<Vote> Votes { public get; private set; }
        public Dictionary<Citizen, int> WeightedVotes
        {
            public get
            {
                return ProposalPassingSchema.WeightedVotes(this);
            }
        }

        public Proposal(ProposalDraft origin)
        {
            Guid = Guid.NewGuid();
            Origin = origin;
            Title = origin.Title;
            Description = origin.Description;
            PublishedDate = DateTime.UtcNow;
            Dictamen = origin.Dictamen;
            ProposalOpenStatusSchema = origin.OpenStatusSchema;
            ProposalPassingSchema = origin.PassingSchema;
            VotingEligibilitySchema = origin.VotingEligibilitySchema;
            VotingChangeSchema = origin.VotingChangeSchema;
            ProposalOpenStatusChecker();
            ProposalPassedStatusChecker();
            Votes = new List<Vote>();
        }

        //Methods
        public bool Vote(Citizen citizen, bool voteValue)
        {
            if (VotingEligibilitySchema.CanVote(citizen) && OpenStatus)
            {
                Votes.Add(new Vote(citizen.Guid, voteValue));
                //run a method to check if anything has changed? 
                return true
            }
            else
            {
                return false;
            }
        }
    }
}