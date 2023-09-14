using PluginDemocracy.Models;
using System;

/// <summary>
/// This class represents a vote
/// </summary>
public class Vote
{
	readonly BaseCitizen Citizen;
	readonly bool Value;
    readonly DateTime Date;
    public Vote(BaseCitizen citizen, bool value)
    {
        Citizen = citizen;
        Value = value;
        Date = DateTime.Now;
    }
}
