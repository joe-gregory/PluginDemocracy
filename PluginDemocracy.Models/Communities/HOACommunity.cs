﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// Represents a Home Owners Association community that has homes and someone is a resident of this community if they live in one of the homes.
    /// </summary>
    public class HOACommunity : IAvatar
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
        private readonly List<CultureInfo> _officialLanguages;
        /// <summary>
        /// Get only property. Represents the official languages of the community.
        /// </summary>
        public IReadOnlyList<CultureInfo> OfficialLanguages
        {
            get
            {
                return _officialLanguages.AsReadOnly();
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
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
#       pragma warning disable CS8618
        private HOACommunity() { }
#       pragma warning restore CS8618
        public HOACommunity(string name, string address)
        {
            Name = name;
            Address = address;
            _officialLanguages = [];
            _homes = [];
            _joinCommunityRequests = [];
            _petitions = [];
            _posts = [];
        }
        public void AddOfficialLanguage(CultureInfo culture)
        {
            if (!_officialLanguages.Contains(culture)) _officialLanguages.Add(culture);
        }
        public void RemoveOfficialLanguage(CultureInfo culture)
        {
            _officialLanguages.Remove(culture);
        }
        public void AddHome(Home home)
        {
            if (!_homes.Contains(home))
            {
                _homes.Add(home);
                home.Community = this;
            }
        }
        public void RemoveHome(Home home)
        {
            _homes.Remove(home);
            home.Community = null;
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
        /// show up as a citizen in <see cref="HOACommunity.Citizens"/>. 
        /// The request should be located in <see cref="HOACommunity.JoinCommunityRequests"/> in order to be able to approve it. 
        /// </summary>
        /// <param name="request">The request you want to approve.</param>
        /// <exception cref="ArgumentException">Thrown if request is not found in <see cref="HOACommunity.JoinCommunityRequests"/> or
        /// if <see cref="JoinCommunityRequest.Home"/> does not match a <see cref="Home"/> in <see cref="HOACommunity.Homes"/> or if
        /// <see cref="JoinCommunityRequest.User"/> is null.</exception>
        public void ApproveJoinCommunityRequest(JoinCommunityRequest request)
        {
            if (request.User == null) throw new ArgumentException("Request.User is null");
            if (!JoinCommunityRequests.Contains(request)) throw new ArgumentException("Request not found in Community.JoinCommunityRequests");
            //Make sure the home belongs to this community
            if (!_homes.Contains(request.Home)) throw new ArgumentException("Home does not belong to this community");
            if (request.JoiningAsOwner)
            {
                request.Home.AddOwner(request.User, request.OwnershipPercentage);
            }
            else if (request.JoiningAsResident)
            {
                request.Home.AddResident(request.User);
            }
            else
            {
                throw new ArgumentException("JoiningAsOwner and JoiningAsResident are both false. At least one must be true.");
            }
            request.Approved = true;
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

