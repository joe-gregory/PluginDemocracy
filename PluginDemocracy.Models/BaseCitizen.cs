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
        public Guid Guid { get; }
        virtual public string? FullName { get; }
        virtual public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        /// <summary>
        /// Communities this Citizen belongs to. 
        /// </summary>
        public List<Community> Citizenships { get; set; }

        /// <summary>
        /// This represents the parent communities from above of the communities where this citizen has citizenship.
        /// </summary>
        [NotMapped]
        public List<Community> AssociatedCommunities
        {
            get
            {
                List<Community> communitiesFromAbove = new();
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
        public BaseCitizen()
        {
            Guid = new();
            Citizenships = new();
        }
        virtual public void AddCitizenship(Community community)
        {
            if (!Citizenships.Contains(community)) Citizenships.Add(community);
        }
        virtual public void RemoveCitizenship(Community community)
        {
            Citizenships.Remove(community);
        }
    }
}
