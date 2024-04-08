using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class JoinCommunityRequest
    {
        public int Id { get; set; }
        public Community Community { get; set; }
        public Home Home { get; set; }
        public User? User { get; set; }
        public bool JoiningAsOwner { get; set; }
        public bool JoiningAsResident { get; set; }
        public double OwnershipPercentage { get; set; }
        public bool? Approved { get; set; } = null;
        protected JoinCommunityRequest()
        {
            Home = new();
            Community = new();
        }
        public JoinCommunityRequest(Community community, Home home, User user, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            Community = community; 
            Home = home;
            User = user;
            JoiningAsOwner = joiningAsOwner;
            if (!joiningAsOwner) JoiningAsResident = true;
            else JoiningAsResident = false;
            OwnershipPercentage = ownershipPercentage;
        }
    }
}
