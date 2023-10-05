namespace PluginDemocracy.Models
{
    public class Home : Community
    {
        public Dictionary<Citizen, int> Owners { get; set; }
        public List<Citizen> Residents { get; set; }
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        override public List<Citizen> Citizens { 
            get => Owners.Keys.Union(Residents).ToList(); 
        }
        public Home() : base()
        {
            Owners = new();
            Residents = new();
            VotingStrategy = new CitizensVotingStrategy();
            CanHaveNonResidentialCitizens = false;
            CanHaveHomes = false;
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships. 
        /// Adds owner with percentage or updates owners percentage
        /// </summary>
        /// <param name="citizen"></param>
        /// <param name="percentage"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddOwner(Citizen citizen, int percentage)
        {
            if (citizen == null || percentage <= 0 || percentage > 100) throw new ArgumentException("Invalid user or percentage");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new ArgumentException("Total ownership percentage exceeds 100.");
            Owners[citizen] = percentage;
            citizen.AddCitizenship(this);
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="Citizen"></param>
        public void RemoveOwner(Citizen Citizen)
        {
            if (Citizen != null)
            {
                Owners.Remove(Citizen);
                if (!Citizens.Contains(Citizen)) Citizen.RemoveCitizenship(this);
            }
            else throw new ArgumentException("Citizen cannot be null");
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void AddResident(Citizen citizen)
        {
            if (citizen != null)
            {
                if (!Residents.Contains(citizen)) Residents.Add(citizen);
                if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
            }
            else throw new ArgumentException("Citizen cannot be null");
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void RemoveResident(Citizen citizen)
        {
            if(citizen != null)
            {
                if (Residents.Contains(citizen)) Residents.Remove(citizen);
                if (!Citizens.Contains(citizen)) citizen.RemoveCitizenship(this);
            }
            else throw new ArgumentException("Citizen cannot be null");
        }
    }
}
