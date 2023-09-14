using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// A Citizen is a member of a community. A community can be compromised of users or other sub-communities. For this reason, 
    /// both Community and Users can be Citizens and as such need to implement this class. 
    /// </summary>
    public abstract class BaseCitizen
    {
        public Guid Guid { get; }
        /// <summary>
        /// Communities this Citizen belongs to. 
        /// </summary>
        public List<BaseCommunity> MemberOfCommunities { get; set; }
        public List<BaseCommunity> CitizenOfCommunities { get; set; }
        public BaseCitizen()
        {
            Guid = new();
            MemberOfCommunities = new();
            CitizenOfCommunities = new();
        }
        public void AddMembership(BaseCommunity community)
        {
            if(!MemberOfCommunities.Contains(community)) MemberOfCommunities.Add(community);
        }
        public void RemoveMembership(BaseCommunity community)
        {
            MemberOfCommunities.Remove(community);
        }
        public void AddCitizenship(BaseCommunity community)
        {
            if(!CitizenOfCommunities.Contains(community)) CitizenOfCommunities.Add(community);
        }
        public void RemoveCitizenship(BaseCommunity community)
        {
            CitizenOfCommunities.Remove(community);
        }
    }
}
