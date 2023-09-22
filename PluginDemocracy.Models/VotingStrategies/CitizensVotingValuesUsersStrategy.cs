namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a voting schema where everybody who is a User class of this Community gets a vote. 
    /// Some restrictions can be applied such as age. 
    /// </summary>
    public class CitizensVotingValuesUsersStrategy : IVotingStrategy
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
                MultilingualString description = new MultilingualString()
                {
                    EN = "This voting strategy vests voting power on users. Every user gets 1 vote. Optional age restrictons can be placed (for example, Users above 18 years old)." +
            "In Communities comprised of other sub-communities, the sub-communities are ignored and instead their respective users get representation. ",
                    ES = "Esta estrategia de voto pone el poder en Usuarios. Cada Usuario recibe 1 voto. Restricciones de edad se pueden agregar opcionalmente (por ejemplo, solo a mayores de 18 años." +
            "En Comunidades conformadas por otras sub-comunidades, la sub-comunidad es ignorada y en su lugar los usuarios de esa comunidad son representados. "
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
        public Type AppliesTo => typeof(Community);
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        /// <summary>
        /// This method needs to check all sub-communities for Users as well.
        /// </summary>
        /// <param name="community"></param>
        /// <returns></returns>
        public Dictionary<BaseCitizen, int> ReturnCitizensVotingValue(Community community)
        {
            //throw an error if this doesn't apply to this type of Community
            if (community.GetType() != AppliesTo) throw new InvalidCommunityTypeException("Wrong community type for this type of strategy."); 

            //Scope to see if there are sub-communities and get the Users of said sub-communities.
            Dictionary<BaseCitizen, int> citizensVotingValue = new();
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
    }
    /// <summary>
    /// Custom exception for invalid community types
    /// </summary>
    public class InvalidCommunityTypeException : Exception
    {
        public InvalidCommunityTypeException(string message) : base(message) { }
    }
}
