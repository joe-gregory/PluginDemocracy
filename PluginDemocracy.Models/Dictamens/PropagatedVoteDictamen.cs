namespace PluginDemocracy.Models
{
    /// <summary>
    /// This Dictamen is a response for when proposals propagate to sub-communities. This Dictamen dictates a vote for the entire community when the sub-proposal passes or fails. 
    /// </summary>
    public class PropagatedVoteDictamen : BaseDictamen
    {
        public Proposal ParentProposal { get; }
        public PropagatedVoteDictamen(string title, string description, Community community, Proposal parentProposal) : base(title, description, community)
        {
            ParentProposal = parentProposal;
            //NEEDS FIXING: 
            //Title = new();
            //Description = new();
            //Title.EN = $"This Dictamen represents a vote in the name of this community for proposal: <br>{ParentProposal.Title}.<br>In parent community {ParentProposal.Community.Name}.";
            //Title.ES = $"Este Dictamen representa un voto en nombre de esta comunidad para la propuesta:<br>{ParentProposal.Title}.<br>En la comunidad padre {ParentProposal.Community.Name}.";
        }
        /// <summary>
        /// In the case of Homes in GatedCommunities, the Dictamen will check what the ParentProposal is looking for in terms of votes. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Execute()
        {
            if (Community == null || Proposal == null) throw new InvalidOperationException("Cannot execute Dictamen without a Community and a Proposal.");
            //Check what the parent proposal is expecting. Does it expect my community's citizens or my community?
            if (ParentProposal.VotingWeights.ContainsKey(Community) && Proposal.Passed != null) ParentProposal.Vote(Community, Proposal.Passed.Value);
            foreach(Citizen citizen in Community.Citizens)
            {
                if (ParentProposal.VotingWeights.ContainsKey(citizen) && Proposal.Passed != null)
                {
                    Vote? vote = Proposal.Votes.FirstOrDefault(vot => vot.Citizen == citizen);
                    if (vote != null) ParentProposal.Vote(vote);
                }
            }
        }
    }
}
