using Microsoft.AspNetCore.Components.Forms;
using PluginDemocracy.DTOs;
using System.Net.Mail;
using PluginDemocracy.API.UrlRegistry;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PluginDemocracy.Translations;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class CreatePetition
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IDialogService DialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PetitionDTO PetitionDTO = new();
        private readonly IList<IBrowserFile> files = [];
        private string? temporaryAddAuthor;
        private bool disableAll = false;
        protected override void OnInitialized()
        {
            if (AppState?.User != null) PetitionDTO.Authors.Add(new UserDTO()
            {
                Id = AppState.User.Id,
                FirstName = AppState.User.FirstName,
                MiddleName = AppState.User.MiddleName,
                LastName = AppState.User.LastName,
                SecondLastName = AppState.User.SecondLastName,
            });
            if (AppState?.User?.Citizenships.Count == 1) PetitionDTO.CommunityDTO = new CommunityDTO()
            {
                Id = AppState.User.Citizenships[0].Id,
                Name = AppState.User.Citizenships[0].Name,
            };
        }
        private void AddSupportingDocumentsToUpload(IReadOnlyList<IBrowserFile> files)
        {
            foreach (IBrowserFile file in files)
            {
                this.files.Add(file);
            }
        }
        private void RemoveSupportingDocumentToBeUploaded(IBrowserFile file)
        {
            files.Remove(file);
            Services.AddSnackBarMessage("success", "Removed " + file.Name);
        }
        private void RemoveSupportingDocument(string fileLink)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This hits API endpoing <see cref="ApiEndPoints.GetUserDtoFromEmail"/> to get the UserDTO
        /// If one is found with the given email, it is added to the Petition's Authors list.
        /// The petition still needs to be saved for the author to be added to the petition.
        /// </summary>
        private async void AddAuthor()
        {
            //Check that the string is a valid email format
            if (temporaryAddAuthor == null)
            {
                Services.AddSnackBarMessage("error", "Please enter an email address.");
                return;
            }
            try
            {
                var mailAddress = new MailAddress(temporaryAddAuthor);
            }
            catch (FormatException)
            {
                Services.AddSnackBarMessage("error", temporaryAddAuthor + " is not a valid email address.");
                return;
            }
            //Get the UserDTO for the given email in the API request if email address is valid
            string endpoint = ApiEndPoints.GetUserDTOFromEmail + "?email=" + temporaryAddAuthor;
            UserDTO? newAuthor = await Services.GetDataGenericAsync<UserDTO>(endpoint);
            if (newAuthor == null) Services.AddSnackBarMessage("error", "Did not find a user with that email address");
            else
            {
                PetitionDTO.Authors.Add(newAuthor);
                Services.AddSnackBarMessage("success", "Added " + newAuthor.FullName + " to the petition. Don't forget to save!");
                //If successful Clear the temporary placeholder for the author to add 
                temporaryAddAuthor = null;
            }
            StateHasChanged();
        }
        private async void RemoveYourselfAsAuthor(UserDTO authorToRemove)
        {
            //show modal window confirming that you want to remove yourself as an author
            bool? result = await DialogService.ShowMessageBox(
                AppState.Translate(ResourceKeys.Warning),
                AppState.Translate(ResourceKeys.WarningRemoveYourselfAsAuthor),
                yesText: AppState.Translate(ResourceKeys.Remove)
                );
            if (result == true)
            {
                PetitionDTO.Authors.Remove(authorToRemove);
                Services.AddSnackBarMessage("success", AppState.Translate(ResourceKeys.SuccessAuthorRemoved));
            }
        }
        private async Task SavePetitionDraft()
        {
            try
            {
                //Disable everything during save
                disableAll = true;
                AppState.IsLoading = true;

                MultipartFormDataContent formData = [];
                // Add each property of PetitionDTO individually
                formData.Add(new StringContent(PetitionDTO.Id.ToString()), nameof(PetitionDTO.Id));
                formData.Add(new StringContent(PetitionDTO.Published.ToString()), nameof(PetitionDTO.Published));
                if (PetitionDTO.PublishedDate.HasValue)
                    formData.Add(new StringContent(PetitionDTO.PublishedDate.Value.ToString("o")), nameof(PetitionDTO.PublishedDate));
                if (PetitionDTO.LastUpdated.HasValue)
                    formData.Add(new StringContent(PetitionDTO.LastUpdated.Value.ToString("o")), nameof(PetitionDTO.LastUpdated));
                if (!string.IsNullOrEmpty(PetitionDTO.Title))
                    formData.Add(new StringContent(PetitionDTO.Title), nameof(PetitionDTO.Title));
                if (!string.IsNullOrEmpty(PetitionDTO.Description))
                    formData.Add(new StringContent(PetitionDTO.Description), nameof(PetitionDTO.Description));
                if (!string.IsNullOrEmpty(PetitionDTO.ActionRequested))
                    formData.Add(new StringContent(PetitionDTO.ActionRequested), nameof(PetitionDTO.ActionRequested));
                if (!string.IsNullOrEmpty(PetitionDTO.SupportingArguments))
                    formData.Add(new StringContent(PetitionDTO.SupportingArguments), nameof(PetitionDTO.SupportingArguments));
                if (PetitionDTO.DeadlineForResponse.HasValue)
                    formData.Add(new StringContent(PetitionDTO.DeadlineForResponse.Value.ToString("o")), nameof(PetitionDTO.DeadlineForResponse));

                // Serialize complex properties like CommunityDTO and Authors
                if (PetitionDTO.CommunityDTO != null)
                {
                    var communityJson = JsonSerializer.Serialize(PetitionDTO.CommunityDTO, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                    formData.Add(new StringContent(communityJson, Encoding.UTF8, "application/json"), nameof(PetitionDTO.CommunityDTO));
                }

                if (PetitionDTO.Authors != null && PetitionDTO.Authors.Count > 0)
                {
                    var authorsJson = JsonSerializer.Serialize(PetitionDTO.Authors, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
                    formData.Add(new StringContent(authorsJson, Encoding.UTF8, "application/json"), nameof(PetitionDTO.Authors));
                }
                //Add each file
                int maxAllowedSize = 100 * 1024 * 1024; //100MB
                foreach (IBrowserFile file in files)
                {
                    StreamContent streamContent = new(file.OpenReadStream(maxAllowedSize: maxAllowedSize));
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "SupportingDocumentsToAdd",
                        FileName = file.Name
                    };
                    formData.Add(streamContent);
                }
                //send the request
                string endpoint = AppState.BaseUrl + ApiEndPoints.SavePetitionDraft;
                HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);

                request.Content = formData;
                //HttpResponseMessage response = await Services._httpClient.SendAsync(request);

                //PDAPIResponse apiResponse = await Services.CommunicationCommon(response);
                PDAPIResponse apiResponse = await Services.SendRequestAsync(request);
                //Update the new petition
                if (apiResponse.SuccessfulOperation)
                {
                    if (apiResponse.Petition != null) PetitionDTO = apiResponse.Petition;
                    files.Clear();
                }
            }
            catch (Exception ex)
            {
                Services.AddSnackBarMessage("error", ex.Message);
            }
            finally
            {
                AppState.IsLoading = false;
                disableAll = false;
            }
        }
        private void DeleteDraft()
        {
            throw new NotImplementedException();
        }
        private void PublishPetition()
        {
            //modal window of are you sure to publish? No more changes! 
            //If there are more authors, show which ones already published
            throw new NotImplementedException();
        }
    }
}
