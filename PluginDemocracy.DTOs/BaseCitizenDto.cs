namespace PluginDemocracy.DTOs
{
    public abstract class BaseCitizenDto
    {
        #region PROPERTIES
        public int? Id { get; set; }
        abstract public string? FullName { get; }
        virtual public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public abstract List<CommunityDto> Citizenships { get; }
        public List<CommunityDto> NonResidentialCitizenIn { get; set; }
        public List<HomeOwnershipDto> HomeOwnerships { get; set; }
        #endregion
        #region METHODS
        /// <summary>
        /// This represents the parent communities from above of the communities where this citizen has citizenship. 
        /// So for example, if Community B is a member of Community A,and this BaseCitizen is a Citizen of Community B, Community A will show up on this list.
        /// </summary>
        public List<CommunityDto> AssociatedCommunities
        {
            get
            {
                List<CommunityDto> communitiesFromAbove = [];
                foreach (CommunityDto community in Citizenships)
                {
                    foreach (CommunityDto aboveCommunity in community.Citizenships)
                    {
                        communitiesFromAbove.Add(aboveCommunity);
                    }
                }
                //Make sure unique results
                return communitiesFromAbove.Distinct().ToList();
            }
        }
        public BaseCitizenDto()
        {
            HomeOwnerships = [];
            NonResidentialCitizenIn = [];
        }
        #endregion
    }
}
