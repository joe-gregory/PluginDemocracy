using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Type AppliesTo { get; }
        public Dictionary<BaseCitizen, int> ReturnCitizensVotingValue(Community community);
    }
}
