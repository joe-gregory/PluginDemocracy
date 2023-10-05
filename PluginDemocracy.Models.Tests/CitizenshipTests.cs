namespace PluginDemocracy.Models.Tests
{
    /// <summary>
    /// This class tests adding and removing of BaseCitizens from Community. A BaseCitize can be a User or a Community.
    /// Arrange, Act, Assert
    /// </summary>
    public class CitizenshipTests
    {
        /// <summary>
        /// Adding a citizen needs to add it to both Community.Citizens and User.Citizenships. There should be no duplicates.
        /// </summary>
        [Fact]
        public void AddingAndRemovingCitizensNoHomesCommunity()
        {
            //Arrange
            Community parentCommunity = new()
            {
                CanHaveHomes = false,
                CanHaveNonResidentialCitizens = true,

            };

            Community communityCitizen = new();
            User userCitizen = new();

            Community grandchildCommunityCitizen = new();
            User grandchildUserCitizen = new();

            //Act - Adding citizens
            parentCommunity.AddNonResidentialCitizen(communityCitizen);
            parentCommunity.AddNonResidentialCitizen(userCitizen);

            communityCitizen.AddNonResidentialCitizen(grandchildCommunityCitizen);
            communityCitizen.AddNonResidentialCitizen(grandchildUserCitizen);

            //Assert - Adding citizens
            Assert.Contains(communityCitizen, parentCommunity.Citizens);
            Assert.Contains(userCitizen, parentCommunity.Citizens);
            Assert.Contains(parentCommunity, communityCitizen.Citizenships);
            Assert.Contains(parentCommunity, userCitizen.Citizenships);

            Assert.Contains(grandchildCommunityCitizen, communityCitizen.Citizens);
            Assert.Contains(grandchildUserCitizen, communityCitizen.Citizens);
            Assert.Contains(communityCitizen, grandchildCommunityCitizen.Citizenships);
            Assert.Contains(communityCitizen, grandchildUserCitizen.Citizenships);

            Assert.Equal(2, parentCommunity.Citizens.Count);
            Assert.Single(communityCitizen.Citizenships);
            Assert.Single(userCitizen.Citizenships);
            Assert.Equal(2, communityCitizen.Citizens.Count);
            Assert.Single(grandchildCommunityCitizen.Citizenships);
            Assert.Single(grandchildUserCitizen.Citizenships);

            //Act - Removing citizenCommunity
            parentCommunity.RemoveNonResidentialCitizen(communityCitizen);
            //Assert - Removing citizenCommunity
            Assert.DoesNotContain(communityCitizen, parentCommunity.Citizens);
            Assert.DoesNotContain(parentCommunity, communityCitizen.Citizenships);
            Assert.Empty(communityCitizen.Citizenships);
            //Assert citizenUser is still part of parentCommunity
            Assert.Contains(userCitizen, parentCommunity.Citizens);
            Assert.Single(parentCommunity.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(grandchildCommunityCitizen, communityCitizen.Citizens);
            Assert.Contains(grandchildUserCitizen, communityCitizen.Citizens);
            Assert.Contains(communityCitizen, grandchildCommunityCitizen.Citizenships);
            Assert.Contains(communityCitizen, grandchildUserCitizen.Citizenships);

            //Act - removing citizenUser
            parentCommunity.RemoveNonResidentialCitizen(userCitizen);
            //Assert - removing citizenUser
            Assert.DoesNotContain(userCitizen, parentCommunity.Citizens);
            Assert.DoesNotContain(parentCommunity, userCitizen.Citizenships);
            Assert.Empty(userCitizen.Citizenships);
            Assert.Empty(parentCommunity.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(grandchildCommunityCitizen, communityCitizen.Citizens);
            Assert.Contains(grandchildUserCitizen, communityCitizen.Citizens);
            Assert.Contains(communityCitizen, grandchildCommunityCitizen.Citizenships);
            Assert.Contains(communityCitizen, grandchildUserCitizen.Citizenships);

            //Act - removing grandchildCitizenCommunity
            communityCitizen.RemoveNonResidentialCitizen(grandchildCommunityCitizen);
            //Assert - removing grandchildCitizenCommunity
            Assert.DoesNotContain(grandchildCommunityCitizen, communityCitizen.Citizens);
            Assert.DoesNotContain(communityCitizen, grandchildCommunityCitizen.Citizenships);
            Assert.Empty(grandchildCommunityCitizen.Citizenships);
            Assert.Single(communityCitizen.Citizens); //only grandchildCitizenUser should remain

            //Assert no changes have been made to the citizenship of grandchildCitizenUser
            Assert.Contains(grandchildUserCitizen, communityCitizen.Citizens);
            Assert.Contains(communityCitizen, grandchildUserCitizen.Citizenships);

            //Act - Add grandchildCitizenCommunity back to citizenCommunity and citizenCommunity back to parentCommunity and remove in reverse: grandchildCitizenUser first
            communityCitizen.AddNonResidentialCitizen(grandchildCommunityCitizen);
            parentCommunity.AddNonResidentialCitizen(communityCitizen);
            parentCommunity.AddNonResidentialCitizen(userCitizen);
            communityCitizen.RemoveNonResidentialCitizen(grandchildCommunityCitizen);

            //Assert no other changes were made
            Assert.Contains(grandchildUserCitizen, communityCitizen.Citizens);
            Assert.Contains(communityCitizen, parentCommunity.Citizens);
            Assert.Contains(userCitizen, parentCommunity.Citizens);
            Assert.Contains(parentCommunity, communityCitizen.Citizenships);
            Assert.Contains(parentCommunity, userCitizen.Citizenships);
            Assert.Single(communityCitizen.Citizens);
            Assert.Empty(grandchildCommunityCitizen.Citizenships);
        }
        [Fact]
        public void AddingAndRemovingCitizensGatedCommunityAndHome()
        {
            //Arrange
            Community gatedCommunity = new()
            {
                Name = "Gated Community",
                CanHaveHomes = true,
                CanHaveNonResidentialCitizens = false
            };
            Home home = new()
            {
                Name = "Home"
            };
            User owner = new() 
            { 
                FirstName = "Owner",
            };
            User resident = new() 
            { 
                FirstName = "Resident",
            };

            //Act
            gatedCommunity.AddHome(home);
            gatedCommunity.AddOwnerToHome(home, owner, 100);
            gatedCommunity.AddResidentToHome(home, owner);
            gatedCommunity.AddResidentToHome(home, resident);

            //Assert
            Assert.Contains(owner, home.Owners.Keys);
            Assert.Contains(owner, home.Citizens);
            Assert.Contains(home, gatedCommunity.Citizens);

            Assert.Contains(resident, home.Citizens);

            Assert.Contains(home, owner.Citizenships); //
            Assert.Contains(gatedCommunity, owner.Citizenships);
            Assert.Contains(home, resident.Citizenships);
            Assert.Contains(gatedCommunity, resident.Citizenships);
            //Act
            //Removing owner from owners list should still show up on citizens because it is on the list of residents
            gatedCommunity.RemoveOwnerFromHome(home, owner);

            //Assert
            Assert.DoesNotContain(owner, home.Owners.Keys);
            Assert.Contains(owner, home.Citizens);
            Assert.Contains(owner, home.Residents);
            Assert.Contains(owner, gatedCommunity.Citizens);
            Assert.Contains(resident, home.Residents);
            Assert.Contains(resident, home.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

            //Act
            gatedCommunity.RemoveResidentFromHome(home,owner);

            //Assert
            Assert.DoesNotContain(owner, home.Owners.Keys);
            Assert.DoesNotContain(owner, home.Residents);
            Assert.DoesNotContain(owner, home.Citizens);
            Assert.DoesNotContain(owner, gatedCommunity.Citizens);
            Assert.Contains(resident, home.Residents);
            Assert.Contains(resident, home.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

            //Act
            gatedCommunity.RemoveResidentFromHome(home,resident);

            //Assert
            Assert.DoesNotContain(owner, home.Owners.Keys);
            Assert.DoesNotContain(owner, home.Residents);
            Assert.DoesNotContain(owner, home.Citizens);
            Assert.DoesNotContain(owner, gatedCommunity.Citizens);
            Assert.DoesNotContain(resident, home.Residents);
            Assert.DoesNotContain(resident, home.Citizens);
            Assert.DoesNotContain(resident, gatedCommunity.Citizens);

            Assert.DoesNotContain(home, owner.Citizenships);
            Assert.DoesNotContain(gatedCommunity, owner.Citizenships);
            Assert.DoesNotContain(home, resident.Citizenships);
            Assert.DoesNotContain(gatedCommunity, resident.Citizenships);
        }
    }
}