using PluginDemocracy.Models.VotingStrategies.Resources;
using System.ComponentModel.DataAnnotations.Schema;
namespace PluginDemocracy.Models
{
    /// <summary>
    /// Fractional GatedCommunity Voting Strategy
    /// This voting strategy represents a Gated Community where each Home has 100.00 votes and different owners can vote in different ways.
    /// There are 2 ways to count votes depending on how partial Home owners are treated.
    /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
    /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
    /// The second alternative is that fractional votes of homes are counted as fractional votes
    /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
    /// other 40% against would count against. 
    /// This strategy represents Homes vote as a whole unit because it is the most straighforward implementation given 
    /// that Home is a subclass of Community and how Proposals propagate down. 
    /// </summary>
    public class HomeOwnersFractionalVotingStrategy : BaseVotingStrategy
    {
        [NotMapped]
        override public string Title => HomeOwnersFractionalVotingStrategyResources.Title;
        [NotMapped]
        override public string Description => HomeOwnersFractionalVotingStrategyResources.Description;
        override public Dictionary<BaseCitizen, double> ReturnVotingWeights(Community community)
        {
            var ownersVotingValue = new Dictionary<BaseCitizen, double>();
            foreach (Home home in community.Homes)
            {
                foreach (BaseCitizen owner in home.Owners.Keys)
                {
                    //an owner may own more than 1 home in the same gated community
                    if (ownersVotingValue.ContainsKey(owner))
                    {
                        ownersVotingValue[owner] += home.Owners[owner];
                    }
                    else
                    {
                        ownersVotingValue[owner] = home.Owners[owner];
                    }
                }
            }
            return ownersVotingValue;
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
