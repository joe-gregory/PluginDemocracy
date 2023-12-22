using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class HomeOwnership
    {
        public int Id { get; set; }
        public double OwnershipPercentage { get; set; }
        private int HomeId { get; set; }
        [ForeignKey("HomeId")]
        public virtual Home Home { get; set; }
        [NotMapped]
        public virtual BaseCitizen Owner { 
            get 
            { 
                if (_userOwner != null) return _userOwner;
                if (_communityOwner != null) return _communityOwner;
                throw new InvalidOperationException("Owner is neither User nor Community");
            } 
        }
        private User? _userOwner;
        private Community? _communityOwner;
        /// <summary>
        /// Required by EFC
        /// </summary>
        private HomeOwnership() 
        {
            Home = new();
            _communityOwner = new Community();
        }
        public HomeOwnership(Home home, BaseCitizen owner, double percentage)
        {
            Home = home;
            if(owner is User userOwner)
            {
                _userOwner = userOwner;
            }
            else if (owner is Community communityOwner)
            {
                _communityOwner = communityOwner;
            }
            else
            {
                throw new ArgumentException("Owner must be either User or Community");
            }

            OwnershipPercentage = percentage;
        }
    }
}
