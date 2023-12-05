using PluginDemocracy.Models.VotingStrategies.Resources;
using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// NonFractional GatedCommunity Voting Strategy
    /// This voting strategy represents a Gated Community where each Home 1 vote and a home can only vote way one or another.
    /// There are 2 ways to count votes depending on how partial Home owners are treated.
    /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
    /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
    /// The second alternative is that fractional votes of homes are counted as fractional votes
    /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
    /// other 40% against would count against. 
    /// This strategy represents Homes vote as a whole unit because it is the most straighforward implementation given 
    /// that Home is a subclass of Community and how Proposals propagate down. 
    /// allow different strategy implementations  
    /// </summary>
    public class HomeOwnersNonFractionalVotingStrategy : BaseVotingStrategy
    {
        [NotMapped]
        override public string Title => HomeOwnersNonFractionalVotingStrategyResources.Title;
        [NotMapped]
        override public string Description => HomeOwnersNonFractionalVotingStrategyResources.Description;
        override public Dictionary<BaseCitizen, int> ReturnVotingWeights(Community community)
        {
                var homesVotingValue = new Dictionary<BaseCitizen, int>();
                foreach (Home home in community.Homes) homesVotingValue[home] = 1;
                return homesVotingValue;
        }
        override public List<Vote> ReturnHomeVotes(Proposal proposal)
        {
            List<Vote> homeVotes = new();
            //only cycle through homes that haven't casted a vote yet
            List<Home> homesThatHaventVoted = new();
            if (proposal.Community != null) homesThatHaventVoted = proposal.Community.Homes.Where(home => !proposal.Votes.Any(vote => vote.Citizen == home)).ToList();
            foreach (Home home in homesThatHaventVoted)
            {
                // Collect votes from homeowners only
                var homeownerVotes = proposal.Votes.Where(vote => home.Owners.ContainsKey(vote.Citizen));

                // Sum up total value votes in favor
                List<Vote> homeownerVotesInFavor = homeownerVotes.Where(vote => vote.InFavor == true).ToList();
                int totalValueVotesInFavor = 0;
                foreach(Vote vote in homeownerVotesInFavor)
                {
                    totalValueVotesInFavor += home.Owners[vote.Citizen];
                }
                
                //Sum votes against
                List<Vote> homeownerVotesAgainst = homeownerVotes.Where(vote => vote.InFavor == false).ToList();
                int totalValueVotesAgainst = 0;
                foreach(Vote vote in homeownerVotesAgainst)
                {
                    totalValueVotesAgainst += home.Owners[vote.Citizen];
                }
                
                //Add vote accordingly
                if(totalValueVotesInFavor > 50) homeVotes.Add(new Vote(proposal, home, true));
                if (totalValueVotesAgainst > 50) homeVotes.Add(new Vote(proposal, home, false));
            }
            return homeVotes;
        }
        override public bool ShouldProposalPropagate(Proposal proposal)
        {
            return true;
        }
    }
}
