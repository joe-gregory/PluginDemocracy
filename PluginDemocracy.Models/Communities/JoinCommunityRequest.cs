using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class JoinCommunityRequest
    {
        public int HomeId { get; set; }
        public int UserId { get; set; }
        public bool JoiningAsOwner { get; set; }
        public bool JoiningAsResident { get; set; }
        public double OwnershipPercentage { get; set; }
        public bool? Approved { get; set; } = null;
        protected JoinCommunityRequest()
        {

        }
        public JoinCommunityRequest(int homeId, int userId, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            HomeId = homeId;
            UserId = userId;
            JoiningAsOwner = joiningAsOwner;
            if (!joiningAsOwner) JoiningAsResident = true;
            else JoiningAsResident = false;
            OwnershipPercentage = ownershipPercentage;
        }
    }
}
