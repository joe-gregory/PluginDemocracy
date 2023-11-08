namespace PluginDemocracy.Models
{
    public class User : BaseCitizen
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        override public string? FullName
        {
            get => string.Join(" ", new string?[] { FirstName, LastName, SecondLastName }
                                   .Where(s => !string.IsNullOrEmpty(s)));
        }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        override public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { 
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            } 
        }
        public bool Admin { get; set; }
        
        private List<Role> Roles { get; set; }
        /// <summary>
        /// All the proposals of communities where this citizen has citizenship and also of parent communities where Proposal.VotingWeights keys contians this User.
        /// </summary>
        public List<Proposal> Proposals
        {
            get
            {
                List<Proposal> allAssociatedProposals = new();
                foreach (Community community in Citizenships) allAssociatedProposals.AddRange(community.Proposals);
                foreach (Community associatedCommunity in AssociatedCommunities)
                {
                    foreach (Proposal proposal in associatedCommunity.Proposals) if (proposal.VotingWeights.ContainsKey(this)) allAssociatedProposals.Add(proposal);
                }
                return allAssociatedProposals.Distinct().ToList();
            }
        }
        public User()
        {
            Roles = new();
            Admin = false;
        }
        public User(bool admin) : this()
        {
            Admin = admin;
        }
        public void AddRole(Role role)
        {
            if (role != null && !Roles.Contains(role)) Roles.Add(role);
        }
        public void RemoveRole(Role role)
        {
            Roles.Remove(role);
        }
    }
}
