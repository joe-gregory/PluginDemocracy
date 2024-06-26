﻿using Microsoft.AspNetCore.Components.Forms;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using static MudBlazor.CategoryTypes;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class CreatePetition
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IDialogService DialogService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [SupplyParameterFromQuery]
        public int? petitionId { get; set; }
        private PetitionDTO petitionDTO = new();
        private readonly IList<IBrowserFile> files = [];
        private string? temporaryAddAuthor;
        private bool disableAll = false;
        protected override async void OnInitialized()
        {
            //This is if it is a new petition
            if (petitionId == null)
            {
                if (AppState?.User != null) petitionDTO.Authors.Add(new UserDTO()
                {
                    Id = AppState.User.Id,
                    FirstName = AppState.User.FirstName,
                    MiddleName = AppState.User.MiddleName,
                    LastName = AppState.User.LastName,
                    SecondLastName = AppState.User.SecondLastName,
                });
                if (AppState?.User?.Citizenships.Count == 1) petitionDTO.CommunityDTO = new CommunityDTO()
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
            if (petitionId == null) return;
            string endpoint = ApiEndPoints.GetPetitionDraft + $"?petitionId={petitionId}";
            PetitionDTO? petition = await Services.GetDataGenericAsync<PetitionDTO>(endpoint);
            if (petition != null) petitionDTO = petition;
            else Services.AddSnackBarMessage("error", "Could not load the petition");
            StateHasChanged();
        }
        protected override async Task OnParametersSetAsync()
        {
            if (petitionId == null)
            {
                petitionDTO = new();
                if (AppState?.User != null) petitionDTO.Authors.Add(new UserDTO()
                {
                    Id = AppState.User.Id,
                    FirstName = AppState.User.FirstName,
                    MiddleName = AppState.User.MiddleName,
                    LastName = AppState.User.LastName,
                    SecondLastName = AppState.User.SecondLastName,
                });
                if (AppState?.User?.Citizenships.Count == 1) petitionDTO.CommunityDTO = new CommunityDTO()
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
        private async void RemoveSupportingDocument(string fileLink)
        {
            MultipartFormDataContent content = new()
            {
                { new StringContent(fileLink), "fileLink" },
                { new StringContent(petitionDTO.Id.ToString()), "petitionId" }
            };
            string endpoint = AppState.BaseUrl + ApiEndPoints.DeleteDocumentFromPetition;
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
            string endpoint = ApiEndPoints.GetUserDTOFromEmail + "?email=" + temporaryAddAuthor;
            UserDTO? newAuthor = await Services.GetDataGenericAsync<UserDTO>(endpoint);
            if (newAuthor == null) Services.AddSnackBarMessage("error", "Did not find a user with that email address");
            else
            {
                petitionDTO.Authors.Add(newAuthor);
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
                petitionDTO.Authors.Remove(authorToRemove);
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
                if (petitionDTO.CommunityDTO != null && petitionDTO.CommunityDTO.Id.HasValue) multiPartFormDataContent.Add(new StringContent(petitionDTO.CommunityDTO.Id.ToString()), nameof(petitionDTO.CommunityDTOId));
                foreach(UserDTO authorDTO in petitionDTO.Authors)
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
                    multiPartFormDataContent.Add(streamContent);
                }
                //send the request
                string endpoint = AppState.BaseUrl + ApiEndPoints.SavePetitionDraft;
                HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);

                request.Content = multiPartFormDataContent;
                PDAPIResponse apiResponse = await Services.SendRequestAsync(request);
                //Update the new petition
                if (apiResponse.SuccessfulOperation)
                {
                    files.Clear();
                    if (apiResponse.Petition != null && petitionId == null) Services.NavigateTo(FrontEndPages.CreatePetition + $"?petitionId={apiResponse.Petition.Id}");
                    else await RefreshPetition();
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
        private void PublishPetition()
        {
            //modal window of are you sure to publish? No more changes! 
            //If there are more authors, show which ones already published
            throw new NotImplementedException();
        }
    }
}
