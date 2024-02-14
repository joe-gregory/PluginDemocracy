using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class Home : BaseCitizen
    {
        public Community? ParentCommunity { get; set; }
        [NotMapped]
        public override string FullName
        {
            get
            {
                string fullName = string.Empty;
                foreach (HomeOwnership hw in Ownerships) fullName += hw.Owner.FullName + ", ";
                return fullName;
            }
        }
        [NotMapped]
        public override List<Community> Citizenships
        {
            get
            {
                if (ParentCommunity != null) return [ParentCommunity];
                else return [];
            }
        }
        [NotMapped]
        public override string Address { get => InternalAddress + "/n" + ParentCommunity?.Address; }
        public string InternalAddress { get; set; }
        public ICollection<HomeOwnership> Ownerships { get; set; }
        [NotMapped]
        public Dictionary<BaseCitizen, double> Owners
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }
        public List<User> Residents { get; set; }
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        [NotMapped]
        public List<BaseCitizen> Citizens
        {
            get => Owners.Keys.Union(Residents).ToList();
        }
        public Home() : base()
        {
            InternalAddress = string.Empty;
            Ownerships = new HashSet<HomeOwnership>();
            Residents = [];
        }
        /// <summary>
        /// Adds owner with percentage or updates owners percentage
        /// </summary>
        /// <param name="citizen"></param>
        /// <param name="percentage"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddOwner(BaseCitizen citizen, double percentage)
        {
            if (citizen == null) throw new ArgumentException("citizen cannot be null");
            if (percentage <= 0 || percentage > 100) throw new ArgumentException("Ownership percentage needs to be between 1 and 100");
            double currentTotalPercentage = Owners.Values.Sum();
            if (currentTotalPercentage + percentage > 100) throw new ArgumentException("Total ownership percentage exceeds 100. Readjust for this or other owners.");
            HomeOwnership newHomeOwnership = new(this, citizen, percentage);
            Ownerships.Add(newHomeOwnership);
            citizen.HomeOwnerships.Add(newHomeOwnership);
        }
        /// <summary>
        /// </summary>
        /// <param name="Citizen"></param>
        public void RemoveOwner(BaseCitizen citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            var ownership = Ownerships.FirstOrDefault(o => o.Owner == citizen);
            if (ownership != null)
            {
                Ownerships.Remove(ownership);
                citizen.HomeOwnerships.Remove(ownership);
            }
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void AddResident(User citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            Residents.Add(citizen);
            citizen.ResidentOfHomes.Add(this);
        }
        /// <summary>
        /// This method should be called on parent Gated Community to correctly update Citizen.Citizenships
        /// </summary>
        /// <param name="citizen"></param>
        public void RemoveResident(User citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            if (Residents.Contains(citizen)) Residents.Remove(citizen);
            if (citizen.ResidentOfHomes.Contains(this)) citizen.ResidentOfHomes.Remove(this);
        }
    }
}
