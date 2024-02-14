using System.ComponentModel.DataAnnotations.Schema;
namespace PluginDemocracy.Models
{
    /// <summary>
    /// A Citizen is a member of a community. A community can be compromised of users or other sub-communities. For this reason, 
    /// both Community and Users can be Citizens and as such need to implement this class. 
    /// </summary>
    public abstract class BaseCitizen
    {
        public int Id { get; set; }
        abstract public string? FullName { get; }
        virtual public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        /// <summary>
        /// Communities this Citizen belongs to. 
        /// </summary>
        [NotMapped]
        public abstract List<Community> Citizenships { get; }
        public List<Community> NonResidentialCitizenIn { get; set; }
        /// <summary>
        /// Because an organization, not just a User, can be the owner of a home, this is in BaseCitizen
        /// </summary>
        public List<HomeOwnership> HomeOwnerships { get; set; }
        /// <summary>
        /// This represents the parent communities from above of the communities where this citizen has citizenship. 
        /// So for example, if Community B is a member of Community A,and this BaseCitizen is a Citizen of Community B, Community A will show up on this list. 
        /// </summary>
        [NotMapped]
        public List<Community> AssociatedCommunities
        {
            get
            {
                List<Community> communitiesFromAbove = [];
                foreach (Community community in Citizenships)
                {
                    foreach (Community aboveCommunity in community.Citizenships)
                    {
                        communitiesFromAbove.Add(aboveCommunity);
                    }
                }
                //Make sure unique results
                return communitiesFromAbove.Distinct().ToList();
            }
        }
        protected BaseCitizen()
        {
            HomeOwnerships = [];
            NonResidentialCitizenIn = [];
        }
    }
}
