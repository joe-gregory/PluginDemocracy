using PluginDemocracy.Models;
using System;
using System.Collections.Generic;

public interface ICommunity
{
    public IVotingEligibilitySchema VotingEligibilitySchema { get; set; }
    public IVotingChangeSchema VotingChangeSchema { get; set; }
    public IProposalPassingSchema ProposalPassingSchema { get; set; }
    public IProposalOpenStatusSchema ProposalOpenStatusSchema { get; set; }
    public IDictamenValiditySchema DictamenValiditySchema { get; set; }
    
    public string GoogleMapsIFrame { get; set; }
    public List<Member> Members { get; set; }
    public Dictionary<Member, int> VotesWeights { get; set; }
    public List<Proposal> Proposals { get; set; }
}
