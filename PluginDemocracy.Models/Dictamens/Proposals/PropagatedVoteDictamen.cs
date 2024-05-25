namespace PluginDemocracy.Models
{
    /// <summary>
    /// This Dictamen is a response for when proposals propagate to sub-communities. This Dictamen dictates a vote for the entire community when the sub-proposal passes or fails. 
    /// </summary>
    public class PropagatedVoteDictamen : BaseDictamen
    {
        public override string? Title { get => string.Empty; set { } }
        public override string? Description { get => string.Empty; set { } }
        public Proposal? ParentProposal { get; set; }
                protected PropagatedVoteDictamen() 
        {
        }
        /// <summary>
        /// In the case of Homes in GatedCommunities, the Dictamen will check what the ParentProposal is looking for in terms of votes. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public PropagatedVoteDictamen(Proposal parentPropsal) : base()
        {
            ParentProposal = parentPropsal;
        }
        public override void Execute()
        {
            if (Community == null || Proposal == null) throw new InvalidOperationException("Cannot execute Dictamen without a Community and a Proposal.");
            //Check what the parent proposal is expecting. Does it expect my community's citizens or my community?
            if (ParentProposal != null && ParentProposal.VotingWeights.ContainsKey(Community) && Proposal.Passed != null) ParentProposal.Vote(Community, Proposal.Passed.Value);
            foreach(BaseCitizen citizen in Community.Citizens)
            {
                if (ParentProposal != null && ParentProposal.VotingWeights.ContainsKey(citizen) && Proposal.Passed != null)
                {
                    Vote? vote = Proposal.Votes.FirstOrDefault(vot => vot.Citizen == citizen);
                    if (vote != null) ParentProposal.Vote(vote);
                }
            }
        }
    }
}
