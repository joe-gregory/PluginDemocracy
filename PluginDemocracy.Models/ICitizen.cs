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
    public interface ICitizen
    {
        public Guid Guid { get; }
        /// <summary>
        /// Communities this Citizen belongs to. 
        /// </summary>
        public IReadOnlyList<BaseCommunity> Communities { get; }
        public void AddCommunity(BaseCommunity community);
        public void RemoveCommunity(BaseCommunity community);
    }
}
