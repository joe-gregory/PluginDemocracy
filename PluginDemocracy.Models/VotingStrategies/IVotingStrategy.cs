namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community);
        public void AddHomeVotes(Proposal proposal);
    }
}
