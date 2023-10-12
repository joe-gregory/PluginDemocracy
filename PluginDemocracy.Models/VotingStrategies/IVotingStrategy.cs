namespace PluginDemocracy.Models
{
    public interface IVotingStrategy
    {
        public string Title { get; }
        public string Description { get; }
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community);
        public List<Vote> ReturnHomeVotes(Proposal proposal);
        public bool ShouldProposalPropagate(Proposal proposal);
    }
}
