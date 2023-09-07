using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Community
{
    /// <summary>
    /// Only humans (users) can be added as citizens of this community. 
    /// </summary>
    public class PeopleCommunity : AbstractCommunity
    {
        /// <summary>
        /// This represents a community where only humans can be part of the community and everybody has equal voting power. 
        /// </summary>
        /// <param name="communityBasicInfoAndStrategyOptions"></param>
        public PeopleCommunity(CommunityBasicInfoAndStrategyOptions communityBasicInfoAndStrategyOptions) : base(communityBasicInfoAndStrategyOptions) { }
        /// <summary>
        /// This method needs to add the citizen to the community's citizen list as well as add the community to the citizen's communities list..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="citizen">The citizen to join the community</param>
        /// <param name="additionalInfoObject"></param>
        /// <returns></returns>
        public override void JoinCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject = null) where T : class
        {
            //Check that the citizen is not already in the community
            if (citizen.GetType() == typeof(User) && !Citizens.Contains(citizen))
            {
                Citizens.Add(citizen);
                citizen.Communities.Add(this);
            }
        }
        /// <summary>
        /// Removes a citizen from the community's citizen list and removes the community from the citizen's communities list.
        /// </summary>
        /// <param name="citizen">The citizen to remove from the community.</param>
        public override void RemoveCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject) where T : class
        {
            if (Citizens.Contains(citizen))
            {
                Citizens.Remove(citizen);
                citizen.Communities.Remove(this);
            }
        }
    }
}
