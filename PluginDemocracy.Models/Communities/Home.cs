using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class Home
    {
        public int Id { get; init; }
        public int Number { get; set; }
        public string InternalAddress { get; set; }
        public ResidentialCommunity? ResidentialCommunity { get; set; }
        /// <summary>
        /// Get only property. Returns Number + InternalAddress + Community.Address.
        /// </summary>
        public string FullAddress 
        { 
            get 
            { 
                return $"{Number} {InternalAddress}.\n{ResidentialCommunity?.Address}"; 
            } 
        }
        private readonly List<HomeOwnership> _ownerships;
        /// <summary>
        /// Get only property. Returns all the ownerships of this home.
        /// </summary>
        public IReadOnlyList<HomeOwnership> Ownerships
        {
            get
            {
                return _ownerships.AsReadOnly();
            }
        }
        /// <summary>
        /// Get only property that returns a dictionary of owners and their ownership percentage of this home. 
        /// </summary>
        public Dictionary<User, double> OwnersOwnerships
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }
        /// <summary>
        /// Get only property that returns a list of owners of this home.
        /// </summary>
        public List<User> Owners 
        {
            get 
            {
                return [.. OwnersOwnerships.Keys];
            }
        }
        /// <summary>
        /// Get only property that returns the currently claimed ownership percentage of this home up to 100%.
        /// </summary>
        public double CurrentlyOwnedPercentage
        {
            get
            {
                return OwnersOwnerships.Values.Sum();
            }
        }
        /// <summary>
        /// Get only property. It returns the available ownership percentage that can be claimed by new owners.
        /// </summary>
        public double AvailableOwnershipPercentage
        {
            get
            {
                return 100 - OwnersOwnerships.Values.Sum();
            }
        }
        private readonly List<User> _residents;
        /// <summary>
        /// Residents are individuals that live in this home but that do not own it. 
        /// </summary>
        public IReadOnlyList<User> Residents 
        { 
            get 
            { 
                return _residents.AsReadOnly();
            } 
        }
        /// <summary>
        /// Get only property. You are a Citizen of this home if you are either an owner or a resident of Home. 
        /// </summary>
        public List<User> Citizens
        {
            get => OwnersOwnerships.Keys.Union(Residents).ToList();
        }
        //Disabling CS8618 as this is a parameterless constructor for the benefit of EF Core
        #pragma warning disable CS8618
        private Home(){}
        #pragma warning restore CS8618
        public Home(ResidentialCommunity community, int number, string internalAddress)
        {
            ResidentialCommunity = community;
            Number = number;
            InternalAddress = internalAddress;
            _ownerships = [];
            _residents = [];
        }
        /// <summary>
        /// Adds owner with percentage or updates owners percentage
        /// </summary>
        /// <param name="user"></param>
        /// <param name="percentage"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddOwner(User user, double percentage)
        {
            if (user == null) throw new ArgumentException("citizen cannot be null");
            if (percentage <= 0 || percentage > 100) throw new ArgumentException("Ownership percentage needs to be between 1 and 100");
            if (percentage > AvailableOwnershipPercentage) throw new ArgumentException("Total ownership percentage exceeds 100. Readjust for this or other owners.");
            HomeOwnership newHomeOwnership = new(this, user, percentage);
            _ownerships.Add(newHomeOwnership);
            user.AddHomeOwnership(newHomeOwnership);
        }
        public void RemoveOwner(User citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            var ownership = Ownerships.FirstOrDefault(o => o.Owner == citizen);
            if (ownership != null)
            {
                _ownerships.Remove(ownership);
                citizen.RemoveHomeOwnership(ownership);
            }
        }
        public void AddResident(User citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            _residents.Add(citizen);
            citizen.AddAsResidentOfHome(this);
        }
        public void RemoveResident(User citizen)
        {
            if (citizen == null) throw new ArgumentException("Citizen cannot be null");

            if (Residents.Contains(citizen)) _residents.Remove(citizen);
            if (citizen.ResidentOfHomes.Contains(this)) citizen.RemoveAsResidentOfHome(this);
        }
    }
}
