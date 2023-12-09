using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a vote
    /// </summary>
    public class Vote
    {
        public int Id { get; set; }
        public Proposal? Proposal { get; private set; }
        public BaseCitizen Citizen { get; private set; }
        public bool InFavor { get; private set; }
        public int VoteWeight { get; private set; }
        public int VoteValueInFavor { get; private set; }
        public int VoteValueAgainst { get; private set; }
        public DateTime Date { get; private set; }
        protected Vote()
        {
            Citizen = new User(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now, new CultureInfo("en-US"));
        }
        /// <summary>
        /// This is the default constructor. The other is only for use by the PropagatedVoteDictamen class
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="citizen"></param>
        /// <param name="inFavor"></param>
        public Vote(Proposal proposal, BaseCitizen citizen, bool inFavor)
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
        public Vote(Proposal proposal, BaseCitizen citizen, bool inFavor, DateTime date)
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