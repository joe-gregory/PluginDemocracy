namespace PluginDemocracy.Models
{
    public abstract class BaseRedFlaggable
    {
        public abstract int Id { get; set; }
        public abstract List<RedFlag> RedFlags { get; }
        public abstract Type Type { get;  }

    }
}
