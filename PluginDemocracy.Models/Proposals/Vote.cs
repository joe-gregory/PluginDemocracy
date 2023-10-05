namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a vote
    /// </summary>
    public class Vote
    {
        public Proposal Proposal { get; }
        public Citizen Citizen { get; }
        public bool InFavor { get; }
        public int VoteWeight { get; }
        public int VoteValueInFavor { get; }
        public int VoteValueAgainst { get; }
        public DateTime Date { get; }
        /// <summary>
        /// This is the default constructor. The other is only for use by the PropagatedVoteDictamen class
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="citizen"></param>
        /// <param name="inFavor"></param>
        public Vote(Proposal proposal, Citizen citizen, bool inFavor)
        {
            Proposal = proposal;
            Citizen = citizen;
            InFavor = inFavor;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            if (Proposal.VotingWeights.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.VotingWeights[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
            if (InFavor == true) VoteValueInFavor = VoteWeight;
            else VoteValueAgainst = VoteWeight;
            Date = DateTime.UtcNow;
        }
        /// <summary>
        /// This constructor is used for when PropagatedVoteDictamen needs to add a vote to the child proposal with the same date and inFavor
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="citizen"></param>
        /// <param name="inFavor"></param>
        /// <param name="date"></param>
        public Vote(Proposal proposal, Citizen citizen, bool inFavor, DateTime date)
        {
            Proposal = proposal;
            Citizen = citizen;
            InFavor = inFavor;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            if (Proposal.VotingWeights.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.VotingWeights[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
            if (InFavor == true) VoteValueInFavor = VoteWeight;
            else VoteValueAgainst = VoteWeight;
            Date = date;
        }
    }
}