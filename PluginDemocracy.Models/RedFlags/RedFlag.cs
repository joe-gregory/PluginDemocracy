namespace PluginDemocracy.Models
{
    public class RedFlag
    {
        public int Id { get; set; }
        public bool Resolved { get; set; }
        public Community Community { get; }
        /// <summary>
        /// The users that are raising this flag. 
        /// </summary>
        public List<User> Users { get; }
        public string Description { get; }
        public IRedFlaggable ItemFlagged { get; }
        public DateTime CreatedOn { get; }
        public RedFlag(Community community, User user, string description, IRedFlaggable itemFlagged)
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
            ItemFlagged.RedFlags.Remove(this);
            Community.RedFlags.Remove(this);
        }
    }
}
