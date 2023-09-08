using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
