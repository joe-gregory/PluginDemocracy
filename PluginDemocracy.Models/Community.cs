using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Community
    {
        public string Name { get; set; }
        public string Address { get; set; }
        //geo location borders
        //Figure out home/house classes
        //Figure out how the voting schemas will work. It needs to work dependency-injection style (being able to swap different kinds). (I am referring to VotinUnit). 
        //History
        public List<Proposal> Proposals { get; set; }
        public List<IDictamen> Dictamens { get; set; }




    }
}
