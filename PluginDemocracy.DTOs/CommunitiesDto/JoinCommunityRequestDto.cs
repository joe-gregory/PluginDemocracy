using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class JoinCommunityRequestDTO
    {
        public int Id { get; set; } = 0;
        public ResidentialCommunityDTO? CommunityDto { get; set; }
        public HomeDTO? HomeDto { get; set; } 
        public UserDTO? UserDto { get; set; }
        public bool JoiningAsOwner { get; set; } = false;
        public bool JoiningAsResident { get; set; } = false;
        public double OwnershipPercentage { get; set; } = 0;
        public bool? Approved { get; internal set; } = null;
        public JoinCommunityRequestDTO() { }
        public JoinCommunityRequestDTO(JoinCommunityRequest jcr)
        {
            Id = jcr.Id;
            CommunityDto = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(jcr.Community);
            HomeDto = new(jcr.Home);
            UserDto = jcr.User != null ? new(jcr.User) : null;
            JoiningAsOwner = jcr.JoiningAsOwner;
            JoiningAsResident = jcr.JoiningAsResident;
            OwnershipPercentage = jcr.OwnershipPercentage;
            Approved = jcr.Approved;
        }
    }
}
