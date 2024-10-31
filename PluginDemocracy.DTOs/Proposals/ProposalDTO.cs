using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class ProposalDTO
    {
        public Guid? Id { get; set; }
        public UserDTO? Author { get; set; }
        public ProposalStatus Status { get; set; }
        public DateTime? PublishedDateTime { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; } = string.Empty;
        public ResidentialCommunityDTO? Community { get; set; }
        public ProposalDTO() { }
        public ProposalDTO(Proposal proposal)
        {
            Id = proposal.Id;
            Author = new UserDTO(proposal.Author);
            Status = proposal.Status;
            PublishedDateTime = proposal.PublishedDateTime;
            Title = proposal.Title;
            Content = proposal.Content;
            Community = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(proposal.Community);
        }   
    }
}
