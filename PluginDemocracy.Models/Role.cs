using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Role : Dictamen.DictamenIssuer
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
        DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; } 
        public Role(BaseCommunity community) : base(community)
        {
        }
        public void Update()
        {
            if (ExpirationDate >= DateTime.Now) Active = false;
        }
    }
}
