namespace PluginDemocracy.Models
{
    /// <summary>
    /// This is the simplest form of a voting strategy where each Community.Citizen gets one vote. 
    /// </summary>
    public class CitizensVotingStrategy : IVotingStrategy
    {
        public MultilingualString Title
        {
            get
            {
                return new MultilingualString()
                {
                    EN = "1 vote per Citizen",
                    ES = "1 voto por Ciudadano"
                };
            }
            set
            {
                throw new Exception("Cannot set IVotingStrategy.Title");
            }
        }
        public MultilingualString Description
        {
            get
            {
                MultilingualString description = new()
                {
                    EN = "This is the simplest voting strategy. Each Citizen gets 1 vote. A Citizen can be a human User or another sub-community. " +
                    "If a sub-community is a Citizen of this community, it counts as 1 vote. The members of the sub-community will vote according to their own voting strategy " +
                    "and the result will be added as a single vote to this community.",
                ES = "Esta es la estrategia de voto mas sencilla. Cada Ciudadano cuenta como 1 voto. Un Ciudadano puede ser un Usuario humano o puede ser una sub-comunidad. " +
                    "Si una sub-comunidad es Ciudadano de esta comunidad, el voto de la sub-comunidad cuenta como 1 voto independientemente de cuantos Ciudadanos la sub-comunidad tenga." +
                    "Los miembros de la sub-comunidad votaran de acuerdo a su propia estrategia de voto y el resultado sera pasado como un solo voto a esta comunidad." 
                };
                return description;
            }
            set
            {
                throw new Exception("Cannot set IVotingStrategy.Description");
            }
        }



        public Dictionary<Citizen, int> ReturnVotingWeights(Community community)
        {
            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<Citizen, int> citizensVotingValue = new();
            foreach(Citizen citizen in community.Citizens)
            {
                citizensVotingValue[citizen] = 1;
            }

            return citizensVotingValue;
        }
        public void AddHomeVotes(Proposal proposal)
        {
            // Intentionally left empty for this strategy, as no home votes need to be added.
        }
    }
}
