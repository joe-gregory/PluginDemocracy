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
        public ResidentialCommunityDTO? CommunityDTO { get; set; }
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
        public List<ResidentialCommunityDTO> CommonCommunitiesBetweenAuthors
        {
            get
            {
                if (Authors.Count == 0) return [];
                if (Authors.Count == 1) return Authors[0].Citizenships;
                IEnumerable<ResidentialCommunityDTO> commonCommunities = Authors[0].Citizenships;
                foreach (UserDTO author in Authors.Skip(1))
                {
                    commonCommunities = commonCommunities.Intersect(author.Citizenships);
                }
                return commonCommunities.ToList();
            }
        }
        public List<ESignatureDTO> Signatures { get; set; } = [];
        /// <summary>
        /// Key: SignatureId with which Value: HomeId is associated. Assume SignatureIds are unique while
        /// homeIds are repated since multiple signatures can share a house. 
        /// </summary>
        public Dictionary<int,int> SignatureIdWithAssociatedHomeId { get; set; } = [];
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
            if (petition.Community != null) CommunityDTO = ResidentialCommunityDTO.ReturnSimpleCommunityDTOFromCommunity(petition.Community); 
            Title = petition.Title;
            Description = petition.Description;
            ActionRequested = petition.ActionRequested;
            SupportingArguments = petition.SupportingArguments;
            DeadlineForResponse = petition.DeadlineForResponse;
            LinksToSupportingDocuments = [..petition.LinksToSupportingDocuments];
            foreach (User author in petition.Authors) Authors.Add(UserDTO.ReturnSimpleUserDTOFromUser(author));
            foreach (User authorReadyToPublish in petition.AuthorsReadyToPublish) AuthorsReadyToPublish.Add(UserDTO.ReturnSimpleUserDTOFromUser(authorReadyToPublish));
            foreach (ESignature signature in petition.Signatures) 
            {
                ESignatureDTO signDTO = new(signature)
                {
                    PetitionDTO = this
                };
                Signatures.Add(signDTO);
            };
        }
        #endregion

    }
}
