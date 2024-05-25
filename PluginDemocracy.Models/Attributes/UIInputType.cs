using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Attributes
{
    /// <summary>
    /// Type of input element to use. 
    /// </summary>
    public enum UIInputType
    {
        /// <summary>
        /// Single line text input
        /// </summary>
        TextBox,
        /// <summary>
        /// Multi line text input
        /// </summary>
        TextArea,
        /// <summary>
        /// Integer Int32 input
        /// </summary>
        Integer,
        Date,
        Double,
        Null
    }
}
