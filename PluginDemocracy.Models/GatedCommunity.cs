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
        /// <summary>
        /// There are 2 ways to count votes depending on how partial Home owners are treated.
        /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
        /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
        /// The second alternative is that fractional votes of homes are counted as fractional votes
        /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
        /// other 40% against would count against. 
        /// For now fractional votes will count so a Home can vote "2 ways" but in the future might 
        /// allow different strategy implementations  
        /// </summary>
        override public Dictionary<User, int> MembersVotingValue { 
            get
            {
                Dictionary<User, int> membersVotingValue = new Dictionary<User, int>();
                foreach (Home home in Homes)
                {
                    foreach (var kvp in home.Owners) // Changed to directly get key-value pair
                    {
                        User owner = kvp.Key;
                        int percentage = kvp.Value;
                        if (!membersVotingValue.ContainsKey(owner)) // Changed to use ContainsKey
                        {
                            membersVotingValue[owner] = percentage;
                        }
                        else
                        {
                            membersVotingValue[owner] += percentage; // sum up ownership percentages
                        }
                    }
                }
                return membersVotingValue;
            } 
            set => throw new InvalidOperationException("Cannot set MembersVotingValue directly in GatedCommunity class."); }
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
        public void AddOwnerToHome(Home home, User user, int percentage)
        {
            home.AddOwnerToHome(user, percentage);
        }
        public void RemoveOwnerFromHome(Home home, User user)
        {
            home.RemoveOwnerFromHome(user);
        }
    }
}
