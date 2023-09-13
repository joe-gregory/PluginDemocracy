/// <summary>
/// This proposal schema interface determines if a proposal is open for voting or not.
/// </summary>
namespace PluginDemocracy.Models
{
    public interface IProposalOpenStatusStrategy
    {
        bool IsOpen(Proposal proposal);
    }
}