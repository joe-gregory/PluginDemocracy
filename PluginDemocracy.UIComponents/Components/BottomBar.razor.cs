using Microsoft.AspNetCore.Components.Forms;
using PluginDemocracy.API.UrlRegistry;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class BottomBar
    {
        private bool contributeToCommunityDialogVisible = false;
        private bool createPostDialogVisibleInternal = false;
        private bool createPostButtonDisabled = false;
        public bool CreatePostDialogVisible
        {
            get
            {
                return createPostDialogVisibleInternal;
            }

            set
            {
                if (value == false)
                {
                    ClearPostInfo();
                }
                if (AppState.SelectedCommunityInFeed != null) MudSelectCommunityId = AppState.SelectedCommunityInFeed;
                createPostDialogVisibleInternal = value;
            }
        }
        private bool isImageUploadLoading = false;
        private int? MudSelectCommunityId;

        private string? postText = null;
        private readonly IList<IBrowserFile> files = [];
        private readonly Dictionary<string, FileData> fileStreams = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (AppState.SelectedCommunityInFeed != null) MudSelectCommunityId = AppState.SelectedCommunityInFeed;
            AppState.OnChange += () => InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            AppState.OnChange -= () => InvokeAsync(StateHasChanged);
        }
        private void ContributeToCommunity()
        {
            contributeToCommunityDialogVisible = !contributeToCommunityDialogVisible;
        }
        private void OpenCreatePost()
        {
            contributeToCommunityDialogVisible = false;
            CreatePostDialogVisible = true;
        }
        private async Task PostFiledAdded(IReadOnlyList<IBrowserFile> files)
        {
            isImageUploadLoading = true;
            long maxSize = 20 * 1024 * 1024; //5 MB
            foreach (IBrowserFile file in files)
            {
                if (file.Size > maxSize)
                {
                    Services.AddSnackBarMessage("error", $"File is too big. Keep below 20 MB");
                    return;
                }

                string imageUrl = await GetBase64Image(file, maxSize);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    this.files.Add(file);
                }
            }
            isImageUploadLoading = false;
        }

        private void RemoveFile(string fileName)
        {
            fileStreams.Remove(fileName);
        }
        private async Task CreatePost()
        {
            createPostButtonDisabled = true;
            if (string.IsNullOrEmpty(postText))
            {
                Services.AddSnackBarMessage("error", "Please add text to your post");
                return;
            }
            int communityId = 0;
            if (AppState?.User?.Citizenships.Count > 1)
            {
                if (MudSelectCommunityId == null)
                {
                    Services.AddSnackBarMessage("error", "Please select a community");
                    return;
                }
                communityId = MudSelectCommunityId.Value;
            }
            else
            {
                communityId = AppState?.User?.Citizenships[0].Id ?? 0;
            }
            if (communityId == 0)
            {
                Services.AddSnackBarMessage("error", "Please select a community");
                return;
            }
            bool wasUploadSuccessful = await Services.UploadPostAsync(ApiEndPoints.CreateNewPost, postText, fileStreams, communityId);

            //Determine if the post was successfully created before clearing everything
            if (wasUploadSuccessful)
            {
                ClearPostInfo();
                CreatePostDialogVisible = false;
                AppState?.NotifyPostCreation();
            }
            createPostButtonDisabled = false;
        }
        private void ClearPostInfo()
        {
            files.Clear();
            fileStreams.Clear();
            MudSelectCommunityId = null;
            postText = null;
            createPostButtonDisabled = false;
        }
        private async Task<string> GetBase64Image(IBrowserFile file, long maxSize)
        {
            if (file.Size > maxSize)
            {
                Services.AddSnackBarMessage("error", $"File is too big for Base64Image");
                return string.Empty;
            }
            MemoryStream memoryStream = new();
            await using (Stream stream = file.OpenReadStream(maxSize))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream to the beginning for further use
            var base64String = Convert.ToBase64String(memoryStream.ToArray());
            string mimeType = file.ContentType.ToLower();
            // Store the MemoryStream or its byte array for reuse when uploading
            // Example: store it in a dictionary if multiple files are being handled
            fileStreams[file.Name] = new FileData(memoryStream, Convert.ToBase64String(memoryStream.ToArray()), file.Name, file.ContentType);

            return $"data:{mimeType};base64,{base64String}";
        }
        #region classes
        public class FileData
        {
            public MemoryStream Stream { get; set; }
            public string Base64Thumbnail { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }

            public FileData(MemoryStream stream, string base64Thumbnail, string fileName, string contentType)
            {
                Stream = stream;
                Base64Thumbnail = base64Thumbnail;
                FileName = fileName;
                ContentType = contentType;
            }
        }
        #endregion
    }
}
