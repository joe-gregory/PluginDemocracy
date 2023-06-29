using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Community
    {
        public Guid Guid { get; }
        public List<GeoCoordinates> Borders { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Citizen> Citizens { get; set; }
        public Dictionary<Citizen, int> VotesWeights { get; set; }
        public List<Proposal> Proposals { get; set; }

    }
}
