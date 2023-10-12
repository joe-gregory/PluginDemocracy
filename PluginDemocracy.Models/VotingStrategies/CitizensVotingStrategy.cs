namespace PluginDemocracy.Models
{
    /// <summary>
    /// This is the simplest form of a voting strategy where each Community.Citizen gets one vote. 
    /// </summary>
    public class CitizensVotingStrategy : IVotingStrategy
    {
        public string Title => CitizensVotingStrategyResources.Title;
        public string Description => CitizensVotingStrategyResources.Description;
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community)
        {
            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<Citizen, int> citizensVotingValue = new();
            foreach(Citizen citizen in community.Citizens)
            {
                citizensVotingValue[citizen] = 1;
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
            return true;
        }
    }
}
