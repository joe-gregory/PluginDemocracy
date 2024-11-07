using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs.Proposals
{
    public class VoteDTO
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public UserDTO Voter { get; set; } = new();
        public VoteDecision Decision { get; set; }
        public DateTime DateTime { get; set; }
        public VoteDTO() { }
        public VoteDTO(Vote vote)
        {
            Id = vote.Id;
            ProposalId = vote.Proposal.Id;
            Voter = UserDTO.ReturnSimpleUserDTOFromUser(vote.Voter);
            Decision = vote.Decision;
            DateTime = vote.DateTime;
        }
    }
}
