using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class UIInputAttribute(UIInputType type = UIInputType.Null, string? labelKey = null, string? descriptionKey = null) : Attribute
    {
        /// <summary>
        /// The type of input to be used in the frontend. Options are of type <see cref="UIInputType"/>."/>
        /// </summary>
        public UIInputType Type { get; } = type;
        /// <summary>
        /// This is the key to use in the frontend translation resource files to obtain the label. 
        /// </summary>
        public string? LabelKey { get; } = labelKey;
        /// <summary>
        /// This is the key to use in the translation resource files to obtain the description on front end.
        /// </summary>
        public string? DescriptionKey { get; } = descriptionKey;

    }
}
