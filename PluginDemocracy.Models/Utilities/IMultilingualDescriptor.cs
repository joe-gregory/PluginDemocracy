namespace PluginDemocracy.Models
{
    /// <summary>
    /// This interface forces implementing classes to have a Multilingual Title and Description. 
    /// </summary>
    public interface IMultilingualDescriptor
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Description { get; set; }
    }
}
