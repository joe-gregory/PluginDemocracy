using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class User : BaseCitizen
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        override public string? FullName
        {
            get => string.Join(" ", new string?[] { FirstName, LastName, SecondLastName }
                                   .Where(s => !string.IsNullOrEmpty(s)));
        }
        public string? Initials
        {
            get
            {
                string initials = "";
                if (FirstName != null) initials += FirstName[0];
                if (LastName != null) initials += LastName[0];
                if(SecondLastName != null)initials += SecondLastName[0];
                return initials;
            }
        }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? HashedPassword { get; set; }
        public string? PhoneNumber { get; set; } = null;
        public bool PhoneNumberConfirmed { get; set; } = false;
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
        private string cultureCode = "en-US";
        [NotMapped]
        public CultureInfo Culture { get => new CultureInfo(cultureCode); set => cultureCode = value.Name; }
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
        protected User(bool admin = false)
        {
            Roles = new();
            Admin = admin;
            Culture = new CultureInfo("en-US");
        }
        public User(string firstName, string? lastName, string? email, string? hashedPassword, string? phoneNumber, string? address, DateTime dateOfBirth, CultureInfo culture, string? middleName = null, string? secondLastName = null)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            SecondLastName = secondLastName;
            Email = email;
            HashedPassword = hashedPassword;
            PhoneNumber = phoneNumber;
            Address = address;
            DateOfBirth = dateOfBirth;
            Culture = culture;

            Roles = new();
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
