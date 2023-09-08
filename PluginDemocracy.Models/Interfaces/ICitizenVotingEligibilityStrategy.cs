using PluginDemocracy.Models;
using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public interface ICitizenVotingEligibilityStrategy
{
	bool CanVote(Citizen citizen, Proposal proposal);
}
