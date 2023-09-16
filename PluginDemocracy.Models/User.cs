using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class User : BaseCitizen, IProposalAuthor
    {
        public Guid Guid { get; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondLastName { get; set; }
        public string Name
        {
            get => string.Join(" ", new string?[] { FirstName, LastName, SecondLastName }
                                   .Where(s => !string.IsNullOrEmpty(s)));
        }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Admin { get; set; }
        public bool SuperAdmin { get; set; }
        public IReadOnlyList<BaseCommunity> Communities => _communities.AsReadOnly();
        private List<BaseCommunity> _communities;
        public IReadOnlyDictionary<BaseCommunity, List<Role>> Roles => _roles.AsReadOnly();

        public Proposal? Proposal => throw new NotImplementedException();

        private Dictionary<BaseCommunity, List<Role>> _roles;
        public User(bool admin, bool superAdmin)
        {
            Guid = new();
            Admin = admin;
            SuperAdmin = superAdmin;
            _communities = new();
            _roles = new();
        }
        public void AddCommunity(BaseCommunity community)
        {
            throw new NotImplementedException();
        }
        public void RemoveCommunity(BaseCommunity community)
        {
            throw new NotImplementedException();
        }
        public void AddRole(Role role)
        {
            if (!_roles.ContainsKey(role.Community)) _roles.Add(role.Community, new List<Role> { role });
            else _roles[role.Community].Add(role);
        }
        public void RemoveRole(Role role)
        {
            _roles[role.Community].Remove(role);
        }

        public void CreateProposal()
        {
            throw new NotImplementedException();
        }

        public void RemoveProposal()
        {
            throw new NotImplementedException();
        }
    }
}
