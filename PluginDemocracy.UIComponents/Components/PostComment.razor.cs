using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class PostComment
    {
        [Parameter]
        public PostCommentDto? PostCommentDto { get; set; }
        [Parameter]
        public EventCallback<PostCommentDto> OnDeleteComment { get; set; }
        [Inject]
        private BaseAppState AppState { get; set; } = default!;
        [Inject]
        private Services Services { get; set; } = default!;
        private async void DeleteComment()
        {
            PDAPIResponse response = new();
            if (PostCommentDto != null) response = await Services.DeleteDataAsyncGeneric<PostCommentDto>(ApiEndPoints.DeleteComment, PostCommentDto);
            if (response.SuccessfulOperation && OnDeleteComment.HasDelegate)
            {
                await OnDeleteComment.InvokeAsync(PostCommentDto);
            }
        
        }

    }
}
