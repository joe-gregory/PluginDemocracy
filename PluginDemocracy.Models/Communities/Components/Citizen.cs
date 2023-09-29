namespace PluginDemocracy.Models
{
    /// <summary>
    /// A Citizen is a member of a community. A community can be compromised of users or other sub-communities. For this reason, 
    /// both Community and Users can be Citizens and as such need to implement this class. 
    /// </summary>
    public abstract class Citizen
    {
        public Guid Guid { get; }
        virtual public string? FullName { get; set; }
        virtual public string? Address { get; set; }
        /// <summary>
        /// Communities this Citizen belongs to. 
        /// </summary>
        public List<Community> Citizenships { get; set; }
        public Citizen()
        {
            Guid = new();
            Citizenships = new();
        }
        virtual public void AddCitizenship(Community community)
        {
            if(!Citizenships.Contains(community)) Citizenships.Add(community);
        }
        virtual public void RemoveCitizenship(Community community)
        {
            Citizenships.Remove(community);
        }
    }
}
