namespace PluginDemocracy.Models
{
    public class Proposal
    {
        /// <summary>
        /// Basic information about the proposal
        /// </summary>
        public Guid Guid { get; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public User Author { get; }
        public Community Community { get; set; }
        public bool Published { get { return DateTime.Now > PublishedDate; } }
        /// <summary>
        /// When a proposal is Published(), a PublishedDate is set and Open status is set to true.
        /// </summary>
        public DateTime? PublishedDate { get; private set; }
        public DateTime? ExpirationDate { get; set; }
        public BaseDictamen? Dictamen { get; set; }
        public IVotingStrategy? VotingStrategy { get; set; }
        public Dictionary<BaseCitizen, int> CitizensVotingValue
        {
            get
            {
                if (VotingStrategy == null) return new Dictionary<BaseCitizen, int>();
                else return VotingStrategy.ReturnCitizensVotingValue(Community);
            }
        }
        /// <summary>
        /// Total amount of votes possible. Some citizens may have more VotingValue than others. This is the sum of all VotingValues.
        /// A Proposal passes when VotesInFavor is more than half of TotalVotingValuesSum
        /// </summary>
        public int TotalVotingValuesSum => CitizensVotingValue.Values.Sum();
        private readonly List<Vote> _votes;
        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();
        public IReadOnlyList<Vote> VotesInFavor => _votes.Where(vote => vote.InFavor == true).ToList().AsReadOnly();
        public IReadOnlyList<Vote> VotesAgainst => _votes.Where(vote => vote.InFavor == false).ToList().AsReadOnly();
        public int TotalValueVotesInFavor => VotesInFavor.Sum(vote => vote.VoteValueInFavor);
        public int TotalValueVotesAgainst => VotesAgainst.Sum(vote => vote.VoteValueAgainst);
        /// <summary>
        /// Gets a value indicating whether the proposal is currently open for voting.
        /// This takes into account both the community's open status strategy and whether the proposal has been published.
        /// </summary>
        public bool Open { get; set; }
        /// <summary>
        /// Indicates if the proposal has passed (majority of votes are true).
        /// </summary>
        public bool Passed { get; set; }
        public Proposal(Community community, User user)
        {
            Guid = Guid.NewGuid();
            Author = user;
            Community = community;
            VotingStrategy = community.VotingStrategy;
            _votes = new();
            //initialize tally
            foreach(BaseCitizen citizen in CitizensVotingValue.Keys)
            {
                _votes.Add(new Vote(this, citizen));
            }
            Passed = false;
        }
        /// <summary>
        /// If the Proposal is Open, anybody can vote. If someone already voted and they vote again, remove old vote and add new one. 
        /// </summary>
        /// <param name="citizen">The citizen voting</param>
        /// <param name="voteValue">False is against (proposal failing). True is in favor of proposal passing.</param>
        /// <exception cref="Exception"></exception>
        public void Vote(BaseCitizen citizen, bool voteValue)
        {
            // Check if the proposal is open for voting
            if (!Open)
            {
                throw new Exception("Unable to vote: Proposal is not open for voting.");
            }

            // Remove the existing vote by the same citizen, if any
            _votes.RemoveAll(vote => vote.Citizen == citizen);

            // Add the new or updated vote
            _votes.Add(new Vote(this, citizen, voteValue));

            // Update the state of the proposal
            Update();
        }
        public void UpdatePassedStatus()
        {
            if (TotalValueVotesInFavor > TotalVotingValuesSum / 2) Passed = true;
            else Passed = false;
        }


        public void Update()
        {
            Open = Published && DateTime.Now < ExpirationDate;
            bool prevPassedValue = Passed;
            UpdatePassedStatus();
            if(prevPassedValue == false && Passed == true)
            {
                IssueDictamen();
            }
        }
        public void IssueDictamen()
        {
            if (Dictamen == null) throw new Exception("Proposal.Dictamen is null");
            Dictamen.Issue();
        }
    }
}