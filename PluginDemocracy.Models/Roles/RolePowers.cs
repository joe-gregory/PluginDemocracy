namespace PluginDemocracy.Models
{
    public class RolePowers
    {
        public bool CanJoinCitizen { get; set; }
        public bool CanRemoveCitizen { get; set; }
        public bool CanJoinResident { get; set; }
        public bool CanRemoveResident { get; set; }
        public bool CanCreateRole { get; set; }
        public bool CanVerifyHomeOwnership { get; set; }
        public bool CanAccounting { get; set; }

    }
}
