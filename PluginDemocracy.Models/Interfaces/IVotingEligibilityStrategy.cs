using PluginDemocracy.Models;
using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public interface IVotingEligibilityStrategy
{
	bool CanVote(AbstractCitizen citizen);
}
