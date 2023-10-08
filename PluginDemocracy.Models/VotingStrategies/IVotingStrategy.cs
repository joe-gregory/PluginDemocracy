namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community);
        public List<Vote> ReturnHomeVotes(Proposal proposal);

        public bool ShouldProposalPropagate(Proposal proposal);
    }
}
