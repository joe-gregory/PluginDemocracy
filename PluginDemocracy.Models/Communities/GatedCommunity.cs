namespace PluginDemocracy.Models
{
    public class GatedCommunity : Community
    {
        public List<Home> Homes { get; set; }
        /// <summary>
        /// This returns a list of homes in the community.
        /// </summary>
        override public List<Citizen> Citizens { get { return Homes.Cast<Citizen>().ToList(); } }
        /// <summary>
        /// Since Citizens will return a list of homes. Residents is added to GatedCommunity to return a list of all the individuals living here. 
        /// </summary>
        public List<Citizen> Residents 
            {
            get => Homes.SelectMany(home => home.Citizens)
                        .Distinct()  // Remove duplicates
                        .ToList();  // Conve
            }
        public GatedCommunity() : base()
        {
            Homes = new();
            VotingStrategy = new GatedCommunityVotingStrategy();
        }
        public void AddResidentToHome(Home home, Citizen user)
        {
            home.AddResident(user);
        }
        public void RemoveResidentFromHome(Home home, Citizen user)
        {
            home.RemoveResident(user);
        }
        public void AddOwnerToHome(Home home, Citizen user, int percentage)
        {
            home.AddOwnerToHome(user, percentage);
        }
        public void RemoveOwnerFromHome(Home home, Citizen user)
        {
            home.RemoveOwnerFromHome(user);
        }
    }
}
