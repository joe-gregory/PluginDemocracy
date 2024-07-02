using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public abstract class BaseVotingStrategy
    {
        public int Id { get; set; }
        [NotMapped]
        public abstract string Title { get; }
        [NotMapped]
        public abstract string Description { get; }
        //METHODS:
        public abstract Dictionary<BaseCitizen, double> ReturnVotingWeights(HOACommunity community);
        public abstract List<Vote> ReturnHomeVotes(Proposal proposal);
        public abstract bool ShouldProposalPropagate(Proposal proposal);
    }
}
