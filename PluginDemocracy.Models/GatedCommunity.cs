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
        public GatedCommunity()
        {
            Homes = new();
        }
        public void AddResidentToHome(Home home, User user)
        {
            Home house = GetHome(home);
            house.AddResident(user);
        }
        public void RemoveResidentFromHome(Home home, User user)
        {
            Home house = GetHome(home);
            house.RemoveResident(user);
        }
        public void AddOwnerToHome(Home home, User user, double percentage)
        {
            Home house = GetHome(home);
            house.AddOwnerToHome(user, percentage);
        }
        public void RemoveOwnerFromHome(Home home, User user)
        {
            Home house = GetHome(home);
            house.RemoveOwnerFromHome(user);
        }
        public Home GetHome(Home home)
        {
             return Homes.Find(h => h == home) ?? throw new ArgumentException("Did not find matching home");
        }
    }
}
