using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class Proposal
    {
        /// <summary>
        /// Basic information about the proposal
        /// </summary>
        public int Id { get; set; }
        public Guid Guid { get; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public User Author { get; }
        public Community Community { get; set; }
        public List<RedFlag> AddressesRedFlags { get; }
        /// <summary>
        /// PublishedDate is set by the Community when Community.PublishProposal() is invoked.
        /// </summary>
        public DateTime? PublishedDate { get; set; }
        [NotMapped]
        public bool Published { get { return PublishedDate != null && DateTime.UtcNow > PublishedDate; } }
        public DateTime ExpirationDate { get; set; }
        public BaseDictamen? Dictamen { get; set; }
        public BaseVotingStrategy? VotingStrategy { get; set; }
        public Dictionary<BaseCitizen, int> VotingWeights
        {
            get
            {
                if (Community != null && VotingStrategy != null) return VotingStrategy.ReturnVotingWeights(Community);
                else return new Dictionary<BaseCitizen, int>();
            }
        }
        /// <summary>
        /// Total amount of votes possible. Some citizens may have more VotingValue than others. This is the sum of all VotingValues.
        /// A Proposal passes when VotesInFavor is more than half of TotalVotingValuesSum
        /// </summary>
        private readonly List<Vote> _votes;
        [NotMapped]
        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();
        [NotMapped]
        public IReadOnlyList<Vote> VotesInFavor => _votes.Where(vote => vote.InFavor == true).ToList().AsReadOnly();
        [NotMapped]
        public IReadOnlyList<Vote> VotesAgainst => _votes.Where(vote => vote.InFavor == false).ToList().AsReadOnly();
        [NotMapped]
        public int TotalVotingValuesSum => VotingWeights.Values.Sum();
        [NotMapped]
        public int TotalValueVotesInFavor => VotesInFavor.Sum(vote => vote.VoteValueInFavor);
        [NotMapped]
        public int TotalValueVotesAgainst => VotesAgainst.Sum(vote => vote.VoteValueAgainst);
        /// <summary>
        /// Gets a value indicating whether the proposal is currently open for voting.
        /// This takes into account both the community's open status strategy and whether the proposal has been published.
        /// </summary>
        public bool Open { get; set; }
        /// <summary>
        /// Indicates if the proposal has passed (majority of votes are true).
        /// </summary>
        public bool? Passed { get; set; }
        protected Proposal() 
        {
            _votes = new();
            AddressesRedFlags = new();
            Author = new();
            Community = new();
        }
        public Proposal(Community community, User user)
        {
            Guid = Guid.NewGuid();
            Author = user;
            Community = community;
            AddressesRedFlags = new();
            VotingStrategy = community.VotingStrategy;
            _votes = new();
            Passed = null;
        }
        /// <summary>
        /// If the Proposal is Open, anybody can vote. If someone already voted and they vote again, remove old vote and add new one. 
        /// </summary>
        /// <param name="citizen">The citizen voting</param>
        /// <param name="inFavor">False is against (proposal failing). True is in favor of proposal passing.</param>
        /// <exception cref="Exception"></exception>
        public void Vote(BaseCitizen citizen, bool inFavor)
        {
            Update();
            // Check if the proposal is open for voting
            if (!Open) throw new Exception("Unable to vote: Proposal is not open for voting.");

            // Remove the existing vote by the same citizen, if any
            _votes.RemoveAll(vote => vote.Citizen == citizen);

            // Add the new or updated vote
            _votes.Add(new Vote(this, citizen, inFavor));

            //Adding Home votes if strategy sets it
            List<Vote>? homeVotes = VotingStrategy?.ReturnHomeVotes(this);
            foreach(Vote vote in homeVotes ?? new List<Vote>())
            {
                _votes.Add(vote);
            }

            // Update the state of the proposal
            Update();
        }
        /// <summary>
        /// This is only for use by PropagatedVoteDictamen. 
        /// For PropagatedVoteDictamen, it is for the case where SubProposals are voting on a ParentProposal via PropagatedVoteDictamen. The SubProposal will create a new vote for the 
        /// Community using the above normal constructor but in case there is a voting strategy that also expects some of the underlying citizens, this below constructor
        /// is added. It may or may not be useful. 
        /// </summary>
        /// <param name="vote">This is the vote being passed by the Dictamen if the ParentProposal is expecting a vote from this SubCitizen</param>
        /// <exception cref="Exception"></exception>
        public void Vote(Vote vote)
        {
            Update();
            if (!Open) throw new Exception("Unable to vote: Proposal is not open for voting.");
            //Find an existing vote by the same citizen, if any
            Vote? existingVote = _votes.FirstOrDefault(v => v.Citizen == vote.Citizen);
            Vote newVote = new(this, vote.Citizen, vote.InFavor, vote.Date);

            if (existingVote != null)
            {
                //if the new vote is newer, replace the existing vote
                if (vote.Date > existingVote.Date)
                {
                    _votes.Remove(existingVote);
                    
                    _votes.Add(newVote);
                }
            }
            else
            {
                _votes.Add(newVote);
            }
            VotingStrategy?.ReturnHomeVotes(this);
            Update();
        }
        public void UpdatePassedStatus()
        {
            if (Passed == null)
            {
                if (TotalValueVotesInFavor > TotalVotingValuesSum / 2)
                {
                    Passed = true;
                    Open = false;
                }
                if (TotalValueVotesAgainst > TotalVotingValuesSum / 2)
                {
                    Passed = false;
                    Open = false;
                }
            }

        }
        /// <summary>
        /// This is doing: 
        /// 1 - Setting Open if Passed == null
        /// 2 - Checking if Passed has changed from null to something else
        /// 3 - issuing dictamen if Passed did change from null to something else
        /// </summary>
        public void Update()
        {
            if (Passed == null) Open = Published && DateTime.UtcNow < ExpirationDate;
            bool? prevPassedValue = Passed;
            UpdatePassedStatus();
            //This runs if it is the first time it passes
            if (prevPassedValue == null && Passed != null)
            {
                IssueDictamen();
                foreach(RedFlag redFlag in AddressesRedFlags)
                {
                    redFlag.ResolveRedFlag();
                }
            }
        }
        public void IssueDictamen()
        {
            if (Dictamen == null) throw new Exception("Proposal.Dictamen is null");
            Dictamen.Issue();
        }
    }
}