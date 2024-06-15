using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class PetitionDrafts
    {
        [Inject]
        BaseAppState AppState { get; set; } = default!;
        [Inject]
        Services Services { get; set; } = default!;
        private List<PetitionDTO>? PetitionDraftsList { get; set; } = null;
        protected async override Task OnInitializedAsync()
        {
            //Get a list of current user's petition drafts
            if (AppState.IsLoggedIn)
            {
                List<PetitionDTO>? petitionDrafts = await Services.GetDataGenericAsync<List<PetitionDTO>>(ApiEndPoints.GetUserPetitionDrafts);
                PetitionDraftsList = petitionDrafts;
            }
             
        }
    }
}
