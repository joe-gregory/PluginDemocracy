using PluginDemocracy.Models;
using System;

/// <summary>
/// This class represents a vote
/// </summary>
public class Vote
{
	readonly ICitizen Citizen;
	readonly bool Value;
    readonly DateTime Date;
    public Vote(ICitizen citizen, bool value)
    {
        Citizen = citizen;
        Value = value;
        Date = DateTime.Now;
    }
}
