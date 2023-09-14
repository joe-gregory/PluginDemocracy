using PluginDemocracy.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models
{
    public class Dictamen : IProposalAuthor, IProposalStrategies
    {
        public Guid Guid { get; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public BaseCommunity Community { get; }
        //Change this because the Origin of a Dictamen is either a Role or a Proposal
        public DictamenIssuer Origin { get; }
        public DateTime? IssueDate { get; private set; }
        public Action? Action { get; private set; }

        public Proposal? Proposal { get; set; }
        /// <summary>
        /// Strategies:
        /// </summary>
        public IProposalPassStrategy? ProposalPassStrategy { get; set; }
        public IProposalOpenStatusStrategy? ProposalOpenStatusStrategy { get; set; }
        public ICitizenVotingEligibilityStrategy? CitizenVotingEligibilityStrategy { get; set; }
        public ICitizenVotingChangeStrategy? CitizenVotingChangeStrategy { get; set; }
        public ICommunitysCitizensVotingWeightsStrategy? CommunitysCitizensVotingWeightsStrategy { get; set; }

        private Dictamen(DictamenIssuer origin)
        {
            Guid = Guid.NewGuid();
            Community = origin.Community;
            Origin = origin;
        }
        public void Issue()
        {
            if (Community.IssueDictamen(this))
            {
                IssueDate = DateTime.Now;
                Execute();
            }
        }
        /// <summary>
        /// Execute() is the actual action the Dictamen takes
        /// </summary>
        public void Execute()
        {
            if (Action == null) throw new Exception("There is no Action associated with this Dictmane");
        }

        public void CreateProposal()
        {
            throw new NotImplementedException();
        }

        public void RemoveProposal()
        {
            throw new NotImplementedException();
        }

        public abstract class DictamenIssuer
        {
            public BaseCommunity Community { get; }
            public Dictamen? DictamenToIssue { get; private set; }
            /// <summary>
            /// By having a protected constructor, only this class or derived class can create instances of DictamenIssuer. Furthermore, by having DictamenIssuer be an Abstract class, this can't 
            /// be instantiated by itself, further preventing and protecting. 
            /// </summary>
            /// <param name="community"></param>
            protected DictamenIssuer(BaseCommunity community)
            {
                Community = community;
            }
            public void CreateDictamen()
            {
                DictamenToIssue = new Dictamen(this);
            }
            public void RemoveDictamen()
            {
                DictamenToIssue = null;
            }
            public void IssueDictamen()
            {
                //Need to implement this in a way that calls the Community
                throw new NotImplementedException();
            }
        }
    }
}
