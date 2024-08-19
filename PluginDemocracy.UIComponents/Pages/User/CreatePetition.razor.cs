using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Newtonsoft.Json;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Translations;
using System.Net.Mail;
using System.Text;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class CreatePetition
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IDialogService DialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [SupplyParameterFromQuery]
        public int? PetitionId { get; set; }
        private PetitionDTO petitionDTO = new();
        private readonly IList<IBrowserFile> files = [];
        private readonly List<MemoryStream> memoryStreams = [];
        private readonly int maxFileSize = 100 * 1024 * 1024; // 100MB
        private string? temporaryAddAuthor;
        private bool disableAll = false;
        protected override async void OnInitialized()
        {
            //This is if it is a new petition
            if (PetitionId == null)
            {
                if (AppState?.User != null) petitionDTO.Authors.Add(new UserDTO()
                {
                    Id = AppState.User.Id,
                    FirstName = AppState.User.FirstName,
                    MiddleName = AppState.User.MiddleName,
                    LastName = AppState.User.LastName,
                    SecondLastName = AppState.User.SecondLastName,
                    ProfilePicture = AppState.User.ProfilePicture,
                });
                if (AppState?.User?.Citizenships.Count == 1) petitionDTO.CommunityDTO = new ResidentialCommunityDTO()
                {
                    Id = AppState.User.Citizenships[0].Id,
                    Name = AppState.User.Citizenships[0].Name,
                };
            }
            else
            {
                await RefreshPetition();
            }
        }
        private async Task RefreshPetition()
        {
            if (PetitionId == null) return;
            string endpoint = ApiEndPoints.GetPetitionDraft + $"?petitionId={PetitionId}";
            PetitionDTO? petition = await Services.GetDataGenericAsync<PetitionDTO>(endpoint);
            if (petition != null)
            {
                foreach (UserDTO author in petition.Authors) petitionDTO.AuthorsIds.Add(author.Id);
                petitionDTO = petition;
            }
            else Services.AddSnackBarMessage("error", "Could not load the petition");
            StateHasChanged();
        }
        protected override async Task OnParametersSetAsync()
        {
            if (PetitionId == null)
            {
                petitionDTO = new();
                if (AppState?.User != null) petitionDTO.Authors.Add(new UserDTO()
                {
                    Id = AppState.User.Id,
                    FirstName = AppState.User.FirstName,
                    MiddleName = AppState.User.MiddleName,
                    LastName = AppState.User.LastName,
                    SecondLastName = AppState.User.SecondLastName,
                    ProfilePicture = AppState.User.ProfilePicture
                });
                if (AppState?.User?.Citizenships.Count == 1) petitionDTO.CommunityDTO = new ResidentialCommunityDTO()
                {
                    Id = AppState.User.Citizenships[0].Id,
                    Name = AppState.User.Citizenships[0].Name,
                };
            }
            else
            {
                await RefreshPetition();
            }
        }
        private async void AddSupportingDocumentsToUpload(IReadOnlyList<IBrowserFile> files)
        {
            foreach (IBrowserFile file in files) this.files.Add(file);
            foreach (IBrowserFile file in files)
            {
                MemoryStream memoryStream = new();
                await using (Stream stream = file.OpenReadStream(maxAllowedSize: maxFileSize))
                {
                    await stream.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0;
                memoryStreams.Add(memoryStream);
            }
        }
        private void RemoveSupportingDocumentToBeUploaded(IBrowserFile file)
        {
            int index = files.IndexOf(file);
            if (index >= 0)
            {
                files.RemoveAt(index);
                memoryStreams.RemoveAt(index);
            }
            Services.AddSnackBarMessage("success", "Removed " + file.Name);
        }
        private async void RemoveSupportingDocument(string fileLink)
        {
            MultipartFormDataContent content = new()
            {
                { new StringContent(fileLink), "fileLink" },
                { new StringContent(petitionDTO.Id.ToString()), "petitionId" }
            };
            string endpoint = AppState.ApiBaseUrl + ApiEndPoints.DeleteDocumentFromPetition;
            HttpRequestMessage request = new(HttpMethod.Delete, endpoint);
            if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);
            request.Content = content;
            PDAPIResponse apiResponse = await Services.SendRequestAsync(request);

            if (apiResponse.SuccessfulOperation) await RefreshPetition();
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
            if (temporaryAddAuthor == AppState.User?.Email)
            {
                Services.AddSnackBarMessage("error", "You are already an author of this petition.");
                return;
            }
            string endpoint = ApiEndPoints.GetUserDTOFromEmail + "?email=" + temporaryAddAuthor;
            UserDTO? newAuthor = await Services.GetDataGenericAsync<UserDTO>(endpoint);
            if (newAuthor == null) Services.AddSnackBarMessage("error", "Did not find a user with that email address");
            else
            {
                petitionDTO.Authors.Add(newAuthor);
                petitionDTO.AuthorsIds.Add(newAuthor.Id);
                Services.AddSnackBarMessage("info", "Added " + newAuthor.FullName + " to the petition. Don't forget to save!");
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
                petitionDTO.Authors.Remove(authorToRemove);
                Services.AddSnackBarMessage("info", AppState.Translate(ResourceKeys.SuccessAuthorRemoved));
                StateHasChanged();
            }
        }
        private async Task SavePetitionDraft()
        {
            try
            {
                //if you removed yourself as author, refresh page
                if (!petitionDTO.Authors.Any(a => a.Id == AppState.User?.Id))
                {
                    NavigationManager.NavigateTo(FrontEndPages.CreatePetition, forceLoad: true);
                    Services.AddSnackBarMessage("info", "Petition draft refreshed.");
                }
                if (petitionDTO.CommunityDTO == null)
                {
                    Services.AddSnackBarMessage("warning", "Please select a community for the petition. Petition was not saved.");
                    return;
                }
                //Disable everything during save
                disableAll = true;
                AppState.IsLoading = true;

                MultipartFormDataContent multiPartFormDataContent = [];
                // Add each property of PetitionDTO individually
                multiPartFormDataContent.Add(new StringContent(petitionDTO.Id.ToString()), nameof(petitionDTO.Id));
                multiPartFormDataContent.Add(new StringContent(petitionDTO.Published.ToString()), nameof(petitionDTO.Published));
                if (petitionDTO.PublishedDate.HasValue) multiPartFormDataContent.Add(new StringContent(petitionDTO.PublishedDate.Value.ToString("o")), nameof(petitionDTO.PublishedDate));
                if (petitionDTO.LastUpdated.HasValue) multiPartFormDataContent.Add(new StringContent(petitionDTO.LastUpdated.Value.ToString("o")), nameof(petitionDTO.LastUpdated));
                if (!string.IsNullOrEmpty(petitionDTO.Title)) multiPartFormDataContent.Add(new StringContent(petitionDTO.Title), nameof(petitionDTO.Title));
                if (!string.IsNullOrEmpty(petitionDTO.Description)) multiPartFormDataContent.Add(new StringContent(petitionDTO.Description), nameof(petitionDTO.Description));
                if (!string.IsNullOrEmpty(petitionDTO.ActionRequested)) multiPartFormDataContent.Add(new StringContent(petitionDTO.ActionRequested), nameof(petitionDTO.ActionRequested));
                if (!string.IsNullOrEmpty(petitionDTO.SupportingArguments)) multiPartFormDataContent.Add(new StringContent(petitionDTO.SupportingArguments), nameof(petitionDTO.SupportingArguments));
                if (petitionDTO.DeadlineForResponse.HasValue) multiPartFormDataContent.Add(new StringContent(petitionDTO.DeadlineForResponse.Value.ToString("o")), nameof(petitionDTO.DeadlineForResponse));
                if (petitionDTO.CommunityDTO != null) multiPartFormDataContent.Add(new StringContent(petitionDTO.CommunityDTO.Id.ToString()), nameof(petitionDTO.CommunityDTOId));
                foreach (UserDTO authorDTO in petitionDTO.Authors)
                {
                    if (authorDTO?.Id != null) multiPartFormDataContent.Add(new StringContent(authorDTO.Id.ToString()), nameof(petitionDTO.AuthorsIds));
                }
                // Serialize complex properties like CommunityDTO and Authors
                if (petitionDTO.CommunityDTO != null)
                {
                    multiPartFormDataContent.Add(new StringContent(JsonConvert.SerializeObject(petitionDTO.CommunityDTO)), nameof(petitionDTO.CommunityDTO));
                }

                if (petitionDTO.Authors != null && petitionDTO.Authors.Count > 0)
                {
                    multiPartFormDataContent.Add(new StringContent(JsonConvert.SerializeObject(petitionDTO.Authors), Encoding.UTF8, "application/json"), nameof(petitionDTO.Authors));
                }

                //Add each file
                if (memoryStreams.Count > 0)
                {
                    for (int i = 0; i < memoryStreams.Count; i++)
                    {
                        MemoryStream memoryStream = memoryStreams[i];
                        StreamContent streamContent = new(memoryStream);
                        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(files[i].ContentType);
                        streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = "SupportingDocumentsToAdd",
                            FileName = files[i].Name
                        };
                        multiPartFormDataContent.Add(streamContent);
                    }
                }
                //send the request
                string endpoint = AppState.ApiBaseUrl + ApiEndPoints.SavePetitionDraft;
                HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);

                request.Content = multiPartFormDataContent;
                PDAPIResponse apiResponse = await Services.SendRequestAsync(request);
                //Update the new petition
                if (apiResponse.SuccessfulOperation)
                {
                    files.Clear();
                    memoryStreams.Clear();
                    if (apiResponse.Petition != null)
                    {
                        NavigationManager.NavigateTo($"{FrontEndPages.CreatePetition}?petitionId={apiResponse.Petition.Id}");
                        PetitionId = apiResponse.Petition.Id;
                    }
                    else Services.AddSnackBarMessage("warning", "No petition returned on response.");
                    await RefreshPetition();
                }
                Services.AddSnackBarMessages(apiResponse.Alerts);
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
        private async void PublishPetition()
        {
            if (petitionDTO.AuthorsReadyToPublish.Count == 0) await SavePetitionDraft();
            PDAPIResponse response = await Services.PostDataAsync<string?>($"{ApiEndPoints.AuthorReadyToPublishPetition}?petitionId={petitionDTO.Id}", null);
            if (string.IsNullOrEmpty(response.RedirectTo)) await RefreshPetition();
        }
        private async void Unpublish()
        {
            await Services.PostDataAsync<string?>($"{ApiEndPoints.UnmarkPetitionReadyToPublish}?petitionId={petitionDTO.Id}", null);
            await RefreshPetition();
        }
    }
}
