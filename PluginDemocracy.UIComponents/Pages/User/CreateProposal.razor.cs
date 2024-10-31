using Syncfusion.Blazor.RichTextEditor;

namespace PluginDemocracy.UIComponents.Pages.User
{
    public partial class CreateProposal
    {
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
        private bool disableAllButtons;
        private async void SaveProposalDraft()
        {
            disableAllButtons = true;

            disableAllButtons = false;
        }
        private async void PublishProposal()
        {
            disableAllButtons = true;

            disableAllButtons = false;
        }
    }
}
