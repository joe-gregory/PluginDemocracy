using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// Represents a community where citizenship depends on either being a homeowner or a resident of a home in the community.
    /// The homes are listed in <see cref="Homes"/>. The <see cref="Citizens"/> property is generated depending on the <see cref="Homes"/> property. 
    /// This type of Community can represent a HOA, gated community , or any other type of residential commmunity. 
    /// </summary>
    public class ResidentialCommunity : IAvatar
    {
        #region PROPERTIES
        public int Id { get; init; }
        /// <summary>
        /// Name of community.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Street address of community.
        /// </summary>
        public string Address { get; set; }
        public string? ProfilePicture { get; set; }
        /// <summary>
        /// Get only property. It returns the full name of the community which is Name + Address.
        /// </summary>
        public string FullName => string.Join(" ", Name, Address);
        /// <summary>
        /// Get only property. It returns the initials of the community.
        /// </summary>
        public string Initials
        {
            get
            {
                // Split the name by spaces and take the first letter of each word
                var initials = Name
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word[0])
                    .ToArray();

                return new string(initials);
            }
        }
        public string? Description { get; set; }
        /// <summary>
        /// TODO: This property is missing encapsulation. 
        /// </summary>
        public string OfficialCurrency { get; set; }
        private readonly List<string> _officialLanguagesCodes;
        /// <summary>
        /// Get only property. Represents the official languages of the community.
        /// </summary>
        public IReadOnlyList<CultureInfo> OfficialLanguages
        {
            get
            {
                List<CultureInfo> officialLanguages = [];
                foreach (string language in _officialLanguagesCodes)
                {
                    officialLanguages.Add(new CultureInfo(language));
                }
                return officialLanguages.AsReadOnly();
            }
        }
        private readonly List<Home> _homes;
        /// <summary>
        /// Get only property. Represents all the homes in the community.
        /// </summary>
        public IReadOnlyList<Home> Homes
        {
            get
            {
                return _homes.AsReadOnly();
            }
        }
        /// <summary>
        /// Get only property. Represents all the individuals living in the community regardless of voting ability
        /// </summary>
        public IReadOnlyList<HomeOwnership> HomeOwnerships
        {
            get
            {
                return Homes.SelectMany(home => home.Ownerships).ToList().AsReadOnly();
            }
        }
        public List<User> Citizens
        {
            get
            {
                List<User> homeOwners = Homes?.SelectMany(home => home.OwnersOwnerships.Keys).ToList() ?? [];
                List<User> homeResidents = Homes?.SelectMany(home => home.Residents).ToList() ?? [];
                return homeResidents.Union(homeResidents).Distinct().ToList();
            }
        }
        /// <summary>
        /// Get only property. It returns a list of the Home Owners in the community.
        /// </summary>
        public List<User> HomeOwners
        {
            get
            {
                return Homes?.SelectMany(home => home.OwnersOwnerships.Keys).Distinct().ToList() ?? [];
            }
        }

        private readonly List<JoinCommunityRequest> _joinCommunityRequests;
        /// <summary>
        /// Get only property. Represents all the requests to join the community.
        /// </summary>
        public IReadOnlyList<JoinCommunityRequest> JoinCommunityRequests
        {
            get
            {
                return _joinCommunityRequests.AsReadOnly();
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
        
        private readonly List<Petition> _petitions;
        public IReadOnlyList<Petition> Petitions
        {
            get
            {
                return _petitions.AsReadOnly();
            }
        }
        public IReadOnlyList<Petition> PetitionsByLatestActivity
        {
            get
            {
                return Petitions.OrderByDescending(petition => petition.LastUpdated ?? petition.PublishedDate).ToList().AsReadOnly();
            }
        }
        public IReadOnlyList<Petition> PublishedPetitions
        {
            get
            {
                return Petitions.Where(p => p.Published == false).ToList().AsReadOnly();
            }
        }

        private readonly List<Post> _posts;
        public IReadOnlyList<Post> Posts
        {
            get
            {
                return _posts.AsReadOnly();
            }
        }
        public IReadOnlyList<Post> PostsByLatestActivity => Posts.OrderByDescending(post => post.LatestActivity ?? post.PublishedDate).ToList().AsReadOnly();
        public IReadOnlyList<Post> PostsByPublishedDate => Posts.OrderByDescending(post => post.PublishedDate).ToList().AsReadOnly();
        #endregion
        #region METHODS
        /// <summary>
        /// Parameterless constructor for the benefit of EFC
        /// </summary>
        private ResidentialCommunity() 
        {
            Name = string.Empty;
            Address = string.Empty;
            OfficialCurrency = "USD";
            _officialLanguagesCodes = [];
            _homes = [];
            _joinCommunityRequests = [];
            _roles = [];
            _petitions = [];
            _posts = [];
        }
        public ResidentialCommunity(string name, string address)
        {
            Name = name;
            Address = address;
            OfficialCurrency = "USD";
            _officialLanguagesCodes = [];
            _homes = [];
            _joinCommunityRequests = [];
            _roles = [];
            _petitions = [];
            _posts = [];
        }
        public void AddOfficialLanguage(CultureInfo culture)
        {
            if (!_officialLanguagesCodes.Contains(culture.Name)) _officialLanguagesCodes.Add(culture.Name);
        }
        public void RemoveOfficialLanguage(CultureInfo culture)
        {
            _officialLanguagesCodes.Remove(culture.Name);
        }
        public void AddHome(Home home)
        {
            if (!_homes.Contains(home))
            {
                _homes.Add(home);
                home.ResidentialCommunity = this;
            }
        }
        public void RemoveHome(Home home)
        {
            _homes.Remove(home);
            home.ResidentialCommunity = null;
        }
        public void AddJoinCommunityRequest(JoinCommunityRequest request)
        {
            //Validate:
            //Does the home exist in the current community?
            if (!_homes.Contains(request.Home)) throw new ArgumentException("Home does not exist in this community.");
            //If user is joining as owner, ensure that the ownership percentage is valid
            if (request.JoiningAsOwner)
            {
                if (request.OwnershipPercentage <= 0) throw new ArgumentException("Ownership cannot be less than 0.");
                if (request.OwnershipPercentage > 100) throw new ArgumentException("Ownership cannot be more than 100.");
                if (request.OwnershipPercentage > request.Home.AvailableOwnershipPercentage) throw new ArgumentException("Ownership percentage exceeds available ownership percentage.");
            }
            //At this point, if I didn't throw any exceptions, the request must be good
            _joinCommunityRequests.Add(request);
        }
        public void RemoveJoinCommunityRequest(JoinCommunityRequest request)
        {
           _joinCommunityRequests.Remove(request);
        }
        /// <summary>
        /// Call to approve a JoinCommunityRequest. Approving sets the user as a homeowner or resident of the home which makes him 
        /// show up as a citizen in <see cref="ResidentialCommunity.Citizens"/>. 
        /// The request should be located in <see cref="ResidentialCommunity.JoinCommunityRequests"/> in order to be able to approve it. 
        /// </summary>
        /// <param name="request">The request you want to approve.</param>
        /// <param name="approver">The individual making the approval.</param>
        /// <exception cref="ArgumentException">Thrown if request is not found in <see cref="ResidentialCommunity.JoinCommunityRequests"/> or
        /// if <see cref="JoinCommunityRequest.Home"/> does not match a <see cref="Home"/> in <see cref="ResidentialCommunity.Homes"/> or if
        /// <see cref="JoinCommunityRequest.User"/> is null.</exception>
        public void ApproveJoinCommunityRequest(JoinCommunityRequest request, User approver)
        {
            if (request.User == null) throw new ArgumentException("Request.User is null");
            if (!JoinCommunityRequests.Contains(request)) throw new ArgumentException("Request not found in Community.JoinCommunityRequests");
            //Make sure the home belongs to this community
            if (!_homes.Contains(request.Home)) throw new ArgumentException("Home does not belong to this community");
            //Does the individual making the approval have the right permissions? Either by role or admin
            if (request.JoiningAsOwner)
            {
                //Does the approver have the power to approve the request?
                if (!approver.Roles.Any(r => r.Community == this && r.Powers.CanEditHomeOwnership) && !approver.Admin) throw new ArgumentException("Approver does not have the right permissions.");
                request.Home.AddOwner(request.User, request.OwnershipPercentage);
            }
            else if (request.JoiningAsResident)
            {
                if (!approver.Roles.Any(r => r.Community == this && r.Powers.CanEditResidency) && !approver.Admin) throw new ArgumentException("Approver does not have the right permissions.");
                request.Home.AddResident(request.User);
            }
            else
            {
                throw new ArgumentException("JoiningAsOwner and JoiningAsResident are both false. At least one must be true.");
            }
            request.Approve();
        }
        public void RejectJoinCommunityRequest(JoinCommunityRequest request, User approver)
        {
            if (request.User == null) throw new ArgumentException("Request.User is null");
            if (!JoinCommunityRequests.Contains(request)) throw new ArgumentException("Request not found in Community.JoinCommunityRequests");
            //Make sure the home belongs to this community
            if (!_homes.Contains(request.Home)) throw new ArgumentException("Home does not belong to this community");
            //Does the individual making the approval have the right permissions? Either by role or admin
            if (request.JoiningAsOwner)
            {
                //Does the approver have the power to approve the request?
                if (!approver.Roles.Any(r => r.Community == this && r.Powers.CanEditHomeOwnership) || approver.Admin == false) throw new ArgumentException("Approver does not have the right permissions.");
            }
            else if (request.JoiningAsResident)
            {
                if (!approver.Roles.Any(r => r.Community == this && r.Powers.CanEditResidency) || approver.Admin == false) throw new ArgumentException("Approver does not have the right permissions.");
            }
            else
            {
                throw new ArgumentException("JoiningAsOwner and JoiningAsResident are both false. At least one must be true.");
            }
            request.Reject();
        }
        public Role AddRole(string title, string description, RolePowers? powers = null)
        {
            Role newRole = new(title, description, this, powers);
            _roles.Add(newRole);
            return newRole;
        }
        public void RemoveRole(Role role)
        {
            if (!_roles.Contains(role)) throw new ArgumentException("Role does not exist in this community.");
            role.Holder?.RemoveRole(role);
            _roles.Remove(role);
        }
        public void AssignRole(Role role, User user)
        {
            if (!_roles.Contains(role)) throw new ArgumentException("Role does not exist in this community.");
            role.Holder = user;
            user.AddRole(role);
        }
        public void UnassignRole(Role role, User assignee)
        {
            if (!_roles.Contains(role)) throw new ArgumentException("Role does not exist in this community.");
            assignee.RemoveRole(role);
            role.Holder = null;
        }
        public void AddPetition(Petition petition)
        {
            petition.Community = this;
            _petitions.Add(petition);
        }
        public void AddPost(Post post)
        {
            _posts.Add(post);
        }
        public void RemovePost(Post post)
        {
            _posts.Remove(post);
        }
        #endregion
    }
}

