using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class Proposal
    {
        public Guid Id { get; private set; }
        public User Author { get; init; }
        public ProposalStatus Status { get; private set; }
        public DateTime? PublishedDateTime { get; init; }
        public DateTime LastUpdated { get; private set; }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (Status == ProposalStatus.Draft)
                {
                    _title = value;
                    UpdateLastUpdated();
                }
                else throw new Exception("Cannot modify title of a published proposal");
            }
        }
        private string? _content;
        public string? Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (Status == ProposalStatus.Draft)
                {
                    _content = value;
                    UpdateLastUpdated();
                }
                else throw new Exception("Cannot modify content of a published proposal");
            }
        }
        public ResidentialCommunity Community { get; init; }
        private readonly List<Vote> _votes = [];
        [NotMapped]
        public IReadOnlyList<Vote> Votes
        {
            get
            {
                return [.. _votes];
            }
        }
        [NotMapped]
        public IReadOnlyList<Vote> VotesInFavor
        {
            get
            {
                return _votes.Where(v => v.Decision == VoteDecision.InFavor).ToList();
            }
        }
        [NotMapped]
        public IReadOnlyList<Vote> VotesAgainst
        {
            get
            {
                return _votes.Where(v => v.Decision == VoteDecision.Against).ToList();
            }
        }
        [NotMapped]
        public IReadOnlyList<User> UsersThatVotedInFavor
        {
            get
            {
                return VotesInFavor.Select(v => v.Voter).ToList();

            }
        }
        [NotMapped]
        public IReadOnlyList<User> UsersThatVotedAgainst
        {
            get
            {
                return VotesAgainst.Select(v => v.Voter).ToList();
            }
        }
        [NotMapped]
        public Dictionary<User, double> VotingWeights => Community.VotingWeights;
        /// <summary>
        /// A dictionary mapping users who voted in favor of the proposal to their weighted vote values.
        /// Each entry represents a user who voted "In Favor" along with their calculated voting weight
        /// based on ownership percentages within the community.
        /// </summary>
        [NotMapped]
        public Dictionary<User, double> WeightedVotesInFavor
        {
            get
            {
                return VotesInFavor
                    .ToDictionary(
                        vote => vote.Voter,
                        vote => VotingWeights.TryGetValue(vote.Voter, out double value) ? value : 0);
            }
        }
        /// <summary>
        /// A dictionary mapping users who voted against the proposal to their weighted vote values.
        /// Each entry represents a user who voted "Against" along with their calculated voting weight
        /// based on ownership percentages within the community.
        /// </summary>
        [NotMapped]
        public Dictionary<User, double> WeightedVotesAgainst
        {
            get
            {
                return VotesAgainst
                    .ToDictionary(
                       vote => vote.Voter,
                       vote => VotingWeights.TryGetValue(vote.Voter, out double value) ? value : 0);
            }
        }
        /// <summary>
        /// Gets the total voting weight across all users in the community,
        /// representing the sum of all individual ownership percentages.
        /// This value signifies the total number of "weighted votes" in the community
        /// and can be used to calculate majority thresholds or other vote-based decisions.
        /// </summary>
        [NotMapped]
        public double SumOfVotingWeights => Community.SumOfVotingWeights;
        /// <summary>
        /// The sum of all weighted votes in favor of the proposal.
        /// This value represents the cumulative voting power of users who have voted in favor
        /// and is used to determine if the proposal meets the threshold for approval.
        /// </summary>
        [NotMapped]
        public double SumOfWeightedVotesInFavor
        {
            get
            {
                return WeightedVotesInFavor.Values.Sum();
            }
        }
        /// <summary>
        /// The sum of all weighted votes against the proposal.
        /// This value represents the cumulative voting power of users who have voted against the proposal
        /// and is used to determine if the proposal meets the threshold for rejection.
        /// </summary>
        [NotMapped]
        public double SumOfWeightedVotesAgainst
        {
            get
            {
                return WeightedVotesAgainst.Values.Sum();
            }
        }
        /// <summary>
        /// protected constructor for the benefit of EFC
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Proposal() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Proposal(User author, ResidentialCommunity community, string title, string? content = null)
        {
            Author = author;
            _title = title;
            Community = community;
            _content = content;
            LastUpdated = DateTime.UtcNow;
        }
        public void Publish()
        {
            if (string.IsNullOrEmpty(_content)) throw new Exception("Cannot publish a proposal with no content");
            Status = ProposalStatus.Published;
            UpdateLastUpdated();
        }
        public void CheckAndUpdateStatus()
        {
            if (Status != ProposalStatus.Published) return;
            double majorityThreshold = SumOfVotingWeights / 2;
            // Update the proposal status based on the majority
            if (SumOfWeightedVotesInFavor > majorityThreshold) Status = ProposalStatus.Passed;
            else if (SumOfWeightedVotesAgainst > majorityThreshold) Status = ProposalStatus.Rejected;
            UpdateLastUpdated();
        }
        public void Vote(User user, VoteDecision decision)
        {
            if (Status != ProposalStatus.Published) throw new Exception("Cannot vote on a proposal that is not published");
            if (_votes.Any(v => v.Voter == user)) throw new Exception("User has already voted on this proposal");
            _votes.Add(new Vote(user, decision, this));
            UpdateLastUpdated();
            CheckAndUpdateStatus();
        }
        private void UpdateLastUpdated()
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
    public enum ProposalStatus
    {
        Draft,
        Published,
        Passed,
        Rejected
    }
}
