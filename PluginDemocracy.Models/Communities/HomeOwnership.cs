using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// Represents a claim of ownership of some % of a home. 
    /// </summary>
    public class HomeOwnership
    {
        public int Id { get; init; }
        public Home Home { get; init; }
        public User Owner { get; init; }
        public double OwnershipPercentage { get; init; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private HomeOwnership() { }
        #pragma warning restore CS8618
        public HomeOwnership(Home home, User owner, double percentage)
        {
            Home = home;
            Owner = owner;
            OwnershipPercentage = percentage;
        }
    }
}
