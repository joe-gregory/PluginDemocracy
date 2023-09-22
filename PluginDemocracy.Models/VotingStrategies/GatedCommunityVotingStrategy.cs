namespace PluginDemocracy.Models
{
    /// <summary>
    /// This voting strategy represents a Gated Community where each owner(s) has a vote.
    /// There are 2 ways to count votes depending on how partial Home owners are treated.
    /// In one scenario, Homes can only represent a whole vote. For example, if 60% of owners
    /// of that home vote in favor of a Proposal, then that entire Home's vote is in favor. 
    /// The second alternative is that fractional votes of homes are counted as fractional votes
    /// meaning that in the previous example, 60% of the Home vote would be in favor and the 
    /// other 40% against would count against. 
    /// For now fractional votes will count so a Home can vote "2 ways" but in the future might 
    /// allow different strategy implementations  
    /// </summary>
    public class GatedCommunityVotingStrategy : IVotingStrategy
    {
        public Type AppliesTo => typeof(GatedCommunity);

        public MultilingualString Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MultilingualString Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Dictionary<BaseCitizen, int> ReturnCitizensVotingValue(Community community)
        {
            var gatedCommunity = (GatedCommunity)community;
            var citizensVotingValue = new Dictionary<BaseCitizen, int>();
            //foreach (var home in gatedCommunity.Homes)
            //{
            //    foreach (var owner in home.Owners)
            //    {
            //        if (citizensVotingValue.ContainsKey(owner))
            //        {
            //            citizensVotingValue[owner] += home.Percentage;
            //        }
            //        else
            //        {
            //            citizensVotingValue.Add(owner, home.Percentage);
            //        }
            //    }
            //}
            return citizensVotingValue;
        }
    }
}
