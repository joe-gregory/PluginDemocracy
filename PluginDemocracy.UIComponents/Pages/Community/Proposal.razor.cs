using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Proposal
    {
        [SupplyParameterFromQuery]
        protected Guid? ProposalId { get; set; }
        private ProposalDTO proposalDTO = new();
        private bool disableVotingInFavorButton;
        private bool disableVotingAgainstButton;
        private bool disableBothVotingButtons;
        protected override async Task OnInitializedAsync()
        {
            PDAPIResponse? response = await Services.GetDataAsync($"{ApiEndPoints.GetProposal}?proposalId={ProposalId}");
            if (response != null && response.ProposalDTO != null) proposalDTO = response.ProposalDTO;
            if (proposalDTO.Votes.Any(v => v.Voter.Id == AppState.User?.Id)) disableBothVotingButtons = true;
        }
        protected async void Vote(VoteDecision vote)
        {
            disableBothVotingButtons = true;
            PDAPIResponse? response = await Services.PostDataAsync($"{ApiEndPoints.VoteOnProposal}?proposalId={proposalDTO.Id}", vote);
            if (response == null) Services.AddSnackBarMessage("error", "No PDAPIResponse received");
            if (response?.ProposalDTO != null) 
            { 
                proposalDTO = response.ProposalDTO;
                StateHasChanged();
            }
            else Services.AddSnackBarMessage("error", "No ProposalDTO received");
        }
    }
}
