using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Attributes
{
    /// <summary>
    /// This attribute tells the frontend how to render the input element for the property.This way the frontend can be built
    /// programatically given that Dictamens can be of many different types. 
    /// </summary>
    /// <param name="type"><see cref="UIInputType"/></param>
    /// <param name="labelKey">This is the key in the ResourceKeys to know which translation to apply to use as the label of the element</param>
    /// <param name="descriptionKey">This is the key in the ResourceKeys to know which translation to apply for the description</param>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class UIInputAttribute(UIInputType type = UIInputType.Null, string? labelKey = null, string? descriptionKey = null, bool optional = true) : Attribute
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
        /// Is this parameter optional?
        /// </summary>
        public bool Optional { get; } = optional;
        /// <summary>
        /// This is the key to use in the translation resource files to obtain the description on front end.
        /// </summary>
        public string? DescriptionKey { get; } = descriptionKey;
        /// <summary>
        /// For type <see cref="UIInputType.Integer"/>, this is the minimum value that the input can have.
        /// For type <see cref="UIInputType.String"/>, this is the minimum string length."/>
        /// </summary>
        public int? MinValue { get; }
        /// <summary>
        /// For type <see cref="UIInputType.Integer"/>, this is the maximum value that the input can have."/>
        /// For type <see cref="UIInputType.String"/>, this is the maximum string length."/>
        /// </summary>
        public int? MaxValue { get; }
        /// <summary>
        /// For type <see cref="UIInputType.Date"/>, this indicates the date must be in the future.
        /// </summary>
        public bool MustBeFutureDate { get; }

    }
}
