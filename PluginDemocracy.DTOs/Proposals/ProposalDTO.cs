using PluginDemocracy.DTOs.Proposals;
using PluginDemocracy.Models;
using System.Text.Json.Serialization;

namespace PluginDemocracy.DTOs
{
    public class ProposalDTO
    {
        public Guid? Id { get; set; }
        public UserDTO? Author { get; set; }
        public ProposalStatus Status { get; set; }
        public DateTime? PublishedDateTime { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; } = string.Empty;
        public ResidentialCommunityDTO? Community { get; set; }
        public List<VoteDTO> Votes { get; set; } = [];
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<VoteDTO> VotesInFavor => Votes.Where(v => v.Decision == VoteDecision.InFavor).ToList();
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<VoteDTO> VotesAgainst => Votes.Where(v => v.Decision == VoteDecision.Against).ToList();
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<UserDTO> UsersThatVotedInFavor => Votes.Where(v => v.Decision == VoteDecision.InFavor).Select(v => v.Voter).ToList();
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<UserDTO> UsersThatVotedAgainst => Votes.Where(v => v.Decision == VoteDecision.Against).Select(v => v.Voter).ToList();
        public Dictionary<int, double> SerializableVotingWeights { get; set; } = [];
        public List<UserDTO> SerializableUsersForVotingWeights { get; set; } = [];
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<UserDTO, double> VotingWeights
        {
            get
            {
                // Match each UserDTO with the corresponding voting weight in SerializableVotingWeights
                return SerializableUsersForVotingWeights
                    .Where(user => SerializableVotingWeights.ContainsKey(user.Id)) // Ensure a matching weight exists
                    .ToDictionary(
                        user => user, // Key is the UserDTO object
                        user => SerializableVotingWeights[user.Id] // Value is the voting weight for this user
                    );
            }
        }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<UserDTO, double> WeightedVotesInFavor => VotesInFavor.Where(vote => VotingWeights.ContainsKey(vote.Voter)) // Guard clause
            .ToDictionary(vote => vote.Voter, vote => VotingWeights[vote.Voter]);
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<UserDTO, double> WeightedVotesAgainst => VotesAgainst
            .Where(vote => VotingWeights.ContainsKey(vote.Voter)) // Guard clause
            .ToDictionary(vote => vote.Voter, vote => VotingWeights[vote.Voter]);
        public double SumOfVotingWeights { get; set; }
        public double SumOfWeightedVotesInFavor { get; set; }
        public double SumOfWeightedVotesAgainst { get; set; }
        public ProposalDTO() { }
        public ProposalDTO(Proposal proposal)
        {
            Id = proposal.Id;
            Author = UserDTO.ReturnAvatarMinimumUserDTOFromUser(proposal.Author);
            Status = proposal.Status;
            PublishedDateTime = proposal.PublishedDateTime;
            LastUpdated = proposal.LastUpdated;
            Title = proposal.Title;
            Content = proposal.Content;
            Community = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(proposal.Community);
            foreach (var kvp in proposal.VotingWeights)
            {
                SerializableUsersForVotingWeights.Add(UserDTO.ReturnAvatarMinimumUserDTOFromUser(kvp.Key));
                SerializableVotingWeights.Add(kvp.Key.Id, kvp.Value);
            }
            foreach (Vote vote in proposal.Votes) Votes.Add(new VoteDTO(vote));
            SumOfVotingWeights = proposal.SumOfVotingWeights;
            SumOfWeightedVotesInFavor = proposal.SumOfWeightedVotesInFavor;
            SumOfWeightedVotesAgainst = proposal.SumOfWeightedVotesAgainst;
        }   
    }
}
