namespace PluginDemocracy.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public abstract class AbstractCommunity: AbstractCitizen
    {
        //Basic information
        public string Name { get; private set; }
        public string Address { get; private set; }
        /// <summary>
        /// The citizens property will be returned as read-only as each community might save citizens in different ways. For example, a PrivadaCommunity will have a 
        /// list of homes and each home will have a list of residents. Someone might own more than one house. Citizens will return a list of the citizens that are part of the community regardless 
        /// of voting status and such. 
        /// </summary>
        public List<AbstractCitizen> Citizens { get; }
        public Dictionary<AbstractCitizen, int> CommunityCitizenStructure { get; }
        //Strategies that define how the community operates, like who has the right to vote, how the voting weights are calculated, etc.
        public IVotingEligibilityStrategy VotingEligibilityStrategy { get; private set; }
        public IVotingChangeStrategy VotingChangeStrategy { get; private set; }
        public IProposalPassingStrategy ProposalPassingStrategy { get; private set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; private set; }
        public IDictamenValidityStrategy DictamenValidityStrategy { get; private set; }
        protected AbstractCommunity(CommunityBasicInfoAndStrategyOptions communityBasicInfoAndStrategyOptions)
        {
            Name = communityBasicInfoAndStrategyOptions.Name;
            Address = communityBasicInfoAndStrategyOptions.Address;
            Citizens = new();
            CommunityCitizenStructure = new();
            VotingEligibilityStrategy = communityBasicInfoAndStrategyOptions.VotingEligibilityStrategy;
            VotingChangeStrategy = communityBasicInfoAndStrategyOptions.VotingChangeStrategy;
            ProposalPassingStrategy = communityBasicInfoAndStrategyOptions.ProposalPassingStrategy;
            ProposalOpenStatusStrategy = communityBasicInfoAndStrategyOptions.ProposalOpenStatusStrategy;
            DictamenValidityStrategy = communityBasicInfoAndStrategyOptions.DictamenValidityStrategy;
        }
        public abstract void JoinCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject = null) where T : class;
        public abstract void RemoveCitizen<T>(AbstractCitizen citizen, T? additionalInfoObject) where T : class;
    }
    /// <summary>
    /// When instantiating a new community, all this information will be necessary and the base constructor of AbstractCommunity will expect it. Other 
    /// types of communities that inherit from AbstractCommunity might 
    /// </summary>
    public class CommunityBasicInfoAndStrategyOptions
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IVotingEligibilityStrategy VotingEligibilityStrategy { get; set; }
        public IVotingChangeStrategy VotingChangeStrategy { get; set; }
        public IProposalPassingStrategy ProposalPassingStrategy { get; set; }
        public IProposalOpenStatusStrategy ProposalOpenStatusStrategy { get; set; }
        public IDictamenValidityStrategy DictamenValidityStrategy { get; set; }
    }
}
