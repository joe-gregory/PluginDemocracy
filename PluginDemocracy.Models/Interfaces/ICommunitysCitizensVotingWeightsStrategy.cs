using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Interfaces
{
    /// <summary>
    /// This interface represents the types of contracts different communities can have. For example, some communities might have each member have a single vote or
    /// in a gated community, for example, you get a vote for each home you own. Having an interface allows to have different rules for different communities this way 
    /// there are only a few Community classes and the different types of communities can be encapsulated in different strategies. Perhaps even users can define their own 
    /// via de UI in the future this way was well. 
    /// </summary>
    public interface ICommunitysCitizensVotingWeightsStrategy
    {
        public IReadOnlyDictionary<BaseCitizen, decimal> ReturnCitizensVotingWeights(BaseCommunity abstractCommunity);
    }
}
