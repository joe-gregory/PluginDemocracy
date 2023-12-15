using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;


namespace PluginDemocracy.API.Models
{
    public class UserDto : BaseCitizenDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; } = string.Empty;
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
                if (SecondLastName != null) initials += SecondLastName[0];
                return initials;
            }
        }
        [System.ComponentModel.DataAnnotations.Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public bool? EmailConfirmed { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        override public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
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
        private string cultureCode = "en-US";
        [JsonIgnore]
        public CultureInfo Culture { get => new CultureInfo(cultureCode); set => cultureCode = value.Name; }
        public bool? Admin { get; set; }
        //TODO: List of RolesDto and List of ProposalsDto
        public static UserDto ReturnUserDtoFromUser(PluginDemocracy.Models.User user)
        {
            UserDto userDto = new() 
            {
                Id = user.Id,
                Guid = user.Guid,
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
                //TODO: ADDROLES
                //TODO: Add List<CommunityDto> Citizenships
                //TODO: Add List<CommunityDto> AssociatedCommunities
            };

            return userDto;
        }
    }
}
