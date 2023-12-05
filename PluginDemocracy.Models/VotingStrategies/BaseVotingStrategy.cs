namespace PluginDemocracy.Models
{
    public abstract class BaseVotingStrategy
    {
        public int Id { get; set; }
        public abstract string Title { get; }
        public abstract string Description { get; }
        public abstract Dictionary<BaseCitizen, int> ReturnVotingWeights(Community community);
        public abstract List<Vote> ReturnHomeVotes(Proposal proposal);
        public abstract bool ShouldProposalPropagate(Proposal proposal);
    }
}
