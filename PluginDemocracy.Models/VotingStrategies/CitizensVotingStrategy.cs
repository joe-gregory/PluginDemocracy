using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This is the simplest form of a voting strategy where each Community.Citizen gets one vote. 
    /// </summary>
    public class CitizensVotingStrategy : BaseVotingStrategy
    {
        [NotMapped]
        override public string Title => CitizensVotingStrategyResources.Title;
        [NotMapped]
        override public string Description => CitizensVotingStrategyResources.Description;
        override public Dictionary<BaseCitizen, double> ReturnVotingWeights(Community community)
        {
            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<BaseCitizen, double> citizensVotingValue = new();
            foreach(BaseCitizen citizen in community.Citizens)
            {
                citizensVotingValue[citizen] = 1;
            }

            return citizensVotingValue;
        }
        override public List<Vote> ReturnHomeVotes(Proposal proposal)
        {
            // Intentionally left empty for this strategy, as no home votes need to be added.
            return new List<Vote>();
        }
        override public bool ShouldProposalPropagate(Proposal proposal)
        {
            return true;
        }
    }
}
