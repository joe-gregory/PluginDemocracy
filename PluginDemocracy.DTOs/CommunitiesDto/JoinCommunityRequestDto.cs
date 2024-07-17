using Microsoft.AspNetCore.Http;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class JoinCommunityRequestDTO
    {
        public int Id { get; set; } = 0;
        public DateTime DateRequested { get; set; }
        public ResidentialCommunityDTO? CommunityDTO { get; set; }
        public HomeDTO? HomeDTO { get; set; } 
        public UserDTO? UserDTO { get; set; }
        public bool JoiningAsOwner { get; set; } = false;
        public bool JoiningAsResident { get; set; } = false;
        public double OwnershipPercentage { get; set; } = 0;
        public List<string> LinksToFiles { get; set; } = [];
        public List<MessageDTO> Messages { get; set; } = [];
        public bool? Approved { get; set; } = null;
        public UserDTO? ApprovalDecisionMadeBy { get; set; }
        public DateTime? DateOfApprovalDecision { get; set; }
        public JoinCommunityRequestDTO() { }
        public JoinCommunityRequestDTO(JoinCommunityRequest jcr)
        {
            Id = jcr.Id;
            DateRequested = jcr.DateRequested;
            CommunityDTO = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(jcr.Community);
            HomeDTO = new(jcr.Home);
            UserDTO = jcr.User != null ? new(jcr.User) : null;
            JoiningAsOwner = jcr.JoiningAsOwner;
            JoiningAsResident = jcr.JoiningAsResident;
            OwnershipPercentage = jcr.OwnershipPercentage;
            Approved = jcr.Approved;
            if (jcr.ApprovalDecisionMadeBy != null) 
            { 
                ApprovalDecisionMadeBy = UserDTO.ReturnAvatarMinimumUserDTOFromUser(jcr.ApprovalDecisionMadeBy);
                DateOfApprovalDecision = jcr.DateOfApprovalDecision;
            } 
            foreach (string link in jcr.LinksToFiles) LinksToFiles.Add(link);
            foreach (Message message in jcr.Messages) 
            {
                UserDTO author = new()
                {
                    Id = message.Sender?.Id ?? 0,
                    FirstName = message.Sender?.FirstName ?? string.Empty,
                    MiddleName = message.Sender?.MiddleName,
                    LastName = message.Sender?.LastName ?? string.Empty,
                    SecondLastName = message.Sender?.SecondLastName,
                    ProfilePicture = message.Sender?.ProfilePicture,
                };
                Messages.Add(new(author, message.Content, message.Date));
            }
        }
    }
    
}
