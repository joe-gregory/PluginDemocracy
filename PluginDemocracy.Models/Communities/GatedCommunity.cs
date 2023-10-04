namespace PluginDemocracy.Models
{
    public class GatedCommunity : Community
    {
        public List<Home> Homes { get; set; }
        /// <summary>
        /// This returns a list of homes in the community.
        /// </summary>
        override public List<Citizen> Citizens { get { return Homes.Cast<Citizen>().ToList(); } }
        public GatedCommunity() : base()
        {
            Homes = new();
            VotingStrategy = new GatedCommunityVotingStrategy();
        }
        public void AddResidentToHome(Home home, Citizen user)
        {
            home.AddResident(user);
            AddCitizen(user);
        }
        public void RemoveResidentFromHome(Home home, Citizen user)
        {
            home.RemoveResident(user);
            //if after removing from home, it still shows up, it means resident might still be listed as an owner or something else somewhere, so don't remove
            if (!Citizens.Contains(user)) RemoveCitizen(user);
        }
        public void AddOwnerToHome(Home home, Citizen user, int percentage)
        {
            home.AddOwner(user, percentage);
            AddCitizen(user);
        }
        public void RemoveOwnerFromHome(Home home, Citizen user)
        {
            home.RemoveOwner(user);
            if (!Citizens.Contains(user)) RemoveCitizen(user);
        }

        override private protected void PropagateProposal(Proposal parentProposal)
        {
            foreach(Home home in Homes)
            {
                Proposal propagatedProposal = ReturnPropagatedProposal(home, parentProposal);
                home.PublishProposal(propagatedProposal);
            }
        }
    }
}
