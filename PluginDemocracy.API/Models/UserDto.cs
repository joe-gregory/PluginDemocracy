using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace PluginDemocracy.API.Models
{
    public class UserDto : BaseCitizenDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
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
                if (SecondLastName != null) initials += SecondLastName[0];
                return initials;
            }
        }
        [System.ComponentModel.DataAnnotations.Required]
        [EmailAddress]
        public string? Email { get; set; }
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
        public CultureInfo Culture { get => new CultureInfo(cultureCode); set => cultureCode = value.Name; }
        public bool? Admin { get; set; }
        //TODO: List of RolesDto and List of ProposalsDto
    }
}
