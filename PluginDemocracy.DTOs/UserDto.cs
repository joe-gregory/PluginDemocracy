﻿using System.ComponentModel.DataAnnotations;
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
        public List<NotificationDto> Notifications { get; set; } = [];
        [JsonIgnore]
        public bool AnyUnreadNotifications => Notifications.Any(notification => !notification.Read);
        [JsonIgnore]
        public int HowManyUnreadNotifications => Notifications.Count(notification => !notification.Read);
        //Password field is used by create account in frontend
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
        public string _cultureCode { get; set; } = "en-US";
        [JsonIgnore]
        public CultureInfo Culture { get => new(_cultureCode); set => _cultureCode = value.Name; }
        public bool? Admin { get; set; }
        //TODO: List of RolesDto, List of ProposalsDto, Citizenships (List<CommunityDto>), AssociatedCommunities (List<CommunityDto>), ResidentOfHomes (List<HomeDto>)
        #region TODO
        //public List<Role> Roles { get; set; } = [];
        //public List<Proposal> Proposals { get; set; } = [];

        /// <summary>
        /// This should be the union of Homes + Ownerships + NonResidentialCitizenships all distinct and dynamically generated
        /// </summary>
        public override List<CommunityDto> Citizenships
        {
            get
            {
                List<CommunityDto> citizenships = [];
                foreach (HomeOwnershipDto homeOwnershipDto in HomeOwnerships) citizenships.AddRange(homeOwnershipDto.Home?.Citizenships ?? []);
                citizenships.AddRange(NonResidentialCitizenIn);
                citizenships.AddRange(ResidentOfHomes.SelectMany(home => home.Citizenships));
                return citizenships.Distinct().ToList();
            }
        }
        /// <summary>
        /// A list where UserDto shows up on HomeDto.Residents
        /// </summary>
        public List<HomeDto> ResidentOfHomes { get; set; } = [];
        #endregion
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

                //Roles = user.Roles,
                //Proposals = user.Proposals,
            };
            foreach(Notification notification in user.Notifications) userDto.Notifications.Add(new NotificationDto(notification));
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
