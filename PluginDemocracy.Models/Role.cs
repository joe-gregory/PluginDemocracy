using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Role
    {
        public string? Title { get; set; }
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Person who has this role assigned to them
        /// </summary>
        public User? Assignee;
        public BaseCommunity Community { get; }
        DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; } 
        public Role(BaseCommunity community)
        {
            Community = community;
        }
        public void Update()
        {
            if (ExpirationDate >= DateTime.Now) Active = false;
        }
    }
}
