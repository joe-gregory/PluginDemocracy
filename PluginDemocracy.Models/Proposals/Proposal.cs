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
        public void Vote(User user, VoteDecision decision)
        {
            if (Status != ProposalStatus.Published) throw new Exception("Cannot vote on a proposal that is not published");
            if (_votes.Any(v => v.Voter == user)) throw new Exception("User has already voted on this proposal");
            _votes.Add(new Vote(user, decision, this));
            UpdateLastUpdated();
            CheckAndUpdateStatus();
        }
        internal void CheckAndUpdateStatus()
        {
            Dictionary<User, double> votingWeights = Community.VotingWeights;

            // Calculate the total voting weight for in-favor and against votes
            double inFavorVotes = _votes
                .Where(v => v.Decision == VoteDecision.InFavor)
                .Sum(v => votingWeights[v.Voter]);

            double againstVotes = _votes
                .Where(v => v.Decision == VoteDecision.Against)
                .Sum(v => votingWeights[v.Voter]);

            // Calculate the majority threshold (total homes / 2 * 100%)
            double majorityThreshold = Community.Homes.Count * 100 / 2;

            // Update the proposal status based on the majority
            if (inFavorVotes > majorityThreshold)
            {
                Status = ProposalStatus.Passed;
                UpdateLastUpdated();
            }
            else if (againstVotes > majorityThreshold)
            {
                Status = ProposalStatus.Rejected;
                UpdateLastUpdated();
            }
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
