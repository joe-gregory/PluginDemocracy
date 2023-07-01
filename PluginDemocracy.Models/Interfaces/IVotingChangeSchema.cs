using System;
using System.Collections.Generic;

public interface IVotingChangeSchema
{
	public bool CanVote(Member member, List<Vote> votes);
}
