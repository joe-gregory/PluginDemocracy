namespace PluginDemocracy.Models
{
    public class HomeVotingStrategy : IVotingStrategy
    {
        public Type AppliesTo => typeof(Home);

        public MultilingualString Title 
        {
            get
            {
                return new MultilingualString()
                {
                    EN = "100 voting points spread among owners",
                    ES = "100 puntos de votacion repartidos entre los propietarios."
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
                    EN = "There are a total of 100 votes that represent 100% ownership of the home. Each owner gets as many votes as percentage of ownership.",
                    ES = "Hay un total de 100 votos que representan el 100% de propiedad de la casa. Cada dueño de casa recibe puntos de acuerdo a su porcentage de propiedad."
                };
            }
            set
            {
                throw new Exception("Cannot set Home.Title");
            }
        }

        public Dictionary<Citizen, int> ReturnCitizensVotingValue(Community community)
        {
            if (community is Home home)
            {
                return home.Owners;
            }
            else throw new Exception("Community provided is not of Home type.");
        }
    }
}
