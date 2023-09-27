namespace PluginDemocracy.Models
{
    /// <summary>
    /// This interface forces implementing classes to have a Multilingual Title and Description. 
    /// </summary>
    public interface IMultilingualDescriptor
    {
        MultilingualString Title { get; set; }
        MultilingualString Description { get; set; }
    }
}
