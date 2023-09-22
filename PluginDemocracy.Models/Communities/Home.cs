namespace PluginDemocracy.Models
{
    public class Home : Community
    {
        public Dictionary<BaseCitizen, int> Owners { get; set; }
        public List<BaseCitizen> Residents { get; set; }
        /// <summary>
        /// You are a Member of this home if you are either an owner or a resident of Home
        /// </summary>
        override public List<BaseCitizen> Citizens { 
            get => Owners.Keys.Union(Residents).ToList(); 
            set { throw new InvalidOperationException("Cannot set Members directly in Home class."); } 
        }
        public Home()
        {
            Owners = new();
            Residents = new();
            Citizens = new();
        }
        public void AddOwnerToHome(BaseCitizen user, int percentage)
        {
            if (user == null || percentage <= 0 || percentage > 100) throw new ArgumentException("Invalid user or percentage");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new InvalidOperationException("Total ownership percentage exceeds 100.");
            Owners[user] = percentage;
            AddResident(user);
        }
        public void RemoveOwnerFromHome(BaseCitizen user)
        {
            Owners.Remove(user);
            RemoveCitizen(user);
        }
        public void AddResident(BaseCitizen user)
        {
            if (!Residents.Contains(user) && user != null) Residents.Add(user);
        }
        public void RemoveResident(BaseCitizen user)
        {
            if (user != null && Residents.Contains(user)) Residents.Remove(user);
        }
    }
}
