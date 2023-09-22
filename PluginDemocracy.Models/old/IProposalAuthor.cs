using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// The classes that implement this interface are able to create Proposals correctly and should be the only ones that create Proposals. 
    /// This interface is implemented by User and DictamenProposalCreator since Proposals can be created by Users and this particular type of Dictamen.
    /// </summary>
    public interface IProposalAuthor
    {
        public Proposal? Proposal { get; }

        public void CreateProposal();

        public void RemoveProposal();

    }
}
