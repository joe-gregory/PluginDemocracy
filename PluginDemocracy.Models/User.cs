using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class User: Citizen
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Admin { get; set; }
        public IReadOnlyDictionary<Community, List<Role>> Roles => _roles.AsReadOnly();
        private Dictionary<Community, List<Role>> _roles;
        public User(string firstName, string lastName, string secondLastName, string email, string phoneNumber, string address, DateTime dateOfBirth, bool admin)
        {
            FirstName = firstName;
            LastName = lastName;
            SecondLastName = secondLastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            DateOfBirth = dateOfBirth;
            Admin = admin;
            _roles = new();
        }
        public void AddRole(Role role)
        {
            if (!_roles.ContainsKey(role.Community)) _roles.Add(role.Community, new List<Role> { role });
            else _roles[role.Community].Add(role);
        }
        public void RemoveRole(Community community, Role role)
        {
            _roles.[role.Community].Remove(role);
        }
    }
}
