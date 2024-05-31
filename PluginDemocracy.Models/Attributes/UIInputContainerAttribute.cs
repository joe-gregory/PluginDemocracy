using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class UIInputContainerAttribute(string titleKey, string descriptionKey) : Attribute
    {
        public string TitleKey { get; } = titleKey;
        public string DescriptionKey { get; } = descriptionKey;
    }
}
