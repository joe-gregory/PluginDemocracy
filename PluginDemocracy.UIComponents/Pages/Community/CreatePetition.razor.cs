using Microsoft.AspNetCore.Components.Forms;
using PluginDemocracy.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class CreatePetition
    {
        private readonly PetitionDTO Petition = new();
        private readonly IList<IBrowserFile> files = [];
        protected override void OnInitialized()
        {
            if (AppState?.User != null) Petition.Authors.Add(AppState.User);
            if (AppState?.User?.Citizenships.Count == 1) Petition.CommunityDTO = AppState.User.Citizenships[0];
        }
    }
}
