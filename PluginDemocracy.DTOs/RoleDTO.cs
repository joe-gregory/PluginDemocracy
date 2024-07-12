using PluginDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ResidentialCommunityDTO? Community { get; set; }
        public UserDTO? Holder { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; }
        public RolePowers? Powers { get; set; }
        public RoleDTO() { }
        public RoleDTO(Role role)
        {
            Id = role.Id;
            Title = role.Title;
            Description = role.Description;
            ResidentialCommunityDTO communityDTO = new()
            {
                Id = role.Community.Id,
                Name = role.Community.Name
            };
            Community = communityDTO;
            if (role.Holder != null) Holder = UserDTO.ReturnAvatarMinimumUserDTOFromUser(role.Holder);
            ExpirationDate = role.ExpirationDate;
            Active = role.Active;
            Powers = role.Powers;
        }
    }
}
