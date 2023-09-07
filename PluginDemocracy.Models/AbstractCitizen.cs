using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// An AbstractCitizen is a member of a community. A community can be compromised of users or other sub-communities. For this reason, 
    /// both Community and User need to implement AbstractCitizen. 
    /// </summary>
    public abstract class AbstractCitizen
    {
        public Guid Guid { get; }
        public List<AbstractCommunity> Communities { get; private set; }
        public AbstractCitizen()
        {
            Guid = Guid.NewGuid();
            Communities = new(); 
        }
    }
}
