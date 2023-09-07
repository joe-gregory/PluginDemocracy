using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Communities.Communities_Implementations
{
    /// <summary>
    /// This represents a classic gated community from Tijuana, Mexico. In these communities, only homeowners have voting power. However, a proposal can be made to create another proposal
    /// in which all citizens have equal vote (for example, if something aesthethic is being decided upon). The community then is compromised of internal homes. A citizen is either the resident or owner 
    /// of a home in order to be part of this community. Their voting power depends on whether they are the owner of the home or not. 
    /// </summary>
    public class PrivadaCommunity : AbstractCommunity
    {
        public PrivadaCommunity(CommunityBasicInfoAndStrategyOptions communityBasicInfoAndStrategyOptions) : base(communityBasicInfoAndStrategyOptions){}
        public override void JoinCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject = null) where T : class
        {
            if (citizen.GetType() == typeof(AbstractCommunity) && !Citizens.Contains(citizen))
            {
                Citizens.Add(citizen);
                citizen.Communities.Add(this);
            }
            else
            {
                throw new Exception("Unable to join citizen.");
            }
        }

        public override void RemoveCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject) where T : class
        {
            if (Citizens.Contains(citizen)) Citizens.Remove(citizen);
        }
    }
}
