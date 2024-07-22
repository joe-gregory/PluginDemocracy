namespace PluginDemocracy.Models
{
    public struct RolePowers
    {
        /// <summary>
        /// This allows it to accept Join Community Requests where the user is joining as an owner. 
        /// </summary>
        public bool CanEditHomeOwnership { get; set; }
        /// <summary>
        /// This allows it to accept Join Community Requests where the user is joining as a resident. 
        /// </summary>
        public bool CanEditResidency { get; set; }
        /// <summary>
        /// Use by community's treasurer or accountant to make changes to the community's accounting. Changes are still recorded
        /// to prevent fraud. 
        /// </summary>
        public bool CanModifyAccounting { get; set; }
    }
}
