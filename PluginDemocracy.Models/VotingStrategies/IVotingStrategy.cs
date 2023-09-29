namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Type AppliesTo { get; }
        public Dictionary<Citizen, int> ReturnCitizensVotingValue(Community community);
    }
}
