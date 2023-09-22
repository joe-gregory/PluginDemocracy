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
