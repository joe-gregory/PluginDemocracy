﻿namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a vote
    /// </summary>
    public class Vote
    {
        public Proposal Proposal { get; }
        public Citizen Citizen { get; }
        public bool? InFavor { get; }
        public int VoteWeight { get; }
        public int VoteValueInFavor { get; }
        public int VoteValueAgainst { get; }
        public DateTime? Date { get; }

        public Vote(Proposal proposal, Citizen citizen, bool inFavor)
        {
            Proposal = proposal;
            Citizen = citizen;
            InFavor = inFavor;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            if (Proposal.CitizensVotingValue.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.CitizensVotingValue[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
            if (InFavor != null && InFavor == true) VoteValueInFavor = VoteWeight;
            else VoteValueAgainst = VoteWeight;
            Date = DateTime.UtcNow;
        }
        public Vote(Proposal proposal, Citizen citizen)
        {
            Proposal = proposal;
            Citizen = citizen;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            InFavor = null;
            if (Proposal.CitizensVotingValue.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.CitizensVotingValue[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
        }
    }
}