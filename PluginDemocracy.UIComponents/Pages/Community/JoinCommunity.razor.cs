using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Newtonsoft.Json;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class JoinCommunity
    {
        private List<ResidentialCommunityDTO> communitiesDtos = [];
        private ResidentialCommunityDTO? selectedCommunityDTO;
        private List<HomeDTO> homesDtosFromSelectedCommunity = [];
        private bool isJoinHomeDialogVisible = false;
        private bool disableSendButton = false;
        private bool showSpinner = false;
        private readonly DialogOptions dialogOptions = new()
        {
            CloseButton = true,
            DisableBackdropClick = true,
            CloseOnEscapeKey = true,
        };
        private HomeDTO? selectedHomeDTO;
        /// <summary>
        /// false is resident, true is owner
        /// </summary>
        private bool residentOrOwner = false;
        private bool displayDialogErrorMessage = false;
        private string dialogErrorMessage = string.Empty;
        private double selectedOwnershipPercentage = 100;
        private readonly IList<IBrowserFile> files = [];
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //Make get request to get list of communities
            PDAPIResponse response = await Services.GetDataAsync(ApiEndPoints.GetListOfAllCommunities);
            communitiesDtos = response.AllCommunities;
        }
        private async Task OnSelectCommunityDtoChanged(ResidentialCommunityDTO newValue)
        {
            selectedCommunityDTO = newValue;
            //Now load the homes for the selected community
            if (selectedCommunityDTO != null)
            {
                await LoadHomesForSelectedCommunity();
            }
        }
        private async Task LoadHomesForSelectedCommunity()
        {
            //Make get request to get list of homes for the selected community
            if (selectedCommunityDTO == null) return;
            string fullUrl = ApiEndPoints.GetListOfHomesForCommunity + $"?communityId={selectedCommunityDTO.Id}";
            PDAPIResponse response = await Services.GetDataAsync(fullUrl);
            if (response.Community != null) homesDtosFromSelectedCommunity = response.Community.Homes;
        }
        private void SelectedHome(int? homeId)
        {
            if (homeId == null) return;
            selectedHomeDTO = homesDtosFromSelectedCommunity.FirstOrDefault(h => h.Id == homeId);
            displayDialogErrorMessage = false;
            dialogErrorMessage = string.Empty;
            isJoinHomeDialogVisible = true;
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
        private async Task SendRequest()
        {
            showSpinner = true;
            AppState.IsLoading = true;
            disableSendButton = true;
            displayDialogErrorMessage = false;
            dialogErrorMessage = string.Empty;
            JoinCommunityRequestDTO joinRequestDTO;
            if (AppState.User != null && selectedHomeDTO != null)
            {
                try
                {
                    selectedHomeDTO.Community = selectedCommunityDTO;
                    joinRequestDTO = selectedHomeDTO.JoinHome(AppState.User, residentOrOwner, selectedOwnershipPercentage);
                    joinRequestDTO.CommunityDTO = selectedCommunityDTO;
                    displayDialogErrorMessage = false;
                    dialogErrorMessage = string.Empty;

                    JoinCommunityRequestUploadDTO joinRequestUploadDTO = new()
                    {
                        CommunityId = joinRequestDTO.CommunityDTO?.Id ?? 0,
                        HomeId = joinRequestDTO.HomeDTO?.Id ?? 0,
                        JoiningAsOwner = joinRequestDTO.JoiningAsOwner,
                        JoiningAsResident = joinRequestDTO.JoiningAsResident,
                        OwnershipPercentage = joinRequestDTO.OwnershipPercentage,
                    };

                    MultipartFormDataContent multiPartFormDataContent = [];
                    multiPartFormDataContent.Add(new StringContent(joinRequestUploadDTO.CommunityId.ToString()), nameof(joinRequestUploadDTO.CommunityId));
                    multiPartFormDataContent.Add(new StringContent(joinRequestUploadDTO.HomeId.ToString()), nameof(joinRequestUploadDTO.HomeId));
                    multiPartFormDataContent.Add(new StringContent(joinRequestUploadDTO.JoiningAsOwner.ToString()), nameof(joinRequestUploadDTO.JoiningAsOwner));
                    multiPartFormDataContent.Add(new StringContent(joinRequestUploadDTO.JoiningAsResident.ToString()), nameof(joinRequestUploadDTO.JoiningAsResident));
                    multiPartFormDataContent.Add(new StringContent(joinRequestUploadDTO.OwnershipPercentage.ToString()), nameof(joinRequestUploadDTO.OwnershipPercentage));
                    // Add files if any
                    if (files.Count > 0)
                    {
                        int maxAllowedSize = 100 * 1024 * 1024; // 100MB
                        foreach (IBrowserFile file in files)
                        {
                            StreamContent streamContent = new(file.OpenReadStream(maxAllowedSize: maxAllowedSize));
                            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                            streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                            {
                                Name = nameof(joinRequestUploadDTO.SupportingDocumentsToAdd),
                                FileName = file.Name
                            };
                            multiPartFormDataContent.Add(streamContent);
                        }
                    }
                    else
                    {
                        // Add an empty content for SupportingDocumentsToAdd if no files are present
                        multiPartFormDataContent.Add(new StringContent("[]"), nameof(joinRequestUploadDTO.SupportingDocumentsToAdd));
                    }
                    //send the request: 
                    string endpoint = AppState.BaseUrl + ApiEndPoints.JoinCommunityRequest;
                    HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                    if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);
                    request.Content = multiPartFormDataContent;
                    PDAPIResponse apiResponse = await Services.SendRequestAsync(request);
                    if (apiResponse.SuccessfulOperation)
                    {
                        files.Clear();
                        Services.AddSnackBarMessages(apiResponse.Alerts);
                    }
                    else
                    {
                        Services.AddSnackBarMessage("error", "Error sending request.");
                    }
                    isJoinHomeDialogVisible = false;
                    disableSendButton = false;
                    showSpinner = false;
                }
                catch (Exception e)
                {
                    displayDialogErrorMessage = true;
                    dialogErrorMessage = AppState.Translate(Translations.ResourceKeys.ErrorMessageJoinHomeWrongPercentage);
                    Services.AddSnackBarMessage("error", e.Message);
                    disableSendButton = false;
                    showSpinner = false;
                    return;
                }
            }
            disableSendButton = false;
            AppState.IsLoading = false;
            showSpinner = false;
        }
    }
}
