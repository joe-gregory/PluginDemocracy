using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class GatedCommunity : BaseCommunity
    {
        public List<Home> Homes { get; set; }
        override public List<User> Members
        {
            get => Homes.SelectMany(home => home.Members)
                        .Distinct()  // Remove duplicates
                        .ToList();  // Convert to List
            set { throw new InvalidOperationException("Cannot set Members directly in GatedCommunity class."); }
        }
        public GatedCommunity()
        {
            Homes = new();
        }
        public void AddResidentToHome(Home home, User user)
        {
            home.AddResident(user);
        }
        public void RemoveResidentFromHome(Home home, User user)
        {
            home.RemoveResident(user);
        }
        public void AddOwnerToHome(Home home, User user, double percentage)
        {
            home.AddOwnerToHome(user, percentage);
        }
        public void RemoveOwnerFromHome(Home home, User user)
        {
            home.RemoveOwnerFromHome(user);
        }
    }
}
