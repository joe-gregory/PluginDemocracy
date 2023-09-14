using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public abstract class DictamenIssuer_old
    {
        public BaseCommunity Community { get; }
        public Dictamen? DictamenToIssue { get; private set; }
        public void CreateDictamen()
        {
            DictamenToIssue = new Dictamen(this);
        }
        public void IssueDictamen()
        {

        }

    }
}
