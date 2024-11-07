namespace PluginDemocracy.Models
{
    public class Vote
    {
        public Guid Id { get; private set; }
        public Proposal Proposal { get; init; }
        public User Voter { get; init; }
        public VoteDecision Decision { get; init; }
        public DateTime DateTime { get; init; }
        /// <summary>
        /// Protected constructor for the benefit of EFC
        /// </summary>
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Vote() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal Vote(User user, VoteDecision decision, Proposal proposal)
        {
            Voter = user;
            Decision = decision;
            Proposal = proposal;
            DateTime = DateTime.Now;
        }
    }
    public enum VoteDecision
    {
        InFavor, 
        Against,
        Abstain
    }
}
