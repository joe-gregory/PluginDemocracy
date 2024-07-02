using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// Represents something that can have "posts" on the frontend and can be represented by an image and so on. 
    /// </summary>
    public interface IAvatar
    {
        public string FullName { get; }
        public string Initials { get; }
        public string? ProfilePicture { get; }
    }
}
