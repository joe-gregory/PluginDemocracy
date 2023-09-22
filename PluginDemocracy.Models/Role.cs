using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Role : BaseDictamen.DictamenIssuer
    {
        public string? Title { get; set; }
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Person who has this role assigned to them
        /// </summary>
        public BaseCitizen? Assignee;
        DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; } 
        public Role(Community community) : base(community)
        {
        }
        public void Update()
        {
            if (ExpirationDate >= DateTime.Now) Active = false;
        }
    }
}
