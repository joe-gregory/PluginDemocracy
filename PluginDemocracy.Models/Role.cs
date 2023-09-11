using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Role
    {
        public string Title;
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string Description;
        public User Who;
        public Community Community;
        DateTime? ExpirationDate;
        public bool Active; 
        public Role(string title, string description, User who, Community community, DateTime expirationDate)
        {
            Title = title;
            Description = description;
            Who = who;
            Community = community;
            ExpirationDate = expirationDate;
        }
        public void Update()
        {
            if (ExpirationDate >= DateTime.Now) Active = false;
        }
    }
}
