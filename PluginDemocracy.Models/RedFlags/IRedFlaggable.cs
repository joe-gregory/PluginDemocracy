namespace PluginDemocracy.Models
{
    public interface IRedFlaggable
    {
        public List<RedFlag> RedFlags { get; }
        public Type Type { get; }
    }
}
