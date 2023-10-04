namespace PluginDemocracy.Models.Tests
{
    public class ProposalTests
    {
        [Fact]
        public void PassedProposalsWithNoFractionalVoting()
        {
            //Arrange
            Community parentCommunity = new()
            {
                VotingStrategy = new CitizenVotingStrategy(),
                Name = "parentCommunity"
            };
            //parentCommunity will consist of 1 sub-community and 2 Users. The subCommunity will consist of 2 users and another sub-community with 3 users. 
            GatedCommunity childGatedCommunity = new()
            {
                Name = "childGatedCommunity"
            };

            Home home1 = new();
            //whichHome_id#_percentageOwnership
            User homeowner1_1_60 = new();
            User homeowner1_2_40 = new();

            Home home2 = new();
            User homeowner2_1_30 = new();
            User homeowner2_2_30 = new();
            User homeowner2_3_40 = new();

            Home home3 = new();
            User homeowner3_1_50 = new();
            User homeowner3_2_50 = new();

            childGatedCommunity.Homes.Add(home1);
            childGatedCommunity.Homes.Add(home2);
            childGatedCommunity.Homes.Add(home3);

            childGatedCommunity.AddOwnerToHome(home1, homeowner1_1_60, 60);
            childGatedCommunity.AddOwnerToHome(home1, homeowner1_2_40, 40);

            User resident1_1 = new();
            User resident1_2 = new();
            User resident1_3 = new();
            User resident2_1 = new();
            User resident2_2 = new();
            User resident2_3 = new();
            childGatedCommunity.AddResidentToHome(home1, resident1_1);
            childGatedCommunity.AddResidentToHome(home1, resident1_2);
            childGatedCommunity.AddResidentToHome(home1, resident1_3);
            childGatedCommunity.AddResidentToHome(home2, resident2_1);
            childGatedCommunity.AddResidentToHome(home2, resident2_2);
            childGatedCommunity.AddResidentToHome(home2, resident2_3);

            childGatedCommunity.AddOwnerToHome(home2, homeowner2_1_30, 30);
            childGatedCommunity.AddOwnerToHome(home2, homeowner2_2_30, 30);
            childGatedCommunity.AddOwnerToHome(home2, homeowner2_3_40, 40);

            childGatedCommunity.AddOwnerToHome(home3, homeowner3_1_50, 50);
            childGatedCommunity.AddOwnerToHome(home3, homeowner3_2_50, 50);

            User citizen1 = new();
            User citizen2 = new();

            //parentCommunity consists of 2 Users and 1 GatedCommunity. The GatedCommunity has 3 homes each with different levels of ownership.
            parentCommunity.AddCitizen(citizen1);
            parentCommunity.AddCitizen(citizen2);
            parentCommunity.AddCitizen(childGatedCommunity);

            //Act - Proposal
            //Make a proposal on the parent Community. Does the Proposal propagate to child communities? Does it propagate down to childGatedCommunity and to each of the homes?
            Proposal proposal = new(parentCommunity, citizen1)
            {
                Title = "title",
                Description = "description",
                Dictamen = new EmptyDictamen(),
            };
            proposal.Dictamen.Community = parentCommunity;

            Assert.Empty(parentCommunity.Proposals);
            Assert.Empty(childGatedCommunity.Proposals);
            Assert.Empty(home1.Proposals);
            Assert.Empty(home2.Proposals);
            Assert.Empty(home3.Proposals);

            parentCommunity.PublishProposal(proposal);

            //Proposal should have the same Voting Strategy has parent
            Assert.Equal(proposal.VotingStrategy, parentCommunity.VotingStrategy);
            Assert.Null(proposal.Passed);
            Assert.True(proposal.Open);

            //Assert propagation to subcommunities& that the proposal was added to parent community
            Assert.Contains(proposal, parentCommunity.Proposals);
            Assert.Single(parentCommunity.Proposals);
            Assert.Single(childGatedCommunity.Proposals);
            Assert.Single(home1.Proposals);
            Assert.Single(home2.Proposals);
            Assert.Single(home3.Proposals);

            Assert.Equal(proposal.Author, childGatedCommunity.Proposals[0].Author);
            Assert.Equal(proposal.Author, home1.Proposals[0].Author);
            Assert.Equal(proposal.Author, home2.Proposals[0].Author);
            Assert.Equal(proposal.Author, home3.Proposals[0].Author);
            Assert.Equal(proposal.ExpirationDate, childGatedCommunity.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home1.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home2.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home3.Proposals[0].ExpirationDate);

            //Voting in Parent Community
            parentCommunity.Proposals[0].Vote(citizen1, true);
            parentCommunity.Proposals[0].Vote(citizen2, false);
            //At this point, childGatedCommunity needs to vote to make the proposal true or false. Proposal should not be Passed right now (so Passed = null)
            Assert.Null(proposal.Passed);

            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote IN THE HOMES. 
            //It's 3 homes, so we need 2 Homes to vote yes.
            home1.Proposals[0].Vote(resident1_1, false);
            home1.Proposals[0].Vote(resident1_2, false);
            home1.Proposals[0].Vote(resident1_3, false);
            home2.Proposals[0].Vote(resident2_1, false);
            home2.Proposals[0].Vote(resident2_2, false);
            home2.Proposals[0].Vote(resident2_3, false);
            home1.Proposals[0].Vote(homeowner1_1_60, true);
            home2.Proposals[0].Vote(homeowner2_1_30, true);
            home2.Proposals[0].Vote(homeowner2_2_30, true);
            //With this the dictamen should pass that should make the Gated Community vote true in the parent proposal
            Assert.True(home1.Proposals[0].Passed);
            Assert.True(home2.Proposals[0].Passed);
            Assert.True(childGatedCommunity.Proposals[0].Passed);
            var vote = parentCommunity.Proposals[0]?.Votes?.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.True(vote?.InFavor ?? false);
            Assert.True(parentCommunity.Proposals[0].Passed);
        }
        [Fact]
        public void FailedProposalsWithNoFractionalVoting()
        {
            //Arrange
            Community parentCommunity = new()
            {
                VotingStrategy = new CitizenVotingStrategy(),
                Name = "parentCommunity"
            };
            //parentCommunity will consist of 1 sub-community and 2 Users. The subCommunity will consist of 2 users and another sub-community with 3 users. 
            GatedCommunity childGatedCommunity = new()
            {
                Name = "childGatedCommunity"
            };

            Home home1 = new();
            //whichHome_id#_percentageOwnership
            User homeowner1_1_60 = new();
            User homeowner1_2_40 = new();

            Home home2 = new();
            User homeowner2_1_30 = new();
            User homeowner2_2_30 = new();
            User homeowner2_3_40 = new();

            Home home3 = new();
            User homeowner3_1_50 = new();
            User homeowner3_2_50 = new();

            childGatedCommunity.Homes.Add(home1);
            childGatedCommunity.Homes.Add(home2);
            childGatedCommunity.Homes.Add(home3);

            childGatedCommunity.AddOwnerToHome(home1, homeowner1_1_60, 60);
            childGatedCommunity.AddOwnerToHome(home1, homeowner1_2_40, 40);

            User resident1_1 = new();
            User resident1_2 = new();
            User resident1_3 = new();
            User resident2_1 = new();
            User resident2_2 = new();
            User resident2_3 = new();
            childGatedCommunity.AddResidentToHome(home1, resident1_1);
            childGatedCommunity.AddResidentToHome(home1, resident1_2);
            childGatedCommunity.AddResidentToHome(home1, resident1_3);
            childGatedCommunity.AddResidentToHome(home2, resident2_1);
            childGatedCommunity.AddResidentToHome(home2, resident2_2);
            childGatedCommunity.AddResidentToHome(home2, resident2_3);

            childGatedCommunity.AddOwnerToHome(home2, homeowner2_1_30, 30);
            childGatedCommunity.AddOwnerToHome(home2, homeowner2_2_30, 30);
            childGatedCommunity.AddOwnerToHome(home2, homeowner2_3_40, 40);

            childGatedCommunity.AddOwnerToHome(home3, homeowner3_1_50, 50);
            childGatedCommunity.AddOwnerToHome(home3, homeowner3_2_50, 50);

            User citizen1 = new();
            User citizen2 = new();

            //parentCommunity consists of 2 Users and 1 GatedCommunity. The GatedCommunity has 3 homes each with different levels of ownership.
            parentCommunity.AddCitizen(citizen1);
            parentCommunity.AddCitizen(citizen2);
            parentCommunity.AddCitizen(childGatedCommunity);

            //Act - Proposal
            //Make a proposal on the parent Community. Does the Proposal propagate to child communities? Does it propagate down to childGatedCommunity and to each of the homes?
            Proposal proposal = new(parentCommunity, citizen1)
            {
                Title = "title",
                Description = "description",
                Dictamen = new EmptyDictamen(),

            };
            proposal.Dictamen.Community = parentCommunity;

            Assert.Empty(parentCommunity.Proposals);
            Assert.Empty(childGatedCommunity.Proposals);
            Assert.Empty(home1.Proposals);
            Assert.Empty(home2.Proposals);
            Assert.Empty(home3.Proposals);

            parentCommunity.PublishProposal(proposal);

            //Proposal should have the same Voting Strategy has parent
            Assert.Equal(proposal.VotingStrategy, parentCommunity.VotingStrategy);
            Assert.Null(proposal.Passed);
            Assert.True(proposal.Open);

            //Assert propagation to subcommunities& that the proposal was added to parent community
            Assert.Contains(proposal, parentCommunity.Proposals);
            Assert.Single(parentCommunity.Proposals);
            Assert.Single(childGatedCommunity.Proposals);
            Assert.Single(home1.Proposals);
            Assert.Single(home2.Proposals);
            Assert.Single(home3.Proposals);

            Assert.Equal(proposal.Author, childGatedCommunity.Proposals[0].Author);
            Assert.Equal(proposal.Author, home1.Proposals[0].Author);
            Assert.Equal(proposal.Author, home2.Proposals[0].Author);
            Assert.Equal(proposal.Author, home3.Proposals[0].Author);
            Assert.Equal(proposal.ExpirationDate, childGatedCommunity.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home1.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home2.Proposals[0].ExpirationDate);
            Assert.Equal(proposal.ExpirationDate, home3.Proposals[0].ExpirationDate);

            //Voting in Parent Community
            parentCommunity.Proposals[0].Vote(citizen1, true);
            parentCommunity.Proposals[0].Vote(citizen2, false);
            //At this point, childGatedCommunity needs to vote to make the proposal true or false. Proposal should not be Passed right now
            Assert.Null(proposal.Passed);

            //For the GatedCommunity to make a choice, it needs the owners of the homes to vote. 
            //It's 3 homes, so 2 homes need to vote no.
            home1.Proposals[0].Vote(resident1_1, true);
            home1.Proposals[0].Vote(resident1_2, true);
            home1.Proposals[0].Vote(resident1_3, true);
            home2.Proposals[0].Vote(resident2_1, true);
            home2.Proposals[0].Vote(resident2_2, true);
            home2.Proposals[0].Vote(resident2_3, true);
            home1.Proposals[0].Vote(homeowner1_1_60, false);
            home2.Proposals[0].Vote(homeowner2_1_30, false);
            home2.Proposals[0].Vote(homeowner2_2_30, false);

            Assert.False(home1.Proposals[0].Passed);
            Assert.False(home2.Proposals[0].Passed);
            //With this the dictamen should pass that should make the Gated Community vote true in the parent proposal
            Assert.False(childGatedCommunity.Proposals[0].Passed);

            var vote = parentCommunity.Proposals[0]?.Votes?.FirstOrDefault(p => p.Citizen == childGatedCommunity);
            Assert.NotNull(vote);
            Assert.NotNull(vote.InFavor);
            Assert.False(vote.InFavor);
            Assert.False(parentCommunity.Proposals[0].Passed);
        }
    }
}
