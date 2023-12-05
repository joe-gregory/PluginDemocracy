using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class HomeOwnership
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int OwnershipPercentage { get; set; }
        public virtual Home? Home { get; set; }
        public virtual BaseCitizen? Owner { get; set; }
    }
}
