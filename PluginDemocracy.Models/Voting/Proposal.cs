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
        public IProposalOpenStatusStrategy OpenStatusSchema { get; }
        public IProposalPassingStrategy PassingSchema { get; }

        public IVotingEligibilityStrategy VotingEligibilitySchema { get; }
        public IVotingChangeStrategy VotingChangeSchema { get; }

        //Properties
        public bool OpenStatus
        {
            public get
            {
                return OpenStatusSchema.IsOpen(this);
            }
        }

        public bool Passed { 
            public get
            {
                return PassingSchema.DidPass(this);
            }
        }

        public List<Vote> Votes { public get; private set; }

        public Dictionary<Member, int, bool> VotesWeights
        {
            public get
            {
                return PassingSchema.VotesWeights(this);
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
        public bool RecordVote(Member member, bool vote)
        {
            if (OpenStatus && VotingEligibilitySchema.CanVote(member) && VotingChangeSchema.CanVote(member, this.Votes))
            {
                Votes.Add(new Vote(citizen.Guid, vote));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}