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
        #region PROPERTIES
        public int Id { get; set; }
        public double OwnershipPercentage { get; set; }
        [ForeignKey("HomeId")]
#pragma warning disable IDE0051 // Remove unused private members warning because this field is used but by EFC
        public int HomeId { get; set; }
#pragma warning restore IDE0051 // Remove unused private members
        public virtual Home Home { get; set; }
        //Todo, these should also be private User and Community owners.
        public User? _userOwner;
        public Community? _communityOwner;
        [NotMapped]
        public virtual BaseCitizen Owner 
        { 
            get 
            { 
                if (_userOwner != null) return _userOwner;
                if (_communityOwner != null) return _communityOwner;
                throw new InvalidOperationException("Owner is neither User nor Community");
            } 
        }
        #endregion
        #region METHODS
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
        #endregion
    }
}
