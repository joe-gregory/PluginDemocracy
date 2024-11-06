using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Proposal
    {
        [SupplyParameterFromQuery]
        protected Guid? ProposalId { get; set; }
        private ProposalDTO proposalDTO = new();
        protected override async Task OnInitializedAsync()
        {
            PDAPIResponse? response = await Services.GetDataAsync($"{ApiEndPoints.GetProposal}?proposalId={ProposalId}");
            if (response != null && response.ProposalDTO != null) proposalDTO = response.ProposalDTO;   
        }
    }
}
