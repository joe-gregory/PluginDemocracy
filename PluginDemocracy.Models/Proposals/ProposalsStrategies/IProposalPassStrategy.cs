namespace PluginDemocracy.Models
{
    public interface IProposalPassStrategy
    {
        bool HasItPassed(Proposal proposal);
    }
}