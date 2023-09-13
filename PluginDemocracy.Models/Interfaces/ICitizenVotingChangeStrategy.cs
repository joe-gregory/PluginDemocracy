using PluginDemocracy.Models;
using System;
using System.Collections.Generic;

public interface ICitizenVotingChangeStrategy
{
	public bool CanVote(ICitizen citizen, Proposal proposal);
}
