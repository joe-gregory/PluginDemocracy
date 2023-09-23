namespace PluginDemocracy.Models
{
    /// <summary>
    /// This Dictamen needs to vote in favor or against (depending proposals outcome) on the parent proposal
    /// </summary>
    public class PropagatedProposalDictamen : BaseDictamen
    {
        public Proposal ParentProposal { get; }
        public PropagatedProposalDictamen(Proposal parentProposal) : base()
        {
            ParentProposal = parentProposal;
            Title.EN = $"This Dictamen represents a vote in the name of this community for proposal {ParentProposal.Title} in parent community {ParentProposal.Community}.";
            Title.ES = $"Este Dictamen representa un voto en nombre de esta comunidad para la propuesta {ParentProposal.Title} en la comunidad padre {ParentProposal.Community}.";
        }
        public override void Execute()
        {
            if(Community != null && Proposal != null) ParentProposal.Vote(Community, Proposal.Passed);
            else throw new InvalidOperationException("Cannot execute Dictamen without a Community and a Proposal.");
        }
    }
}
