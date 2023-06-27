using System;
/// <summary>
/// Summary description for IDictamen
/// </summary>
/// 
namespace PluginDemocracy.Models
{
    public interface IDictamen
    {
        public Guid Guid { get; }
        public ICommunity Community { get; }
        public Proposal Origin { get; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public IDictamen Dictamen { get; set; }

        public bool Valid { get; set; }
    }
}