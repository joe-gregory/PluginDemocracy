namespace PluginDemocracy.Models
{
    public class RedFlag
    {
        public int Id { get; set; }
        public bool Resolved { get; set; }
        public HOACommunity Community { get; private set;}
        /// <summary>
        /// The users that are raising this flag. 
        /// </summary>
        public List<User> Users { get; private set; }
        public string? Description { get; private set; }
        public BaseRedFlaggable? ItemFlagged { get; private set; }
        public DateTime? CreatedOn { get; private set; }
        protected RedFlag() {
            Users = new();
            Community = new();
            
        }
        public RedFlag(HOACommunity community, User user, string description, BaseRedFlaggable itemFlagged)
        {
            Users = new();
            Community = community;
            if(!Community.RedFlags.Contains(this)) Community.RedFlags.Add(this);
            Users.Add(user);
            Description = description;
            ItemFlagged = itemFlagged;
            if(!itemFlagged.RedFlags.Contains(this))itemFlagged.RedFlags.Add(this);
            CreatedOn = DateTime.UtcNow;
        }
        public void JoinRedFlag(User user)
        {
            if(!Users.Contains(user)) Users.Add(user);
        }
        public void RemoveRedFlag(User user)
        {
            if (Users.Contains(user)) Users.Remove(user);
        }
        public void ResolveRedFlag()
        {
            Resolved = true;
            ItemFlagged?.RedFlags.Remove(this);
            Community.RedFlags.Remove(this);
        }
    }
}
