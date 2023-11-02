namespace PluginDemocracy.Models
{
    public class Role
    {
        public string Title { get; set; }
        /// <summary>
        /// Description of responsabilities
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Person who has this role assigned to them
        /// </summary>
        public Community Community { get; }
        public User? Assignee { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool Active { get; set; } 
        public Role(string title, string description, Community community)
        {
            Title = title;
            Description = description;
            Community = community;
            Active = true;
        }
        public void Update()
        {
            bool prevActiveValue = Active;
            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow) Active = false;
            if (Assignee != null && prevActiveValue == true && Active == false) Assignee.RemoveRole(this);
        }
    }
}
