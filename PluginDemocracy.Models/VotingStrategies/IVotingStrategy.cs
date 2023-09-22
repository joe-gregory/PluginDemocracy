namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Type AppliesTo { get; }
        public Dictionary<BaseCitizen, int> ReturnCitizensVotingValue(Community community);
    }
}
