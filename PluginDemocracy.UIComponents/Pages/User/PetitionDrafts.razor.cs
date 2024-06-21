using Microsoft.AspNetCore.Components;
using MudBlazor;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class PetitionDrafts
    {
        [Inject]
        BaseAppState AppState { get; set; } = default!;
        [Inject]
        Services Services { get; set; } = default!;
        private IEnumerable<PetitionDTO> PetitionDraftsList { get; set; } = [];
        private bool dialogVisibleMultipleAuthors = false;
        private int petitionDraftIdToDelete = 0;
        protected async override Task OnInitializedAsync()
        {
            //Get a list of current user's petition drafts
            if (AppState.IsLoggedIn)
            {
                List<PetitionDTO>? petitionDrafts = await Services.GetDataGenericAsync<List<PetitionDTO>>(ApiEndPoints.GetUserPetitionDrafts);
                if (petitionDrafts != null) PetitionDraftsList = [.. petitionDrafts.OrderByDescending(p => p.LastUpdated)];
            }

        }
        private void CheckDeletePetitionDraft(int petitionId)
        {
            petitionDraftIdToDelete = petitionId;
            //If there are multiple authors, let the person know that the petition draft will not be deleted
            //and instead he/she will be removed as an author
            if (PetitionDraftsList?.FirstOrDefault(p => p.Id == petitionId)?.Authors.Count > 1)
            {
                dialogVisibleMultipleAuthors = true;
                return;
            }
            else
            {
                DeletePetitionDraft();
            }
        }
        private async void DeletePetitionDraft()
        {
            await Services.DeleteDataAsync(ApiEndPoints.DeletePetitionDraft + $"?petitionId={petitionDraftIdToDelete}");
            dialogVisibleMultipleAuthors = false;
        }
        private void CanceledDeleteMultipleAuthors()
        {
            dialogVisibleMultipleAuthors = false;
            petitionDraftIdToDelete = 0;
        }
    }
}
