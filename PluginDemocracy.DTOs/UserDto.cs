using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class UserDto : BaseCitizenDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        [JsonIgnore]
        override public string? FullName
        {
            get => string.Join(" ", new string?[] { FirstName, LastName, SecondLastName }
                                   .Where(s => !string.IsNullOrEmpty(s)));
        }
        [JsonIgnore]
        public string? Initials
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
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public bool? EmailConfirmed { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        override public string? Address { get; set; }
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
        private string cultureCode = "en-US";
        [JsonIgnore]
        public CultureInfo Culture { get => new(cultureCode); set => cultureCode = value.Name; }
        public bool? Admin { get; set; }
        //TODO: List of RolesDto and List of ProposalsDto
        public static UserDto ReturnUserDtoFromUser(User user)
        {
            UserDto userDto = new()
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
                //TODO: ADDROLES
                //TODO: Add List<CommunityDto> Citizenships
                //TODO: Add List<CommunityDto> AssociatedCommunities
            };

            return userDto;
        }
        public static User ReturnUserFromUserDto(UserDto userDto)
        {
            User user = new(
                firstName: userDto.FirstName,
                lastName: userDto.LastName,
                email: userDto.Email,
                hashedPassword: string.Empty,
                phoneNumber: userDto.PhoneNumber,
                address: userDto.Address,
                dateOfBirth: userDto.DateOfBirth,
                culture: userDto.Culture,
                middleName: userDto.MiddleName,
                secondLastName: userDto.SecondLastName);

            return user;
        }
    }
}
