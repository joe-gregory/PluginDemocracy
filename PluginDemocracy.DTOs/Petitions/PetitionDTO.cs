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
        public CommunityDTO? CommunityDTO { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActionRequested { get; set; }
        public string? SupportingArguments { get; set; }
        public DateTime? DeadlineForResponse { get; set; }
        public List<string> LinksToSupportingDocuments { get; set; } = [];
        public List<IFormFile> SupportingDocumentsToAdd { get; set; } = [];
        public List<UserDTO> Authors { get; set; } = [];
        public List<UserDTO> AuthorsReadyToPublish { get; set; } = [];
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
            if (petition.Community != null) CommunityDTO = CommunityDTO.ReturnSimpleCommunityDtoFromCommunity(petition.Community); 
            Title = petition.Title;
            Description = petition.Description;
            ActionRequested = petition.ActionRequested;
            SupportingArguments = petition.SupportingArguments;
            DeadlineForResponse = petition.DeadlineForResponse;
            LinksToSupportingDocuments = petition.LinksToSupportingDocuments.ToList();
            foreach (User author in petition.Authors) Authors.Add(UserDTO.ReturnSimpleUserDtoFromUser(author));
            foreach (User authorReadyToPublish in petition.AuthorsReadyToPublish) AuthorsReadyToPublish.Add(UserDTO.ReturnSimpleUserDtoFromUser(authorReadyToPublish)); 
        }
        #endregion

    }
}
