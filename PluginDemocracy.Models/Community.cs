using PluginDemocracy.Models.Interfaces;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public abstract class Community : Citizen
    {
        //Basic information
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public IReadOnlyDictionary<Citizen, decimal>? Owners => _owners?.AsReadOnly();
        protected Dictionary<Citizen, decimal>? _owners;
        public IReadOnlyList<Citizen> Citizens { get { return _citizens.AsReadOnly(); } }
        private List<Citizen> _citizens;
        public IReadOnlyDictionary<Citizen, decimal> CitizensVotingWeights { get { return CommunitysCitizensVotingWeightsStrategy.ReturnCitizensVotingWeights(this); } }
        public Constitution Constitution { get; private set; }
        public List<Proposal> Proposals { get; private set; }
        public List<Role> Roles { get; private set; }
        /// <summary>
        /// Strategies for the community
        /// </summary>
        public IProposalPassStrategy ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy CommunitysCitizensVotingWeightsStrategy { get; set; }
        public IDictamenValidityStrategy DictamenValidityStrategy { get; set; }
        public void JoinCitizen(Citizen citizen)
        {
            if (!Citizens.Contains(citizen))
            {
                _citizens.Add(citizen);
                citizen.AddCommunity(this);
            }
        }
        public void RemoveCitizen(Citizen citizen)
        {
            _citizens.Remove(citizen);
            citizen.RemoveCommunity(this);
        }
        public void Update()
        {
            Constitution.Update();
            foreach (var proposal in Proposals) proposal.Update();
            foreach (var role in Roles) role.Update();
        }
    }
}
