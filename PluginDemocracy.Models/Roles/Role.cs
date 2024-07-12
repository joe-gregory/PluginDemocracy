namespace PluginDemocracy.Models
{
    public class Role
    {
        public int Id { get; init; }
        public string Title { get; internal set; }
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// The community that the role is part of
        /// </summary>
        public ResidentialCommunity Community { get; init; }
        /// <summary>
        /// The person that holds the Role
        /// </summary>
        public User? Holder { get; internal set; }
        public DateTime? ExpirationDate { get; internal set; }
        public bool Active { get; internal set; }
        public RolePowers Powers { get; init; }
        /// <summary>
        /// Private constructor for the benefit of EFC.
        /// </summary>
        private Role() 
        {
            Title = string.Empty;
            Description = string.Empty;
            Community = new(string.Empty, string.Empty);
            Powers = new();
        }
        internal Role(string title, string description, ResidentialCommunity community, RolePowers? powers = null)
        {
            Title = title;
            Description = description;
            Active = true;
            if (powers != null) Powers = powers;
            else Powers = new();
            Community = community;
        }
        public void Update()
        {
            bool prevActiveValue = Active;
            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow) Active = false;
            if (Holder != null && prevActiveValue == true && Active == false) Holder.RemoveRole(this);
        }
    }
}
