using PluginDemocracy.Models.VotingStrategies.Resources;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a voting schema where everybody who is a User type of this Community gets a vote even if it is a member of a sub-community. 
    /// Restrictions can be applied such as age. 
    /// </summary>
    public class UsersVotingStrategy : IVotingStrategy
    {
        public string Title => UsersVotingStrategyResources.Title;
        public string Description
        {
            get
            {
                string description = UsersVotingStrategyResources.Description;
                if (MinimumAge != null) description += $"{UsersVotingStrategyResources.MinimumAge} {MinimumAge.ToString}.";
                if (MaximumAge != null) description += $"{UsersVotingStrategyResources.MaximumAge} {MaximumAge.ToString}.";
                return description;
            }
        }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        /// <summary>
        /// This method needs to check all sub-communities for Users as well.
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community)
        {
            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<Citizen, int> citizensVotingValue = new();
            List<User> allUsers;
            allUsers = community.ReturnAllNestedUsers();

            foreach (var user in allUsers)
            {
                if ((MinimumAge == null || user.Age >= MinimumAge) && (MaximumAge == null || user.Age <= MaximumAge)) citizensVotingValue[user] = 1;  // Each user gets one vote
            }

            return citizensVotingValue;
        }
        public List<Vote> ReturnHomeVotes(Proposal proposal)
        {
            // Intentionally left empty for this strategy, as no home votes need to be added.
            return new List<Vote>();
        }
        public bool ShouldProposalPropagate(Proposal proposal)
        {
            return false;
        }
    }
}
