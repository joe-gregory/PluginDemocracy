using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// Represents a human user of the system. Email needs to be verified. 
    /// </summary>
    public class User : IAvatar
    {
        public int Id { get; init; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? SecondLastName { get; set; }
        /// <summary>
        /// Get only property. Returns the full name of the user.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Join(" ", new string?[] { FirstName, MiddleName, LastName, SecondLastName }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }
        /// <summary>
        /// Get only property. Returns the initials of the user.
        /// </summary>
        public string Initials
        {
            get
            {
                string initials = "";
                if (!string.IsNullOrEmpty(FirstName)) initials += FirstName[0];
                if (!string.IsNullOrEmpty(MiddleName)) initials += MiddleName[0];
                if (!string.IsNullOrEmpty(LastName)) initials += LastName[0];
                if (!string.IsNullOrEmpty(SecondLastName)) initials += SecondLastName[0];
                return initials;
            }
        }
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Get only property. Returns the age of the user using the DateOfBirth property.
        /// </summary>
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string? Address { get; set; }
        public string Email { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public bool EmailConfirmed { get; set; }
        public string HashedPassword { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; } 
        public bool PhoneNumberConfirmed { get; set; }
        public CultureInfo Culture { get; set; }
        public bool Admin { get; set; }
        /// <summary>
        /// Thinking about Citizenships having its own backing field in case you want to
        /// be part of communities without homes. 
        /// </summary>
        public IReadOnlyList<ResidentialCommunity> Citizenships
        {
            get
            {
                //ull-forgiving operator (!) to assert that the ResidentialCommunity
                //will not be null, after ensuring that null values are filtered out.
                List<ResidentialCommunity> communities = _residentOfHomes
                    .Select(home => home.ResidentialCommunity)
                    .Where(community => community != null)
                    .Select(community => community!)
                    .Union(_homeOwnerships.Select(ownership => ownership.Home.ResidentialCommunity)
                    .Where(community => community != null)
                    .Select(community => community!))
                    .Distinct()
                    .ToList();

                return communities.AsReadOnly();
            }
        }
        private readonly List<HomeOwnership> _homeOwnerships;
        public IReadOnlyList<HomeOwnership> HomeOwnerships
        {
            get
            {
                return _homeOwnerships.AsReadOnly();
            }
        }
        private readonly List<Home> _residentOfHomes;
        public IReadOnlyList<Home> ResidentOfHomes
        {
            get
            {
                return _residentOfHomes.AsReadOnly();
            }
        }
        private readonly List<Role> _roles;
        public IReadOnlyList<Role> Roles
        {
            get
            {
                return _roles.AsReadOnly();
            }
        }
        private readonly List<Notification> _notifications;
        public IReadOnlyList<Notification> Notifications
        {
            get
            {
                return _notifications.AsReadOnly();
            }
        }
        public bool AnyUnreadNotifications => Notifications.Any(notification => !notification.Read);
        public int HowManyUnreadNotifications => Notifications.Count(notification => !notification.Read);
        private readonly List<Petition> _petitionDrafts;
        public IReadOnlyList<Petition> PetitionDrafts 
        { 
            get 
            { 
                return _petitionDrafts.AsReadOnly();
            } 
        }
        /// <summary>
        /// Parameterless constructor for the benefit of EF Core
        /// </summary>
        private User()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            HashedPassword = string.Empty;
            Culture = new("en-US");
            _homeOwnerships = [];
            _residentOfHomes = [];
            _roles = [];
            _petitionDrafts = [];
            _notifications = [];
        }
        public User(string firstName, string lastName, string email, string? phoneNumber, string? address, DateTime dateOfBirth, CultureInfo culture, string? middleName = null, string? secondLastName = null)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            SecondLastName = secondLastName;
            Email = email;
            HashedPassword = string.Empty;
            PhoneNumber = phoneNumber;
            Address = address;
            DateOfBirth = dateOfBirth;
            Culture = culture;
            _homeOwnerships = [];
            _residentOfHomes = [];
            _roles = [];
            _petitionDrafts = [];
            _notifications = [];
        }
        internal void AddHomeOwnership(HomeOwnership homeOwnership)
        {
            _homeOwnerships.Add(homeOwnership);
        }
        internal void RemoveHomeOwnership(HomeOwnership homeOwnership)
        {
            _homeOwnerships.Remove(homeOwnership);
        }
        internal void AddAsResidentOfHome(Home home)
        {
            if (!_residentOfHomes.Contains(home)) _residentOfHomes.Add(home);
        }
        internal void RemoveAsResidentOfHome(Home homeToRemove)
        {
            Home? home = _residentOfHomes.FirstOrDefault(h => h.Id == homeToRemove.Id);
            if (home != null) _residentOfHomes.Remove(home);
            else throw new ArgumentException("No home found to remove in User.Residents.");
        }
        internal void AddRole(Role role)
        {
            if (!Roles.Contains(role)) _roles.Add(role);
        }
        internal void RemoveRole(Role role)
        {
            _roles.Remove(role);
        }
        public void AddNotification(string title, string message)
        {
            _notifications.Add(new Notification(title, message));
        }
        public void DeleteNotification(Notification notification)
        {
            _notifications.Remove(notification);
        }
        public void AddPetitionDraft(Petition petition)
        {
            _petitionDrafts.Add(petition);
        }
        public void RemovePetitionDraft(Petition petition)
        {
            Petition? petitionToRemove = _petitionDrafts.FirstOrDefault(p => p.Id == petition.Id);
            if (petitionToRemove != null) _petitionDrafts.Remove(petitionToRemove);
            else throw new ArgumentException("No petition found to remove in User.PetitionDrafts.");
        }
    }
}