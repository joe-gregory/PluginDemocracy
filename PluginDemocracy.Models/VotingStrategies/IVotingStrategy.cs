namespace PluginDemocracy.Models
{
    public interface IVotingStrategy : IMultilingualDescriptor
    {
        public Dictionary<Citizen, int> ReturnCitizensVotingValue(Community community);
    }
}
