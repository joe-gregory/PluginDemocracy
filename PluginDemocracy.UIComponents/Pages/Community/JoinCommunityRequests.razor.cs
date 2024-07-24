using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    /// <summary>
    /// This is the page that gives you the summary of a given request.
    /// </summary>
    public partial class JoinCommunityRequests
    {
        [SupplyParameterFromQuery]
        public int? RequestId { get; set; }
        private MudBlazor.Color statusColor = MudBlazor.Color.Primary;
        private string statusText = "Pending";
        private JoinCommunityRequestDTO? joinCommunityRequestDTO;
        private HomeDTO? homeToJoinDTO;
        private readonly IList<IBrowserFile> files = [];
        private bool disableAll = false;
        private bool spinnerOn = false;
        private string? message;
        private bool isCurrentUserTheOneFromTheRequest;
        private bool isCurrentUserAdmin;
        private bool isCurrentUserRoleWithHomeOwnershipPowers;
        private bool isCurrentUserRoleWithHomeResidencyPowers;
        protected override async Task OnInitializedAsync()
        {
            if (Services != null && RequestId != null) joinCommunityRequestDTO = await Services.GetDataGenericAsync<JoinCommunityRequestDTO>($"{ApiEndPoints.GetJoinCommunityRequest}?requestId={RequestId}");
            if (Services != null) await Services.GetDataAsync(ApiEndPoints.RefreshUserData);
            if (joinCommunityRequestDTO != null)
            {
                UpdateStatusText();
                if (Services != null) homeToJoinDTO = await Services.GetDataGenericAsync<HomeDTO>($"{ApiEndPoints.GetHomeForJoinCommunityRequestInfo}?requestId={joinCommunityRequestDTO.Id}");
                if (homeToJoinDTO == null && Services != null) Services.AddSnackBarMessage("warning", "HomeDTO information was not received.");
                StateHasChanged();
            }
            isCurrentUserTheOneFromTheRequest = AppState.User?.Id == joinCommunityRequestDTO?.UserDTO?.Id;
            isCurrentUserAdmin = AppState.User?.Admin == true;
            isCurrentUserRoleWithHomeOwnershipPowers = AppState.User?.Roles.Any(r => r.Community?.Id == joinCommunityRequestDTO?.CommunityDTO?.Id && r.Powers.CanEditHomeOwnership == true) ?? false;
            isCurrentUserRoleWithHomeResidencyPowers = AppState.User?.Roles.Any(r => r.Community?.Id == joinCommunityRequestDTO?.CommunityDTO?.Id && r.Powers.CanEditResidency == true) ?? false;
        }
        private void UpdateStatusText()
        {
            if (joinCommunityRequestDTO != null)
            {
                if (joinCommunityRequestDTO.Approved == null)
                {
                    statusColor = MudBlazor.Color.Info;
                    statusText = "Pending";
                }
                else if (joinCommunityRequestDTO.Approved == true)
                {
                    statusColor = MudBlazor.Color.Success;
                    statusText = "Approved";
                }
                else
                {
                    statusColor = MudBlazor.Color.Error;
                    statusText = "Denied";
                }
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
        private async void RefreshJoinCommunityRequestDTO()
        {
            joinCommunityRequestDTO = await Services.GetDataGenericAsync<JoinCommunityRequestDTO>($"{ApiEndPoints.GetJoinCommunityRequest}?requestId={RequestId}");
            if (joinCommunityRequestDTO != null) homeToJoinDTO = await Services.GetDataGenericAsync<HomeDTO>($"{ApiEndPoints.GetHomeForJoinCommunityRequestInfo}?requestId={joinCommunityRequestDTO.Id}");
            UpdateStatusText();
            StateHasChanged();
        }
        private async void UploadSupportingDocumentsToUpload()
        {
            disableAll = true;
            spinnerOn = true;
            AppState.IsLoading = true;
            if (files.Count > 0)
            {
                MultipartFormDataContent multiPartFormDataContent = [];
                int maxAllowedSize = 100 * 1024 * 1024; // 100MB
                foreach (IBrowserFile file in files)
                {
                    StreamContent streamContent = new(file.OpenReadStream(maxAllowedSize: maxAllowedSize));
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "files",
                        FileName = file.Name
                    };
                    multiPartFormDataContent.Add(streamContent);
                }
                string endpoint = $"{ApiEndPoints.AddAdditionalSupportingDocumentsToJoinCommunityRequest}?requestId={joinCommunityRequestDTO?.Id}";
                HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                if (!string.IsNullOrEmpty(AppState.SessionJWT)) request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppState.SessionJWT);
                request.Content = multiPartFormDataContent;
                PDAPIResponse apiResponse = await Services.SendRequestAsync(request);
                if (apiResponse.SuccessfulOperation)
                {
                    files.Clear();
                    Services.AddSnackBarMessages(apiResponse.Alerts);
                    RefreshJoinCommunityRequestDTO();
                }
                else
                {
                    Services.AddSnackBarMessage("error", "Error uploading files.");
                }
            }
            else
            {
                Services.AddSnackBarMessage("warning", "No files to upload.");
            }
            spinnerOn = false;
            disableAll = false;
            AppState.IsLoading = false;
        }
        private async void SendNewMessage()
        {
            if (!string.IsNullOrEmpty(message))
            {
                disableAll = true;
                AppState.IsLoading = true;
                string endpoint = $"{ApiEndPoints.AddMessageToJoinCommunityRequest}?requestId={joinCommunityRequestDTO?.Id}";
                PDAPIResponse pdApiResponse = await Services.PostDataAsync<string>(endpoint, message);
                if (pdApiResponse.SuccessfulOperation) 
                {                     
                    message = null;
                    RefreshJoinCommunityRequestDTO();
                }
                else
                {
                    Services.AddSnackBarMessage("error", "Error sending message.");
                }
            }
            disableAll = false;
            AppState.IsLoading = false;
        }
        private async void AcceptOrRejectRequest(bool accept)
        {
            disableAll = true;
            string endpoint = $"{ApiEndPoints.AcceptOrRejectJoinCommunityRequest}?requestId={joinCommunityRequestDTO?.Id}";
            await Services.PostDataAsync<bool>(endpoint, accept);
            RefreshJoinCommunityRequestDTO();
            disableAll = false;
        }
    }
}
