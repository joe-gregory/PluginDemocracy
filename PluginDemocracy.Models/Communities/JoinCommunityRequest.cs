using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This represents a request to join a community by stating if you are an owner (of some %) or a resident of a home listed in the community's <see cref="ResidentialCommunity.Homes"/>.
    /// </summary>
    public class JoinCommunityRequest
    {
        public int Id { get; init; }
        public DateTime DateRequested { get; init; }
        public ResidentialCommunity Community { get; init; }
        public Home Home { get; init; }
        public User User { get; init; }
        public bool JoiningAsOwner { get; init; }
        public bool JoiningAsResident { get; init; }
        public double OwnershipPercentage { get; init; }
        private readonly List<string> _linksToFiles;
        /// <summary>
        /// Link to files that the user will need to upload in order to 
        /// </summary>
        public IReadOnlyList<string> LinksToFiles 
        {
            get
            {
                return _linksToFiles.AsReadOnly();
            }
        }
        private readonly List<Message> _messages;
        /// <summary>
        /// These are messages that will be displayed in case the admin needs more info or more files in order to verify the request to join the community.
        /// </summary>
        public IReadOnlyList<Message> Messages { 
            get
            { 
                return _messages.AsReadOnly(); 
            } 
        }
        /// <summary>
        /// Null indicates that a decision has not been made. 
        /// True indicates that the request has been approved. 
        /// False indicates that the request has been denied.
        /// </summary>
        public bool? Approved { get; private set; }
        public User? ApprovalDecisionMadeBy { get; private set; }
        public DateTime DateOfApprovalDecision { get; private set; }

        /// <summary>
        /// Parameterless constructor for the benefit of EFC
        /// </summary>
        private JoinCommunityRequest() 
        {
            Community = new(string.Empty, string.Empty);
            Home = new(Community, 0, string.Empty);
            User = new(firstName: string.Empty, lastName: string.Empty, email: string.Empty,phoneNumber: string.Empty,address: string.Empty, dateOfBirth: DateTime.UtcNow, culture: new("en-US"));
            _linksToFiles = [];
            _messages = [];
        }
        public JoinCommunityRequest(ResidentialCommunity community, Home home, User user, bool joiningAsOwner = false, double ownershipPercentage = 0)
        {
            DateRequested = DateTime.UtcNow;
            Community = community; 
            Home = home;
            User = user;
            JoiningAsOwner = joiningAsOwner;
            if (!joiningAsOwner) JoiningAsResident = true;
            else JoiningAsResident = false;
            OwnershipPercentage = ownershipPercentage;
            _linksToFiles = [];
            _messages = [];
        }
        public void AddLinkToFile(string link)
        {
            _linksToFiles.Add(link);
        }
        public void AddMessage(User author, string message)
        {
            _messages.Add(new(author, message));
        }
        /// <summary>
        /// This is internal as it is called by the parent community. 
        /// </summary>
        internal void Approve(User approver)
        {
            Approved = true;
            ApprovalDecisionMadeBy = approver;
            DateOfApprovalDecision = DateTime.UtcNow;
        }
        /// <summary>
        /// This is internal as it is called by the parent community.
        /// </summary>
        internal void Reject(User approver)
        {
            Approved = false;
            ApprovalDecisionMadeBy = approver;
            DateOfApprovalDecision = DateTime.UtcNow;
        }
    }
    
}
