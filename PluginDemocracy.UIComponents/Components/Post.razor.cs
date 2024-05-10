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
        [Parameter]
        public PostDto? PostDto { get; set; }
        private string? newCommentText;

        private bool thumbUp = false;
        private bool thumbDown = false;

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
        /// <summary>
        /// Aesthetic changes to thumb up.   
        /// </summary>
        
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
        private void SubmitComment()
        {
            newCommentText = null;
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
    }
}
