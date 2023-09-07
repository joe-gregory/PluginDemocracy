namespace PluginDemocracy.Models
{
    public interface IProposalPassingStrategy
    {
        bool DidPass(Proposal proposal);

        Dictionary<Member, int> VotesWeights(Proposal proposal);
    }
}