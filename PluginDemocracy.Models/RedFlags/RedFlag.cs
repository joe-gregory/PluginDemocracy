namespace PluginDemocracy.Models
{
    public class RedFlag
    {
        
        public bool Resolved { get; set; }
        public Community Community { get; }
        public List<User> Users { get; }
        public string Description { get; }
        public IRedFlaggable ItemFlagged { get; }
        public RedFlag(Community community, User user, string description, IRedFlaggable itemFlagged)
        {
            Users = new();
            Community = community;
            Users.Add(user);
            Description = description;
            ItemFlagged = itemFlagged;
        }
        public void JoinRedFlag(User user)
        {
            if(!Users.Contains(user)) Users.Add(user);
        }
    }
}
