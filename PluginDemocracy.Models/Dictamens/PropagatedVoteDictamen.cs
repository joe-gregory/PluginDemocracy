namespace PluginDemocracy.Models
{
    /// <summary>
    /// This Dictamen is a response for when proposals propagate to sub-communities. This Dictamen dictates a vote for the entire community when the sub-proposal passes or fails. 
    /// </summary>
    public class PropagatedVoteDictamen : BaseDictamen
    {
        public Proposal ParentProposal { get; }
        public override MultilingualString Title { get; set; }
        public override MultilingualString Description { get; set; }

        public PropagatedVoteDictamen(Proposal parentProposal) : base()
        {
            ParentProposal = parentProposal;
            Title = new();
            Description = new();
            Title.EN = $"This Dictamen represents a vote in the name of this community for proposal {ParentProposal.Title} in parent community {ParentProposal.Community}.";
            Title.ES = $"Este Dictamen representa un voto en nombre de esta comunidad para la propuesta {ParentProposal.Title} en la comunidad padre {ParentProposal.Community}.";
        }
        /// <summary>
        /// In the case of Homes in GatedCommunities, the Dictamen will check what the ParentProposal is looking for in terms of votes. 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Execute()
        {
            if (Community == null || Proposal == null) throw new InvalidOperationException("Cannot execute Dictamen without a Community and a Proposal.");
            //Check what the parent proposal is expecting. Does it expect my community's citizens or my community?
            if (ParentProposal.CitizensVotingValue.ContainsKey(Community)) ParentProposal.Vote(Community, Proposal.Passed);
            foreach(Citizen citizen in Community.Citizens)
            {
                if (ParentProposal.CitizensVotingValue.ContainsKey(citizen))
                {
                    Vote? vote = Proposal.Votes.FirstOrDefault(vot => vot.Citizen == citizen);
                    if (vote != null) ParentProposal.Vote(vote);
                }
            }
        }
    }
}
