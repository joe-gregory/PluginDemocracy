using System;

/// <summary>
/// Summary description for Vote
/// </summary>
internal class Vote
{
	readonly Guid CitizenGuid;
	readonly DateTime When;
	readonly bool? Vote;

	public Vote(Guid citizenGuid, bool vote)
	{
		CitizenGuid = citizenGuid;
		Vote = vote;
		When = DateTime.UtcNow;
	}
}
