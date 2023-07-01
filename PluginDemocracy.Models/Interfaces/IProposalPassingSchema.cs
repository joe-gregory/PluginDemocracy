namespace PluginDemocracy.Models
{
    internal interface IProposalPassingSchema
    {
        bool DidPass(Proposal proposal);

        Dictionary<Member, int> VotesWeights(Proposal proposal);
    }
}