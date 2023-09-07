using System;
using System.Collections.Generic;

public interface IVotingChangeStrategy
{
	public bool CanVote(Member member, List<Vote> votes);
}
