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
    public class Citizen
    {
        public Guid Guid { get; }
        public IReadOnlyList<Community> Communities { get { return _communities.AsReadOnly(); } }
        private List<Community> _communities;
        public Citizen()
        {
            Guid = Guid.NewGuid();
            _communities = new(); 
        }
        public void AddCommunity(Community community)
        {
            _communities.Add(community);
        }
        public void RemoveCommunity(Community community)
        {
            _communities.Remove(community);
        }
        public void Vote(Proposal proposal, bool voteValue)
        {
            proposal.Vote(this, voteValue);
        }
    }
}
