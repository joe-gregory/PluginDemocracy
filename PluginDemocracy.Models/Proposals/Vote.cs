using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// This class represents a vote
    /// </summary>
    public class Vote
    {
        public int Id { get; set; }
        public Proposal? Proposal { get; private set; }
        [NotMapped]
        public BaseCitizen Citizen { get 
            { 
                if(_userCitizen != null) return _userCitizen;
                else if(_communityCitizen != null) return _communityCitizen;
                else if(_homeCitizen != null) return _homeCitizen;
                else throw new Exception("Citizen is neither User nor Community");
            } 
        }
        public User? _userCitizen { get; }
        public HOACommunity? _communityCitizen { get; }
        public Home? _homeCitizen { get; }
        public bool InFavor { get; private set; }
        public double VoteWeight { get; private set; }
        public double VoteValueInFavor { get; private set; }
        public double VoteValueAgainst { get; private set; }
        public DateTime Date { get; private set; }
        protected Vote()
        {
            _communityCitizen = new();
        }
        /// <summary>
        /// This is the default constructor. The other is only for use by the PropagatedVoteDictamen class
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="citizen"></param>
        /// <param name="inFavor"></param>
        public Vote(Proposal proposal, BaseCitizen citizen, bool inFavor)
        {
            Proposal = proposal;
            if(citizen is User userCitizen)
            {
                _userCitizen = userCitizen;
            }
            else if(citizen is HOACommunity communityCitizen)
            {
                _communityCitizen = communityCitizen;
            }
            else if(citizen is Home homeCitizen)
            {
                _homeCitizen = homeCitizen;
            }
            InFavor = inFavor;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            if (Proposal.VotingWeights.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.VotingWeights[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
            if (InFavor == true) VoteValueInFavor = VoteWeight;
            else VoteValueAgainst = VoteWeight;
            Date = DateTime.UtcNow;
        }
        /// <summary>
        /// This constructor is used for when PropagatedVoteDictamen needs to add a vote to the child proposal with the same date and inFavor
        /// </summary>
        /// <param name="proposal"></param>
        /// <param name="citizen"></param>
        /// <param name="inFavor"></param>
        /// <param name="date"></param>
        public Vote(Proposal proposal, BaseCitizen citizen, bool inFavor, DateTime date)
        {
            Proposal = proposal;
            if (citizen is User userCitizen)
            {
                _userCitizen = userCitizen;
            }
            else if (citizen is HOACommunity communityCitizen)
            {
                _communityCitizen = communityCitizen;
            }
            InFavor = inFavor;
            VoteValueAgainst = 0;
            VoteValueInFavor = 0;
            if (Proposal.VotingWeights.ContainsKey(Citizen))
            {
                VoteWeight = Proposal.VotingWeights[Citizen];
            }
            else
            {
                VoteWeight = 0;
            }
            if (InFavor == true) VoteValueInFavor = VoteWeight;
            else VoteValueAgainst = VoteWeight;
            Date = date;
        }
    }
}