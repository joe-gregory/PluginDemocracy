using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public interface IMultilingualDescriptor
    {
        public MultilingualString Title { get; set; }
        public MultilingualString Description { get; set; }
    }
}
