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
        [JsonIgnore]
        public List<UserDTO> UsersThatVotedInFavor => Votes.Where(v => v.Decision == VoteDecision.InFavor).Select(v => v.Voter).ToList();
        [JsonIgnore]
        public List<UserDTO> UsersThatVotedAgainst => Votes.Where(v => v.Decision == VoteDecision.Against).Select(v => v.Voter).ToList();
        public ProposalDTO() { }
        public ProposalDTO(Proposal proposal)
        {
            Id = proposal.Id;
            Author = new UserDTO(proposal.Author);
            Status = proposal.Status;
            PublishedDateTime = proposal.PublishedDateTime;
            LastUpdated = proposal.LastUpdated;
            Title = proposal.Title;
            Content = proposal.Content;
            Community = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(proposal.Community);
            foreach (Vote vote in proposal.Votes)
            {
                Votes.Add(new VoteDTO(vote));
            }
        }   
    }
}
