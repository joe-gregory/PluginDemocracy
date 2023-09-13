using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public abstract class BaseDictamenOrigin
    {
        public BaseCommunity Community { get; }
        public BaseDictamen Dictamen { get; private set; }
        public void CreateDictamen()
        {
            Dictamen = new(Community);
        }
        public void IssueDictamen()
        {

        }
    }
}
