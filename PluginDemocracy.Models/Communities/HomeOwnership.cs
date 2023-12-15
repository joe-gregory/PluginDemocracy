using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class HomeOwnership
    {
        public int Id { get; set; }
        public double OwnershipPercentage { get; set; }
        public virtual Home Home { get; set; }
        public virtual BaseCitizen Owner { get; set; }
        /// <summary>
        /// Required by EFC
        /// </summary>
        private HomeOwnership() 
        {
            Home = new();
            Owner = new Community();
        }
        public HomeOwnership(Home home, BaseCitizen owner, double percentage)
        {
            Home = home;
            Owner = owner;
            OwnershipPercentage = percentage;
        }
    }
}
