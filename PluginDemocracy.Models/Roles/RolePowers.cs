namespace PluginDemocracy.Models
{
    public class RolePowers
    {
        public int Id { get; init; }
        /// <summary>
        /// This allows it to accept Join Community Requests where the user is joining as an owner. 
        /// </summary>
        public bool CanEditHomeOwnership { get; set; }
        /// <summary>
        /// This allows it to accept Join Community Requests where the user is joining as a resident. 
        /// </summary>
        public bool CanEditResidency { get; set; }
        public bool CanModifyAccounting { get; set; }
    }
}
