using Microsoft.AspNetCore.Http;
using PluginDemocracy.Models;

namespace PluginDemocracy.DTOs
{
    public class PetitionDTO
    {
        #region DATA
        public int Id { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public HOACommunityDTO? CommunityDTO { get; set; }
        public int CommunityDTOId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActionRequested { get; set; }
        public string? SupportingArguments { get; set; }
        public DateTime? DeadlineForResponse { get; set; }
        public List<string> LinksToSupportingDocuments { get; set; } = [];
        public List<IFormFile> SupportingDocumentsToAdd { get; set; } = [];
        public List<UserDTO> Authors { get; set; } = [];
        public List<int> AuthorsIds { get; set; } = [];
        public List<UserDTO> AuthorsReadyToPublish { get; set; } = [];
        public List<HOACommunityDTO> CommonCommunitiesBetweenAuthors
        {
            get
            {
                if (Authors.Count == 0) return [];
                if (Authors.Count == 1) return Authors[0].Citizenships;
                IEnumerable<HOACommunityDTO> commonCommunities = Authors[0].Citizenships;
                foreach (UserDTO author in Authors.Skip(1))
                {
                    commonCommunities = commonCommunities.Intersect(author.Citizenships);
                }
                return commonCommunities.ToList();
            }
        }
        #endregion
        #region METHODS
        public PetitionDTO()
        {

        }
        public PetitionDTO(Petition petition)
        {
            Id = petition.Id;
            Published = petition.Published;
            PublishedDate = petition.PublishedDate;
            LastUpdated = petition.LastUpdated;
            if (petition.Community != null) CommunityDTO = HOACommunityDTO.ReturnSimpleCommunityDTOFromCommunity(petition.Community); 
            Title = petition.Title;
            Description = petition.Description;
            ActionRequested = petition.ActionRequested;
            SupportingArguments = petition.SupportingArguments;
            DeadlineForResponse = petition.DeadlineForResponse;
            LinksToSupportingDocuments = petition.LinksToSupportingDocuments.ToList();
            foreach (User author in petition.Authors) Authors.Add(UserDTO.ReturnSimpleUserDTOFromUser(author));
            foreach (User authorReadyToPublish in petition.AuthorsReadyToPublish) AuthorsReadyToPublish.Add(UserDTO.ReturnSimpleUserDTOFromUser(authorReadyToPublish)); 
        }
        #endregion

    }
}
