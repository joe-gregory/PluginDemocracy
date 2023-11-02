namespace PluginDemocracy.Models
{
    public interface IRedFlaggable
    {
        public List<RedFlag> Flags { get; set; }
        public Type Type { get; }
    }
}
