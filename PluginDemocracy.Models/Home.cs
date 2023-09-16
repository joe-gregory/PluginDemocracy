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
        public List<User> Residents { get; set; }
        /// <summary>
        /// You are a Member of this home if you are either an owner or a resident of Home
        /// </summary>
        override public List<User> Members { 
            get => Owners.Keys.Union(Residents).ToList(); 
            set { throw new InvalidOperationException("Cannot set Members directly in Home class."); } 
        }
        public Home()
        {
            Owners = new();
            Residents = new();
            Members = new();
        }
        public void AddOwnerToHome(User user, double percentage)
        {
            if (user == null || percentage <= 0 || percentage > 100) throw new ArgumentException("Invalid user or percentage");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new InvalidOperationException("Total ownership percentage exceeds 100.");
            Owners[user] = percentage;
            AddResident(user);
        }
        public void RemoveOwnerFromHome(User user)
        {
            Owners.Remove(user);
            RemoveMember(user);
        }
        public void AddResident(User user)
        {
            if (!Residents.Contains(user) && user != null) Residents.Add(user);
        }
        public void RemoveResident(User user)
        {
            if (user != null && Residents.Contains(user)) Residents.Remove(user);
        }
    }
}
