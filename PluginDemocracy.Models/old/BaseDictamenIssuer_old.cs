using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public abstract class BaseDictamenIssuer_old
    {
        public BaseDictamen_old? Dictamen { get; private set; }
        public void IssueDictamen()
        {
            if (Dictamen.Issuer != this) throw new Exception("Invalid: mismatch between entity trying to issue Dictamen and Dictamen.Issuer");
        }
    }
}
