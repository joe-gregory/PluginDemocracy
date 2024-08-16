using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Petition
    {
        [SupplyParameterFromQuery]
        public int? PetitionId { get; set; }
        protected PetitionDTO? PetitionDTO;

        protected override async Task OnInitializedAsync()
        {
            if (PetitionId != null) PetitionDTO = await Services.GetDataGenericAsync<PetitionDTO>($"{ApiEndPoints.GetPetition}?petitionId={PetitionId}");
            if (PetitionDTO == null) Services.AddSnackBarMessage("warning", "No petition data received.");
        }
    }
}
