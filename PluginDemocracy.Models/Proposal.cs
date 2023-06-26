using System;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    internal class Proposal
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
        public bool OpenStatus { public get; private set; }
        public bool PassedStatus { public get; private set; }

        public List<Vote> Votes { public get; private set; }
        public Dictionary<Citizen, int> WeightedVotes {public get;}

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
        public void Vote(Citizen citizen, bool voteValue)
        {
            if (VotingEligibilitySchema.CanVote(citizen)) Votes.Add(new Vote(citizen.Guid, voteValue));
            else throw new Exception;
        }

        private void ProposalOpenStatusChecker()
        {
            OpenStatus = ProposalOpenStatusSchema.IsOpen(this);
        }

        private void ProposalPassedStatusChecker()
        {
            PassedStatus = ProposalPassingSchema.DidPass(this);
        }

        private void IDictamenUpdater()
        {
            //Publish if Proposal passes. Publishing means adding to Community.Dictamens
            //Make status inactive if proposal status has dropped to not passed
        }
    }
}