using System.ComponentModel.DataAnnotations.Schema;
namespace PluginDemocracy.Models
{
    public class ProposalWithDifferentVotingStrategyDictamen : BaseDictamen
    {
        [NotMapped]
        public override string? TitleKey { get => ProposalWithDifferentVotingStrategyDictamenResources.Title; set { } }
        [NotMapped]
        public override string? DescriptionKey { get => ProposalWithDifferentVotingStrategyDictamenResources.Description; set { } }
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
