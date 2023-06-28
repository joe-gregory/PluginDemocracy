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
        public Dictamen Dictamen { get; }

        //Schemas
        public IProposalOpenStatusSchema OpenStatusSchema { get; }
        public IProposalPassingSchema PassingSchema { get; }

        public IVotingEligibilitySchema VotingEligibilitySchema { get; }
        public IVotingChangeSchema VotingChangeSchema { get; }

        //Properties
        public bool OpenStatus
        {
            public get
            {
                return OpenStatusSchema.IsOpen(this)
            }
        }
        public bool Passed { 
            public get
            {
                return PassingSchema.DidPass(this);
            }
        }

        public List<Vote> Votes { public get; private set; }
        public Dictionary<Citizen, int> WeightedVotes
        {
            public get
            {
                return PassingSchema.WeightedVotes(this);
            }
        }

        internal Proposal(ProposalDraft origin)
        {
            Guid = Guid.NewGuid();
            Origin = origin;
            Title = origin.Title;
            Description = origin.Description;
            PublishedDate = DateTime.UtcNow;
            Dictamen = origin.Dictamen;
            OpenStatusSchema = origin.OpenStatusSchema;
            PassingSchema = origin.PassingSchema;
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
                return true
            }
            else
            {
                return false;
            }
        }
    }
}