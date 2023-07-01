using System;
using System.Collections.Generic;

namespace PluginDemocracy.Models
{
	public abstract class Member
	{
		Guid Guid { get; }
		public string Name { get; set; }
		public string Address { get; set; }
		public List<ICommunity> Communities { get; set; }

		public Member()
		{
			Guid = Guid.NewGuid();
		}

		public bool Vote(Proposal proposal, bool voteValue)
		{
			Vote vote = new Vote(this.Guid, voteValue);
			return proposal.RecordVote(this, vote);
		}
	}
}