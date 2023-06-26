namespace PluginDemocracy.Models
{
    internal interface IProposalPassingSchema
    {
        bool DidPass(Proposal proposal);
        Dictionary<Citizen, int> WeightedVotes(Proposal proposal);
    }
}