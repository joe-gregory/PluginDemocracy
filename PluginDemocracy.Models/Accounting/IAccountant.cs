namespace PluginDemocracy.Models
{
    public interface IAccountant
    {
        public Type Type { get; }
        public string Title { get; }
        public Community Community { get; }
    }
}
