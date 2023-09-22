using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class GatedCommunity : Community
    {
        public List<Home> Homes { get; set; }
        override public List<BaseCitizen> Citizens
        {
            get => Homes.SelectMany(home => home.Citizens)
                        .Distinct()  // Remove duplicates
                        .ToList();  // Convert to List
            set { throw new InvalidOperationException("Cannot set Members directly in GatedCommunity class. Need to add to a Home either as owner or resident."); }
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
       
        public GatedCommunity()
        {
            Homes = new();
        }
        public void AddResidentToHome(Home home, BaseCitizen user)
        {
            home.AddResident(user);
        }
        public void RemoveResidentFromHome(Home home, BaseCitizen user)
        {
            home.RemoveResident(user);
        }
        public void AddOwnerToHome(Home home, BaseCitizen user, int percentage)
        {
            home.AddOwnerToHome(user, percentage);
        }
        public void RemoveOwnerFromHome(Home home, BaseCitizen user)
        {
            home.RemoveOwnerFromHome(user);
        }
    }
}
