using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class PostComment
    {
        [Parameter]
        public PostCommentDTO? PostCommentDto { get; set; }
        [Parameter]
        public EventCallback<PostCommentDTO> OnDeleteComment { get; set; }
        [Inject]
        private BaseAppState AppState { get; set; } = default!;
        [Inject]
        private Services Services { get; set; } = default!;
        private async void DeleteComment()
        {
            PDAPIResponse response = new();
            if (PostCommentDto != null) response = await Services.DeleteDataAsyncGeneric<PostCommentDTO>(ApiEndPoints.DeleteComment, PostCommentDto);
            if (response.SuccessfulOperation && OnDeleteComment.HasDelegate)
            {
                await OnDeleteComment.InvokeAsync(PostCommentDto);
            }
        
        }

    }
}
