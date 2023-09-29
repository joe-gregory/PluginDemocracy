namespace PluginDemocracy.Models
{
    /// <summary>
    /// This is the simplest form of a voting strategy where each Community.Citizen gets one vote. 
    /// </summary>
    public class CitizenVotingStrategy : IVotingStrategy
    {
        public Type AppliesTo => typeof(Community);

        public MultilingualString Title
        {
            get
            {
                return new MultilingualString()
                {
                    EN = "Voting power in Users with optional age restriction.",
                    ES = "Voto de poder en Usuarios con restriccion de edad opcional."
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
                MultilingualString description = new MultilingualString()
                {
                    EN = "This voting strategy vests voting power on users. Every user gets 1 vote. Optional age restrictons can be placed (for example, Users above 18 years old)." +
            "In Communities comprised of other sub-communities, the sub-communities are ignored and instead their respective users get representation. ",
                    ES = "Esta estrategia de voto pone el poder en Usuarios. Cada Usuario recibe 1 voto. Restricciones de edad se pueden agregar opcionalmente (por ejemplo, solo a mayores de 18 años." +
            "En Comunidades conformadas por otras sub-comunidades, la sub-comunidad es ignorada y en su lugar los usuarios de esa comunidad son representados. "
                };
                return description;
            }
            set
            {
                throw new Exception("Cannot set IVotingStrategy.Description");
            }
        }

        public Dictionary<Citizen, int> ReturnCitizensVotingValue(Community community)
        {
            //throw an error if this doesn't apply to this type of Community
            if (community.GetType() != AppliesTo) throw new InvalidCommunityTypeException("Wrong community type for this type of strategy.");

            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<Citizen, int> citizensVotingValue = new();
            foreach(Citizen citizen in community.Citizens)
            {
                citizensVotingValue[citizen] = 1;
            }

            return citizensVotingValue;
        }
    }
}
