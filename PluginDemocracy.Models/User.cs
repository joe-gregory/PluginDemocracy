using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    public class User : BaseCitizen
    {
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? SecondLastName { get; set; }
        [NotMapped]
        override public string? FullName
        {
            get => string.Join(" ", new string?[] { FirstName, LastName, SecondLastName }
                                   .Where(s => !string.IsNullOrEmpty(s)));
        }
        [NotMapped]
        public string? Initials
        {
            get
            {
                string initials = "";
                if (!string.IsNullOrEmpty(FirstName)) initials += FirstName[0];
                if (!string.IsNullOrEmpty(LastName)) initials += LastName[0];
                if(!string.IsNullOrEmpty(SecondLastName))initials += SecondLastName[0];
                return initials;
            }
        }
        [Required]
        public string Email { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        [Required]
        public string HashedPassword { get; set; }
        public string? PhoneNumber { get; set; } = null;
        public bool PhoneNumberConfirmed { get; set; } = false;
        override public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int Age { 
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            } 
        }
        public string CultureCode { get; private set; } = "en-US";
        [NotMapped]
        public CultureInfo Culture { get => new(CultureCode); set => CultureCode = value.Name; }
        public bool Admin { get; set; }
        public List<Role> Roles { get; set; }
        /// <summary>
        /// All the proposals of communities where this citizen has citizenship and also of parent communities where Proposal.VotingWeights keys contians this User.
        /// </summary>
        [NotMapped]
        public List<Proposal> Proposals
        {
            get
            {
                List<Proposal> allAssociatedProposals = [];
                foreach (Community community in Citizenships) allAssociatedProposals.AddRange(community.Proposals);
                foreach (Community associatedCommunity in AssociatedCommunities)
                {
                    foreach (Proposal proposal in associatedCommunity.Proposals) if (proposal.VotingWeights.ContainsKey(this)) allAssociatedProposals.Add(proposal);
                }
                return allAssociatedProposals.Distinct().ToList();
            }
        }
        /// <summary>
        /// This should be the union of Homes + Ownerships + NonResidentialCitizenships all distinct and dynamically generated
        /// </summary>
        [NotMapped]
        public override List<Community> Citizenships 
        { 
            get 
            {
                List<Community> citizenships = [];
                foreach(HomeOwnership ho in HomeOwnerships) citizenships.AddRange(ho.Home.Citizenships);
                citizenships.AddRange(NonResidentialCitizenIn);
                citizenships.AddRange(ResidentOfHomes.SelectMany(h => h.Citizenships));
                return citizenships.Distinct().ToList();
            } 
        }
        
        /// <summary>
        /// A list where User shows up on Home.Residents
        /// </summary>
        public List<Home> ResidentOfHomes { get; set; }
        protected User(bool admin = false)
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            HashedPassword = string.Empty;
            Culture = new CultureInfo("en-US");
            Admin = admin;
            Roles = [];
            ResidentOfHomes = [];
        }
        public User(string firstName, string lastName, string email, string hashedPassword, string? phoneNumber, string? address, DateTime dateOfBirth, CultureInfo culture, string? middleName = null, string? secondLastName = null)
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

            Roles = [];
            ResidentOfHomes = [];
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
