using PluginDemocracy.Models;
using PluginDemocracy.UIComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Mobile
{
    public class MobileAppState : BaseAppState
    {
        //DEL this and implement real HasInternet
        private bool hasInternet;
        public override bool HasInternet { get => hasInternet; protected set => hasInternet = value; }
       
    }
}

