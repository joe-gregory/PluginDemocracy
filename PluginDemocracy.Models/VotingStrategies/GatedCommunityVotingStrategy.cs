namespace PluginDemocracy.Models
{
    /// <summary>
    /// This voting strategy represents a Gated Community where each owner(s) has a vote.
    /// There are 2 ways to count votes depending on how partial Home owners are treated.
    /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
    /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
    /// The second alternative is that fractional votes of homes are counted as fractional votes
    /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
    /// other 40% against would count against. 
    /// For now Homes vote as a unit because it is the most straighforward implementation given 
    /// that Home is a subclass of Community and how Proposals propagate down. 
    /// allow different strategy implementations  
    /// </summary>
    /// <remarks>
    /// In the future would like to implement a voting strategy that allows for fractional votes for home owners.
    /// </remarks>
    public class GatedCommunityVotingStrategy : IVotingStrategy
    {
        public Type AppliesTo => typeof(GatedCommunity);

        public MultilingualString Title {
            get
            {
                return new MultilingualString()
                {
                    EN = "Each Home in the gated community gets a vote.",
                    ES = "Cada casa en la privada corresponde a 1 voto. "
                };
            }
            set
            {
                throw new Exception("Cannot set GatedCommunityVotingStrategy.Title");
            }
        }
        public MultilingualString Description 
        {
            get
            {
                MultilingualString description = new MultilingualString()
                {
                    EN = "This voting strategy vests voting power on home owners. Each home corresponds to 1 vote.",
                    ES = "Esta modo de voto pone el poder del voto en los propietarios de casas de la privada. Cada casa corresponde a 1 voto."
                };
                return description;
            }
            set
            {
                throw new Exception("Cannot set GatedCommunity.Description");
            }
        }

        public Dictionary<BaseCitizen, int> ReturnCitizensVotingValue(Community community)
        {
            if (community is GatedCommunity gatedCommunity)
            {
                var homesVotingValue = new Dictionary<BaseCitizen, int>();
                foreach (Home home in gatedCommunity.Homes) homesVotingValue[home] = 100;
                return homesVotingValue;
            }
            else throw new Exception("community argument is not of type GatedCommunity.");
        }
    }
}
