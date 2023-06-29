namespace PluginDemocracy.Models
{
    internal interface IProposalPassingSchema
    {
        bool DidPass(Proposal proposal);

        Dictionary<Citizen, int> VotesWeights(Proposal proposal);
    }
}