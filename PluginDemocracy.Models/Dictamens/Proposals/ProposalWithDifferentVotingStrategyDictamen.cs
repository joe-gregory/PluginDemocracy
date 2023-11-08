namespace PluginDemocracy.Models
{
    public class ProposalWithDifferentVotingStrategyDictamen : BaseDictamen
    {
        public override string Title => ProposalWithDifferentVotingStrategyDictamenResources.Title;
        public override string Description => ProposalWithDifferentVotingStrategyDictamenResources.Description;
        public Proposal? ProposalToCreate { get; set; }

        public override void Execute()
        {
            if(Community != null && ProposalToCreate != null)
            {
                Community.PublishProposal(ProposalToCreate);
            }
            else
            {
                throw new Exception("Proposal To Create or Community is null");
            }
        }
    }
}
