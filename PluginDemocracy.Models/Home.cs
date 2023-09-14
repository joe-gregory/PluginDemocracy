using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Home : BaseCommunity
    {
        public Dictionary<User, double> Owners { get; set; }
        public Home()
        {
            Owners = new();
            Residents = new();
        }

        public void AddOwnerToHome(User user, double percentage)
        {
            if(user == null || percentage <= 0 || percentage > 100) throw new ArgumentException("Invalid user or percentage");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new InvalidOperationException("Total ownership percentage exceeds 100.");
            Owners[user] = percentage;
            AddResident(user);
        }
       
        public void RemoveOwnerFromHome(User user)
        {
            Owners.Remove(user);
            RemoveResident(user);
        }
    }
}
