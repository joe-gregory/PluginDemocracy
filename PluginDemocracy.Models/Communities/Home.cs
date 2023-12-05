namespace PluginDemocracy.Models
{
    public class Home : Community
    {
        public virtual ICollection<HomeOwnership> Ownerships { get; set; }
        public Dictionary<BaseCitizen, int> Owners
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }

        public List<BaseCitizen> Residents { get; set; }
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        override public List<BaseCitizen> Citizens { 
            get => Owners.Keys.Union(Residents).ToList(); 
        }
        public Home() : base()
        {
            Ownerships = new HashSet<HomeOwnership>();
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
        public void AddOwner(BaseCitizen citizen, int percentage)
        {
            if (citizen == null) throw new ArgumentException("citizen cannot be null");
            if (percentage <= 0 || percentage > 100) throw new ArgumentException("Ownership percentage needs to be between 1 and 100 integer");
            int currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new ArgumentException("Total ownership percentage exceeds 100. Readjust for this or other owners.");
            HomeOwnership newOwner = new()
            {
                Owner = citizen,
                OwnershipPercentage = percentage,
                Home = this,
            };
            Ownerships.Add(newOwner);
            citizen.AddCitizenship(this);
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="Citizen"></param>
        public void RemoveOwner(BaseCitizen citizen)
        {
            if (citizen == null)
                throw new ArgumentException("Citizen cannot be null");

            var ownership = Ownerships.FirstOrDefault(o => o.OwnerId == citizen.Id);
            if (ownership != null)
            {
                Ownerships.Remove(ownership);
                if (!Citizens.Contains(citizen))
                    citizen.RemoveCitizenship(this);
            }
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void AddResident(BaseCitizen citizen)
        {
            if (citizen != null)
            {
                if (!Residents.Contains(citizen)) Residents.Add(citizen);
                else throw new InvalidOperationException("Citizen is already a resident");
                if (Citizens.Contains(citizen)) citizen.AddCitizenship(this);
            }
            else throw new ArgumentException("Citizen cannot be null");
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void RemoveResident(BaseCitizen citizen)
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
