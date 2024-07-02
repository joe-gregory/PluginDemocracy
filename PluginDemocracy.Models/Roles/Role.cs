namespace PluginDemocracy.Models
{
    public class Role : BaseRedFlaggable
    {
        public override int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Person who has this role assigned to them
        /// </summary>
        public HOACommunity Community { get; }
        /// <summary>
        /// The person that holds the Role
        /// </summary>
        public User? Holder { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; }
        public RolePowers Powers { get; }
        //IREDFLAGGABLE
        public override List<RedFlag> RedFlags { get; }
        public override Type Type => typeof(Role);
        //END IREDFLAGGABLE
        protected Role()
        {
            Community = new();
            Powers = new();
            RedFlags = [];
        }
        public Role(string title, string description, HOACommunity community)
        {
            Title = title;
            Description = description;
            Community = community;
            Active = true;
            Powers = new();
            RedFlags = [];
        }
        public void Update()
        {
            bool prevActiveValue = Active;
            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow) Active = false;
            if (Holder != null && prevActiveValue == true && Active == false) Holder.RemoveRole(this);
        }
    }
}
