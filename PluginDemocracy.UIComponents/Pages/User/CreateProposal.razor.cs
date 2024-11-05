using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using Syncfusion.Blazor.RichTextEditor;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class CreateProposal
    {
        

        [SupplyParameterFromQuery]
        public Guid? ProposalId { get; set; }
        private string? title;
        private string richTextEditorValue = string.Empty;
        private ResidentialCommunityDTO? communityDTO;

        private ProposalDTO proposalDTO = new();

        private static readonly List<ToolbarItemModel> Tools =
        [
            new() { Command = ToolbarCommand.Bold },
            new() { Command = ToolbarCommand.Italic },
            new() { Command = ToolbarCommand.Underline },
            new() { Command = ToolbarCommand.StrikeThrough },
            new() { Command = ToolbarCommand.FontName },
            new() { Command = ToolbarCommand.FontSize },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.FontColor },
            new() { Command = ToolbarCommand.BackgroundColor },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.Formats },
            new() { Command = ToolbarCommand.Alignments },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.LowerCase },
            new() { Command = ToolbarCommand.UpperCase },
            new() { Command = ToolbarCommand.SuperScript },
            new() { Command = ToolbarCommand.SubScript },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.OrderedList },
            new() { Command = ToolbarCommand.UnorderedList },
            new() { Command = ToolbarCommand.Outdent },
            new() { Command = ToolbarCommand.Indent },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.CreateLink },
            new() { Command = ToolbarCommand.Image },
            new() { Command = ToolbarCommand.CreateTable },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.ClearFormat },
            new() { Command = ToolbarCommand.Print },
            new() { Command = ToolbarCommand.SourceCode },
            new() { Command = ToolbarCommand.FullScreen },
            new() { Command = ToolbarCommand.Separator },
            new() { Command = ToolbarCommand.Undo },
            new() { Command = ToolbarCommand.Redo }
        ];
        private bool disableAll = false;
        protected override async Task OnInitializedAsync()
        {
            //if existing proposal draft 
            if (ProposalId != null)
            {
                //let's get the proposal draft
                string endpoint = ApiEndPoints.GetProposalDraft + $"?proposalId={ProposalId}";
                ProposalDTO? proposalDTOMessage = await Services.GetDataGenericAsync<ProposalDTO>(endpoint);
                if (proposalDTOMessage != null)
                {
                    proposalDTO = proposalDTOMessage;
                    UpdateFieldsFromProposalDTO();
                }
                else proposalDTO = new();
            }
            //if it's a new proposal
            else
            {
                title = "Please enter a title for your proposal";
                communityDTO = AppState?.User?.Citizenships[0];
            }
        }
        private async void SaveProposalDraft()
        {
            disableAll = true;
            UpdateProposalDTOFromFields();
            try
            {
                PDAPIResponse response = await Services.PostDataAsync<ProposalDTO>(ApiEndPoints.SaveProposalDraft, proposalDTO);
                if (response.ProposalDTO != null)
                {
                    proposalDTO = response.ProposalDTO;
                    UpdateFieldsFromProposalDTO();
                }
            }
            catch
            {
                Services.AddSnackBarMessage("error", "There was an issue from the server response");
            }
            disableAll = false;
            StateHasChanged();
        }
        private async void PublishProposal()
        {
            disableAll = true;
            UpdateProposalDTOFromFields();

            disableAll = false;
        }
        private void UpdateProposalDTOFromFields()
        {
            proposalDTO.Id = ProposalId;
            proposalDTO.Title = title;
            proposalDTO.Content = richTextEditorValue;
            proposalDTO.Community = communityDTO;
        }
        private void UpdateFieldsFromProposalDTO()
        {
            title = proposalDTO.Title;
            richTextEditorValue = proposalDTO.Content ?? string.Empty;
            communityDTO = proposalDTO.Community;
            StateHasChanged();
        }
        private async void DeleteProposal()
        {
            disableAll = true;
            PDAPIResponse response = await Services.DeleteDataAsync(ApiEndPoints.DeleteProposalDraft + $"?proposalId={proposalDTO.Id}");
            if (response.SuccessfulOperation) Services.NavigateTo(FrontEndPages.ProposalDrafts);
            disableAll = false;
        }
    }
}
