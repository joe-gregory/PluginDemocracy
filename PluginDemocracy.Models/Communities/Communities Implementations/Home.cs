using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Communities.Communities_Implementations
{
    public class Home : AbstractCommunity
    {
        public Home(CommunityBasicInfoAndStrategyOptions communityBasicInfoAndStrategyOptions) : base(communityBasicInfoAndStrategyOptions) { }
        public override void JoinCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject = null) where T : class
        {
            throw new NotImplementedException();
        }

        public override void RemoveCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
