using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class JoinCommunityRequestDto
    {
        public int Id { get; set; } = 0;
        public CommunityDto? CommunityDto { get; set; }
        public HomeDto? HomeDto { get; set; } 
        public UserDto? UserDto { get; set; }
        public bool JoiningAsOwner { get; set; } = false;
        public bool JoiningAsResident { get; set; } = false;
        public double OwnershipPercentage { get; set; } = 0;
        public bool? Approved { get; internal set; } = null;
        public JoinCommunityRequestDto() { }
        public JoinCommunityRequestDto(JoinCommunityRequest jcr)
        {
            Id = jcr.Id;
            CommunityDto = CommunityDto.ReturnSimpleCommunityDtoFromCommunity(jcr.Community);
            HomeDto = HomeDto.ReturnHomeDtoFromHome(jcr.Home);
            UserDto = jcr.User != null ? new(jcr.User) : null;
            JoiningAsOwner = jcr.JoiningAsOwner;
            JoiningAsResident = jcr.JoiningAsResident;
            OwnershipPercentage = jcr.OwnershipPercentage;
            Approved = jcr.Approved;
        }
    }
}
