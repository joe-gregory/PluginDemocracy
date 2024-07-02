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
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string? ProfilePicture { get; set; }
        /// <summary>
        /// Get only property. Returns the full name of the user.
        /// </summary>
        public string FullName
        {
            get 
            {
                return string.Join(" ", new string?[] {FirstName, MiddleName, LastName, SecondLastName}.Where(s => !string.IsNullOrEmpty(s)));
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
        public string Email { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public bool EmailConfirmed { get; set; }
        public string HashedPassword { get; set; }
        public string? PhoneNumber { get; set; } 
        public bool PhoneNumberConfirmed { get; set; } 
       public string? Address { get; set; }
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
        public CultureInfo Culture { get; set; }
        public bool Admin { get; set; }
        private readonly List<Petition> _petitionDrafts;
        public IReadOnlyList<Petition> PetitionDrafts 
        { 
            get 
            { 
                return _petitionDrafts.AsReadOnly();
            } 
        }
        private readonly List<Community> _citizenships;
        public IReadOnlyList<Community> Citizenships
        {
            get
            {
                return _citizenships.AsReadOnly();
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
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private User(){}
        #pragma warning restore CS8618
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
            _petitionDrafts = [];
            _citizenships = [];
            _notifications = [];
        }
        public void AddPetitionDraft(Petition petition)
        {
            _petitionDrafts.Add(petition);
        }
        public void RemovePetitionDraft(Petition petition)
        {
            _petitionDrafts.Remove(petition);
        }
        public void AddCitizenship(Community community)
        {
            _citizenships.Add(community);
        }
        public void RemoveCitizenship(Community community)
        {
            _citizenships.Remove(community);
        }
        public void AddNotification(string title, string message)
        {
            _notifications.Add(new Notification(title, message));
        }
        public void DeleteNotification(Notification notification)
        {
            _notifications.Remove(notification);
        }
    }
}