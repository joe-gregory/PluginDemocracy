using Microsoft.AspNetCore.Components;
using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.DTOs.CommunitiesDto;
using PluginDemocracy.Models;
using PluginDemocracy.UIComponents.Translations;

namespace PluginDemocracy.UIComponents.Components
{
    public partial class Post
    {
        [Inject]
        private BaseAppState AppState { get; set; } = default!;
        [Inject]
        private Services Services { get; set; } = default!;
        [Inject]
        private IDialogService DialogService { get; set; } = default!;
        /// <summary>
        /// This is the Post to display
        /// </summary>
        [Parameter]
        public PostDto? PostDto { get; set; }
        /// <summary>
        /// This is the bind value for writing a comment on the text box. 
        /// </summary>
        private string? newCommentText;

        private string cssThumb = cssForUnpressedThumb;
        const string cssForUnpressedThumb = "thumb-options-open";
        const string cssForPressedThumb = "thumb-options-closed";

        private Color thumbUpColor = Color.Default;
        private Color thumbDownColor = Color.Default;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RefreshLookOfThumbs();
        }
        private async void ReactToPost(ReactionType reactionType)
        {
            if (AppState.User != null && PostDto != null)
            {
                PostReactionDto reaction = new(PostDto.Id, AppState.User, reactionType);
                List<PostReactionDto>? PostDtoReactions = await Services.PostDataGenericAsync<PostReactionDto, List<PostReactionDto>>(ApiEndPoints.ReactToPost, reaction);
                if (PostDtoReactions != null)
                {
                    PostDto.Reactions = PostDtoReactions;
                    RefreshLookOfThumbs();
                }
            }
        }
        private void RefreshLookOfThumbs()
        {
            //Check if current user has already reacted to this post, color the corresponding thumb
            if (PostDto?.Reactions?.Any(r => r.User.Equals(AppState.User) && r.ReactionType == ReactionType.Like) == true)
            {
                PressThumbUp();
            }
            else if (PostDto?.Reactions?.Any(r => r.User.Equals(AppState.User) && r.ReactionType == ReactionType.Dislike) == true)
            {
                PressThumbDown();
            }
            else
            {
                DePressThumbUp();
                DePressThumbDown();
            }
            StateHasChanged();
        }
        private void PressThumbUp()
        {
            thumbUpColor = Color.Success;
            cssThumb = cssForPressedThumb;
            thumbDownColor = Color.Default;
        }
        private void DePressThumbUp()
        {
            thumbUpColor = Color.Default;
            cssThumb = cssForUnpressedThumb;
        }
        /// <summary>
        /// Aesthethic changes to thumb down when pressed
        /// </summary>
        private void PressThumbDown()
        {
            thumbDownColor = Color.Error;
            cssThumb = cssForPressedThumb;
            thumbUpColor = Color.Default;
        }
        /// <summary>
        /// Aesthethic changes to thumb down when depressed
        /// </summary>
        private void DePressThumbDown()
        {
            thumbDownColor = Color.Default;
            cssThumb = cssForUnpressedThumb;
        }
        private async void SubmitComment()
        {
            //Make post request to submit comment
            if (AppState.User != null && PostDto != null && !string.IsNullOrEmpty(newCommentText))
            {
                PostCommentDto postComment = new(PostDto.Id, AppState.User, newCommentText);
                //Update this Post with this request:
                PostDto? refreshedPost = await Services.PostDataGenericAsync<PostCommentDto, PostDto>(ApiEndPoints.AddCommentToPost, postComment);
                if (refreshedPost != null)
                {
                    //If successful, add comment to PostDto. which refreshing the post should do:
                    PostDto = refreshedPost;
                    //Clear the text box
                    newCommentText = null;
                    StateHasChanged();
                }
                else
                {
                    //If not successful, show error message
                    Services.AddSnackBarMessage("error", "Error");
                }
            }
        }
        private async void DeletePost()
        {
            if (PostDto != null)
            {
                bool? confirmation = await DialogService.ShowMessageBox(
                    AppState.Translate(ResourceKeys.DeletePost),
                    AppState.Translate(ResourceKeys.DeletePostCannotBeUndone),
                    yesText: "Ok",
                    cancelText: "Cancel");
                if (confirmation == true)
                {
                    bool success = await Services.DeletePostAsync(PostDto.Id);
                    if (success)
                    {
                        AppState.DeletePost(PostDto);
                    }
                }
            }
        }
        public void RemoveComment(PostCommentDto comment)
        {
            if (PostDto != null)
            {
                PostDto.Comments.Remove(comment);
                StateHasChanged();
            }
        }
    }
}
