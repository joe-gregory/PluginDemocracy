namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a voting schema where everybody who is a User type of this Community gets a vote even if it is a member of a sub-community. 
    /// Restrictions can be applied such as age. 
    /// </summary>
    public class UsersVotingStrategy : IVotingStrategy
    {
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
                MultilingualString description = new()
                {
                    EN = "This voting strategy vests voting power on Users. Users are human members of a Community. " +
                    "For child communities that are citizens of this parent community, their own citizens votes are counted as 1 each as well. " +
                    "Every User gets 1 vote regardless if they are a member of the parent community or child communities. " +
                    ". Optional age restrictons can be placed (for example, Users above 18 years old).",
                    ES = "Esta estrategia de voto pone el poder en Usuarios. Usuarios son los miembros humanos de una Comunidad. " +
                    "Para sub-comunidades que son miembros de esta comunidad, sus propios ciudadanos votos se cuentan como 1 cada uno también. " +
                    "Cada Usuario recibe 1 voto independientemente si son miembros de la comunidad o de una sub-comunidad que pertece a esta comunidad. " +
                    "Restricciones de edad se pueden agregar opcionalmente (por ejemplo, solo a mayores de 18 años."
                };
                if (MinimumAge != null)
                {
                    description.EN += $"The minimum voting age has been set at {MinimumAge}. ";
                    description.ES += $"La edad minima de voto se ha restringido a {MinimumAge}. ";
                }
                if(MaximumAge != null)
                {
                    description.EN += $"The maximum voting age has been set at {MaximumAge}. ";
                    description.ES += $"La edad maxima de voto se ha restringido a {MaximumAge}. ";
                }
                return description;
            }
            set
            {
                throw new Exception("Cannot set IVotingStrategy.Description");
            }
        }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        /// <summary>
        /// This method needs to check all sub-communities for Users as well.
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        public Dictionary<Citizen, int> ReturnVotingWeights(Community community)
        {
            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<Citizen, int> citizensVotingValue = new();
            HashSet<User> allUsers = new();
            GetAllNestedUsers(community, allUsers);

            foreach(var user in allUsers)
            {
                if ((MinimumAge == null || user.Age >= MinimumAge) && (MaximumAge == null || user.Age <= MaximumAge)) citizensVotingValue[user] = 1;  // Each user gets one vote
            }

            return citizensVotingValue;
        }
        public void GetAllNestedUsers(Community community, HashSet<User> allUsers)
        {
            foreach(var citizen in community.Citizens)
            {
                if (citizen is User user) allUsers.Add(user);
                else if(citizen is Community nestedCommunity) GetAllNestedUsers(nestedCommunity, allUsers);                
            }
        }

        public void AddHomeVotes(Proposal proposal)
        {
            // Intentionally left empty for this strategy, as no home votes need to be added.
        }
    }
    /// <summary>
    /// Custom exception for invalid community types
    /// </summary>
}
