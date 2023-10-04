namespace PluginDemocracy.Models.VotingStrategies
{
    /// <summary>
    /// This voting strategy needs to be part of Home when GatedCommunity is using GatedCommunityFractionalVotingStrategy.
    /// </summary>
    public class HomeFractionalVotingStrategy : IVotingStrategy
    {
        public Type AppliesTo => typeof(Home);

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
                throw new Exception("Cannot set Home.Title");
            }

        }
        public MultilingualString Description
        {
            get
            {
                return new MultilingualString()
                {
                    EN = "This voting strategy vests voting power on home owners. Each home corresponds to 100 votes. Home owners of the same home can vote differently. Their vote value is equal to the home ownership percentage.",
                    ES = "Esta modo de voto pone el poder del voto en los propietarios de casas de la privada. Cada casa corresponde a 100 votos. Propietarios de la misma casa pueden votar diferente. El valor de voto de los propietarios es de acuerdo al porcentage de propiedad sobre la casa."
                };
            }
            set
            {
                throw new Exception("Cannot set Home.Title");
            }
        }

        public Dictionary<Citizen, int> ReturnCitizensVotingValue(Community community)
        {
            throw new NotImplementedException();
        }
    }
}
