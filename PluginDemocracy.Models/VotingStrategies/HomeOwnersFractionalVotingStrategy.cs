namespace PluginDemocracy.Models.VotingStrategies
{
    /// <summary>
    /// Fractional GatedCommunity Voting Strategy
    /// This voting strategy represents a Gated Community where each Home has 100 votes and different owners can vote in different ways.
    /// There are 2 ways to count votes depending on how partial Home owners are treated.
    /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
    /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
    /// The second alternative is that fractional votes of homes are counted as fractional votes
    /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
    /// other 40% against would count against. 
    /// This strategy represents Homes vote as a whole unit because it is the most straighforward implementation given 
    /// that Home is a subclass of Community and how Proposals propagate down. 
    /// allow different strategy implementations  
    /// </summary>
    public class HomeOwnersFractionalVotingStrategy : IVotingStrategy
    {
        public MultilingualString Title
        {
            get
            {
                return new MultilingualString()
                {
                    EN = "Each Home in the gated community gets 100 votes and homeowners of the same home can vote differently.",
                    ES = "Cada casa en la privada corresponde a 100 votos. Dueños de la misma casa pueden votar diferente."
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
                MultilingualString description = new()
                {
                    EN = "This voting strategy vests voting power on home owners. Each home corresponds to 100 votes. Home owners of the same home can vote differently. Their vote value is equal to the home ownership percentage.",
                    ES = "Esta modo de voto pone el poder del voto en los propietarios de casas de la privada. Cada casa corresponde a 100 votos. Propietarios de la misma casa pueden votar diferente. El valor de voto de los propietarios es de acuerdo al porcentage de propiedad sobre la casa."
                };
                return description;
            }
            set
            {
                throw new Exception("Cannot set GatedCommunity.Description");
            }
        }

        public Dictionary<Citizen, int> ReturnVotingWeights(Community community)
        {
            var ownersVotingValue = new Dictionary<Citizen, int>();
            foreach (Home home in community.Homes)
            {
                foreach (Citizen owner in home.Owners.Keys)
                {
                    //an owner may own more than 1 home in the same gated community
                    if (ownersVotingValue.ContainsKey(owner))
                    {
                        ownersVotingValue[owner] += home.Owners[owner];
                    }
                    else
                    {
                        ownersVotingValue[owner] = home.Owners[owner];
                    }
                }
            }
            return ownersVotingValue;
        }
        public void AddHomeVotes(Proposal proposal)
        {
        }
    }
}
