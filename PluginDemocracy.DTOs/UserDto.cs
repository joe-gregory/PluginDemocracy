using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class UserDTO : IAvatar
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        [JsonIgnore]
        public string FullName
        {
            get => string.Join(" ", new string?[] {FirstName, LastName, SecondLastName}.Where(s => !string.IsNullOrEmpty(s)));
        }
        [JsonIgnore]
        public string Initials
        {
            get
            {
                string initials = string.Empty;
                if (!string.IsNullOrEmpty(FirstName)) initials += FirstName[0];
                if (!string.IsNullOrEmpty(LastName)) initials += LastName[0];
                if (!string.IsNullOrEmpty(SecondLastName)) initials += SecondLastName[0];
                return initials;
            }
        }
        public DateTime DateOfBirth { get; set; }
        [JsonIgnore]
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
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool? EmailConfirmed { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public string CultureCode { get; set; } = "en-US";
        [JsonIgnore]
        public CultureInfo Culture 
        {
            get 
            {
                return new(CultureCode);
            }
            set
            {
                CultureCode = value.Name;
            
            }
        }
        public bool Admin { get; set; }
        public List<ResidentialCommunityDTO> Citizenships { get; set; } = [];
        public List<HomeOwnershipDTO> HomeOwnerships { get; set; } = [];
        public List<HomeDTO> ResidentOfHomes { get; set; } = [];
        public List<NotificationDTO> Notifications { get; set; } = [];
        [JsonIgnore]
        public bool AnyUnreadNotifications => Notifications.Any(notification => !notification.Read);
        [JsonIgnore]
        public int HowManyUnreadNotifications => Notifications.Count(notification => !notification.Read);
        public List<PetitionDTO> PetitionDrafts = [];
        public UserDTO() 
        { 
            Culture = CultureInfo.CurrentCulture;
        }
        public UserDTO(User user)
        {
            Id = user.Id;
            ProfilePicture = user.ProfilePicture;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            SecondLastName = user.SecondLastName;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            Address = user.Address;
            DateOfBirth = user.DateOfBirth;
            Culture = user.Culture;
            Admin = user.Admin;
            foreach (Notification notification in user.Notifications) Notifications.Add(new NotificationDTO(notification));
            foreach(ResidentialCommunity community in user.Citizenships) Citizenships.Add(ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(community));
            foreach (HomeOwnership homeOwnership in user.HomeOwnerships) HomeOwnerships.Add(HomeOwnershipDTO.ReturnSimpleHomeOwnershipDTOFromHomeOwnership(homeOwnership));
            foreach (Home home in user.ResidentOfHomes) ResidentOfHomes.Add(new(home));
        }
        [Obsolete("This method can lead to infinite recursions like when loggin in")]
        public static UserDTO ReturnUserDTOFromUser(User user)
        {
            UserDTO userDto = new()
            {
                Id = user.Id,
                ProfilePicture = user.ProfilePicture,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                SecondLastName = user.SecondLastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Culture = user.Culture,
                Admin = user.Admin,

                //Roles = user.Roles,
                //Proposals = user.Proposals,
            };
            foreach(Notification notification in user.Notifications) userDto.Notifications.Add(new NotificationDTO(notification));
            foreach(HomeOwnership homeOwnership in user.HomeOwnerships) userDto.HomeOwnerships.Add(HomeOwnershipDTO.ReturnSimpleHomeOwnershipDTOFromHomeOwnership(homeOwnership));
            foreach(Home home in user.ResidentOfHomes) userDto.ResidentOfHomes.Add(HomeDTO.ReturnHomeDTOFromHome(home));
            return userDto;
        }
        public static UserDTO ReturnSimpleUserDTOFromUser(User user)
        {
            UserDTO userDto = new()
            {
                Id = user.Id,
                ProfilePicture = user.ProfilePicture,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                SecondLastName = user.SecondLastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Culture = user.Culture,
                Admin = user.Admin,
            };
            return userDto;
        }
        /// <summary>
        /// Two userDtos are equal if their Ids are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is UserDTO userDto) return Id == userDto.Id;
            else return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
