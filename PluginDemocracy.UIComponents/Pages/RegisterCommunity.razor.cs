using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.UIComponents.Pages
{
    /// <summary>
    /// Don't assign HomeDto.ParentCommunity in CommunityDto.Homes in order to avoid circular references when serializing to JSON in the post method
    /// </summary>
    public partial class RegisterCommunity
    {
    }
}
