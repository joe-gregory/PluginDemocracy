namespace PluginDemocracy.Models
{
    public class Home : Community
    {
        public Dictionary<Citizen, int> Owners { get; set; }
        public List<Citizen> Residents { get; set; }
        /// <summary>
        /// You are a Member of this home if you are either an owner or a resident of Home
        /// </summary>
        override public List<Citizen> Citizens { 
            get => Owners.Keys.Union(Residents).ToList(); 
        }
        public Home() : base()
        {
            Owners = new();
            Residents = new();
            VotingStrategy = new HomeVotingStrategy();
        }
        public void AddOwnerToHome(Citizen user, int percentage)
        {
            if (user == null || percentage <= 0 || percentage > 100) throw new ArgumentException("Invalid user or percentage");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new InvalidOperationException("Total ownership percentage exceeds 100.");
            Owners[user] = percentage;
            AddResident(user);
        }
        public void RemoveOwnerFromHome(Citizen user)
        {
            Owners.Remove(user);
            RemoveCitizen(user);
        }
        public void AddResident(Citizen user)
        {
            if (!Residents.Contains(user) && user != null) Residents.Add(user);
        }
        public void RemoveResident(Citizen user)
        {
            if (user != null && Residents.Contains(user)) Residents.Remove(user);
        }
    }
}
