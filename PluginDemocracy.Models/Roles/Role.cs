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
        public HOACommunity Community { get; init; }
        /// <summary>
        /// The person that holds the Role
        /// </summary>
        public User? Holder { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; }
        public RolePowers Powers { get; init; }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private Role() { }
        #pragma warning restore CS8618
        public Role(string title, string description, HOACommunity community)
        {
            Title = title;
            Description = description;
            Community = community;
            Active = true;
            Powers = new();
        }
        public void Update()
        {
            bool prevActiveValue = Active;
            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow) Active = false;
            if (Holder != null && prevActiveValue == true && Active == false) Holder.RemoveRole(this);
        }
    }
}
