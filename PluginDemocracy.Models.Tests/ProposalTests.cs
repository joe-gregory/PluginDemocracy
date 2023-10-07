namespace PluginDemocracy.Models.Tests
{
    /// <summary>
    /// Tests a convential community of citizens (as the parent community) with one citizen being a gatedCommunity to ensure Proposal propagates. 
    /// Parent community is of NonResidentialCitizens with CanHaveHomes as false. 
    /// </summary>
    public class ProposalTests
    {
        public Community parentCommunity;
        public User citizen1;
        public User citizen2;
        public Community childGatedCommunity;
        public Home home1;
        public Home home2;
        public Home home3;
        public User homeowner1_1_60;
        public User homeowner1_2_40;
        public User resident1_1 = new();
        public User resident1_2 = new();
        public User resident1_3 = new();

        public User homeowner2_1_30;
        public User homeowner2_2_30;
        public User homeowner2_3_40;
        public User resident2_1 = new();
        public User resident2_2 = new();
        public User resident2_3 = new();

        public User homeowner3_1_50;
        public User homeowner3_2_50;

        public ProposalTests()
        {
            //Parent community and its 3 children: citizen1, citizen2 and childGatedCommunity
            parentCommunity = new()
            {
                VotingStrategy = new CitizensVotingStrategy(),
                Name = "parentCommunity",
                CanHaveHomes = false,
                CanHaveNonResidentialCitizens = true,
            };
            citizen1 = new();
            citizen2 = new();
            childGatedCommunity = new()
            {
                Name = "childGatedCommunity",
                CanHaveHomes = true,
                CanHaveNonResidentialCitizens = false,
            };
            parentCommunity.AddNonResidentialCitizen(citizen1);
            parentCommunity.AddNonResidentialCitizen(citizen2);
            parentCommunity.AddNonResidentialCitizen(childGatedCommunity);

            //home 1
            home1 = new();
            homeowner1_1_60 = new(); //whichHome_id#_percentageOwnership
            homeowner1_2_40 = new();
            childGatedCommunity.AddHome(home1);
            childGatedCommunity.AddOwnerToHome(homeowner1_1_60, 60, home1);
            childGatedCommunity.AddOwnerToHome(homeowner1_2_40, 40, home1);
            resident1_1 = new();
            resident1_2 = new();
            resident1_3 = new();
            childGatedCommunity.AddResidentToHome(resident1_1, home1);
            childGatedCommunity.AddResidentToHome(resident1_2, home1);
            childGatedCommunity.AddResidentToHome(resident1_3, home1);

            //home2
            home2 = new();
            homeowner2_1_30 = new();
            homeowner2_2_30 = new();
            homeowner2_3_40 = new();
            childGatedCommunity.AddHome(home2);
            childGatedCommunity.AddOwnerToHome(homeowner2_1_30, 30, home2);
            childGatedCommunity.AddOwnerToHome(homeowner2_2_30, 30, home2);
            childGatedCommunity.AddOwnerToHome(homeowner2_3_40, 40, home2);
            resident2_1 = new();
            resident2_2 = new();
            resident2_3 = new();
            childGatedCommunity.AddResidentToHome(resident2_1, home2);
            childGatedCommunity.AddResidentToHome(resident2_2, home2);
            childGatedCommunity.AddResidentToHome(resident2_3, home2);

            //home3
            home3 = new();
            homeowner3_1_50 = new();
            homeowner3_2_50 = new();
            childGatedCommunity.AddHome(home3);
            childGatedCommunity.AddOwnerToHome(homeowner3_1_50, 50, home3);
            childGatedCommunity.AddOwnerToHome(homeowner3_2_50, 50, home3);
        }
        public List<Proposal> CommonSetup(IVotingStrategy votingStrategy, User author, string proposalTitle, string proposalDescription)
        {
            //Arrange
            childGatedCommunity.VotingStrategy = votingStrategy;
            //This is what I am using to differentiate the proposals between different tests
            Proposal parentProposal = new(parentCommunity, author)
            {
                Title = proposalTitle,
                Description = proposalDescription,
                Dictamen = new EmptyDictamen(),
            };

            //Act
            parentCommunity.PublishProposal(parentProposal);

            //Proposal should have the same Voting Strategy as parent, it should have Passed as null and Open be set to true after publishing
            Assert.Equal(parentProposal.VotingStrategy, parentCommunity.VotingStrategy);
            Assert.Null(parentProposal.Passed);
            Assert.True(parentProposal.Open);

            //Assert propagation to subcommunities& that the proposal was added to parent community
            Assert.Contains(parentProposal, parentCommunity.Proposals);
            var childProposal = childGatedCommunity.Proposals.FirstOrDefault(p => p.Author == author);
            Assert.Contains(childProposal, childGatedCommunity.Proposals);
            Assert.Null(childProposal?.Passed);
            Assert.True(childProposal?.Open);
            Assert.Equal(parentProposal.ExpirationDate, childProposal?.ExpirationDate);

            //Voting in Parent Community
            parentProposal.Vote(citizen1, true);
            parentProposal.Vote(citizen2, false);

            //At this point, childGatedCommunity needs to vote to make the proposal true or false. Proposal should not be Passed right now (so Passed = null)
            Assert.Null(parentProposal.Passed);
            List<Proposal> proposals = new()
            {
                parentProposal
            };
            if (childProposal != null) proposals.Add(childProposal);

            return proposals;
        }

        [Fact]
        public void PassedProposalsWithNoFractionalVoting()
        {
            //Arrange
            //Options:
            IVotingStrategy votingStrategy = new HomeOwnersNonFractionalVotingStrategy();
            User author = citizen1;
            string proposalTitle = "Passed_Non_Fractional_Voting";
            string proposalDescription = "Passed_Non_Fractional_Voting";
            //Arrange:
            List<Proposal> proposals = CommonSetup(votingStrategy, author, proposalTitle, proposalDescription);
            Proposal parentProposal = proposals[0];
            Proposal childProposal = proposals[1];
            ///////////////////////////////////////////Here it should start being different between the 2 nonfractional voting tests
            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote IN THE HOMES. 
            //It's 3 homes, so we need 2 Homes to vote yes.
            childProposal?.Vote(resident1_1, false);
            childProposal?.Vote(resident1_2, false);
            childProposal?.Vote(resident1_3, false);
            childProposal?.Vote(resident2_1, false);
            childProposal?.Vote(resident2_2, false);
            childProposal?.Vote(resident2_3, false);
            //Making home 1 pass
            childProposal?.Vote(homeowner1_1_60, true);
            //making home 2 pass
            childProposal?.Vote(homeowner2_1_30, true);
            childProposal?.Vote(homeowner2_2_30, true);
            
            //With this the dictamen should pass that should make the Gated Community vote true in the parent proposal
            Assert.True(childProposal?.Passed);
            var vote = parentProposal.Votes.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.NotNull(vote);
            Assert.NotNull(vote?.InFavor);
            Assert.True(vote?.InFavor);
            Assert.True(parentProposal.Passed);
        }
        [Fact]
        public void FailedProposalsWithNoFractionalVoting()
        {
            //Arrange
            //Options:
            IVotingStrategy votingStrategy = new HomeOwnersNonFractionalVotingStrategy(); ;
            User author = citizen2;
            string proposalTitle = "Fail_Non_Fractional_Voting";
            string proposalDescription = "Fail_Non_Fractional_Voting";
            //Arrange:
            List<Proposal> proposals = CommonSetup(votingStrategy, author, proposalTitle, proposalDescription);
            Proposal parentProposal = proposals[0];
            Proposal childProposal = proposals[1];
            ///////////////////////////////////////////Here it should start being different between the 2 nonfractional voting tests
            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote. 
            //It's 3 homes, so 2 homes need to vote no.
            childProposal?.Vote(resident1_1, true);
            childProposal?.Vote(resident1_2, true);
            childProposal?.Vote(resident1_3, true);
            childProposal?.Vote(resident2_1, true);
            childProposal?.Vote(resident2_2, true);
            childProposal?.Vote(resident2_3, true);
            //Making home 1 reject
            childProposal?.Vote(homeowner1_1_60, false);
            //Making home 2 reject
            childProposal?.Vote(homeowner2_1_30, false);
            childProposal?.Vote(homeowner2_2_30, false);

            Assert.False(childProposal?.Passed);
            var vote = parentProposal.Votes.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.NotNull(vote);
            Assert.NotNull(vote?.InFavor);
            Assert.False(vote?.InFavor);
            Assert.False(parentProposal.Passed);
        }

        [Fact]
        public void PassedProposalWithFractionalVoting()
        {
            //Arrange
            //Options:
            IVotingStrategy votingStrategy = new HomeOwnersFractionalVotingStrategy();
            User author = resident1_1;
            string proposalTitle = "Passed_Fractional_Voting";
            string proposalDescription = "Passed_Fractional_Voting";
            //Arrange:
            List<Proposal> proposals = CommonSetup(votingStrategy, author, proposalTitle, proposalDescription);
            Proposal parentProposal = proposals[0];
            Proposal childProposal = proposals[1];
            ///////////////////////////////////////////Here it should start being different between the 2 nonfractional voting tests
            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote. 
            //It's 3 homes, so 2 homes need to vote no.
            childProposal?.Vote(resident1_1, false);
            childProposal?.Vote(resident1_2, false);
            childProposal?.Vote(resident1_3, false);
            childProposal?.Vote(resident2_1, false);
            childProposal?.Vote(resident2_2, false);
            childProposal?.Vote(resident2_3, false);
            //Total vote sums = 300, so I need at least 151 to make it happen. However, 
            //Making home 1 reject
            childProposal?.Vote(homeowner1_1_60, true);
            childProposal?.Vote(homeowner2_1_30, true);
            childProposal?.Vote(homeowner2_2_30, true);
            if (childProposal != null) Assert.Null(childProposal.Passed);
            else Assert.Fail("childProposal was unexpectedly null.");
            //At this point it Passed should still be null, but the next statement should make it pass
            childProposal?.Vote(homeowner3_1_50, true);

            Assert.True(childProposal?.Passed);
            var vote = parentProposal.Votes.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.NotNull(vote);
            Assert.NotNull(vote?.InFavor);
            Assert.True(vote?.InFavor);
            Assert.True(parentProposal.Passed);
        }
        [Fact]
        public void FailedProposalWithFractionalVoting()
        {
            //Arrange
            //Options:
            IVotingStrategy votingStrategy = new HomeOwnersFractionalVotingStrategy();
            User author = resident1_2;
            string proposalTitle = "Fail_Fractional_Voting";
            string proposalDescription = "Fail_Fractional_Voting";
            //Arrange:
            List<Proposal> proposals = CommonSetup(votingStrategy, author, proposalTitle, proposalDescription);
            Proposal parentProposal = proposals[0];
            Proposal childProposal = proposals[1];
            ///////////////////////////////////////////Here it should start being different between the 2 nonfractional voting tests
            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote. 
            //It's 3 homes, so 2 homes need to vote no.
            childProposal?.Vote(resident1_1, true);
            childProposal?.Vote(resident1_2, true);
            childProposal?.Vote(resident1_3, true);
            childProposal?.Vote(resident2_1, true);
            childProposal?.Vote(resident2_2, true);
            childProposal?.Vote(resident2_3, true);
            //Total vote sums = 300, so I need at least 151 to make it happen. However, 
            //Making home 1 reject
            childProposal?.Vote(homeowner1_1_60, false);
            childProposal?.Vote(homeowner2_1_30, false);
            childProposal?.Vote(homeowner2_2_30, false);
            if (childProposal != null) Assert.Null(childProposal.Passed);
            else Assert.Fail("childProposal was unexpectedly null.");
            //At this point it Passed should still be null, but the next statement should make it pass
            childProposal?.Vote(homeowner3_1_50, false);

            Assert.False(childProposal?.Passed);
            var vote = parentProposal.Votes.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.NotNull(vote);
            Assert.NotNull(vote?.InFavor);
            Assert.False(vote?.InFavor);
            Assert.False(parentProposal.Passed);
        }
    }
}
