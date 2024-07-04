using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This represents a request to join a community by stating if you are an owner (of some %) or a resident of a home listed in the community's <see cref="ResidentialCommunity.Homes"/>.
    /// </summary>
    public class JoinCommunityRequest
    {
        public int Id { get; init; }
        public ResidentialCommunity Community { get; init; }
        public Home Home { get; init; }
        public User User { get; init; }
        public bool JoiningAsOwner { get; init; }
        public bool JoiningAsResident { get; init; }
        public double OwnershipPercentage { get; init; }
        /// <summary>
        /// Link to files that the user will need to upload in order to 
        /// </summary>
        public List<string> LinksToFiles { get; init; }
        /// <summary>
        /// These are messages that will be displayed in case the admin needs more info or more files in order to verify the request to join the community.
        /// </summary>
        public List<string> Messages { get; init; }
        /// <summary>
        /// Null indicates that a decision has not been made. 
        /// True indicates that the request has been approved. 
        /// False indicates that the request has been denied.
        /// </summary>
        public bool? Approved { get; set; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private JoinCommunityRequest() {}
        #pragma warning restore CS8618
        public JoinCommunityRequest(ResidentialCommunity community, Home home, User user, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            Community = community; 
            Home = home;
            User = user;
            JoiningAsOwner = joiningAsOwner;
            if (!joiningAsOwner) JoiningAsResident = true;
            else JoiningAsResident = false;
            OwnershipPercentage = ownershipPercentage;
            LinksToFiles = [];
            Messages = [];
        }
    }
}
