using PluginDemocracy.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class DictamenProposalCreator : BaseDictamen, IProposalAuthor, IProposalStrategies
    {
        /// <summary>
        /// Strategies about the behavior of the proposal
        /// </summary>
        public IProposalPassStrategy? ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy? ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy? CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy? CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy? CommunitysCitizensVotingWeightsStrategy { get; set; }

        public Proposal? Proposal { get; private set; }

        public DictamenProposalCreator(IProposalAuthor origin) : base(origin)
        {
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public void CreateProposal()
        {
            Proposal = new Proposal(this);
        }

        public void RemoveProposal()
        {
            throw new NotImplementedException();
        }
    }
}
