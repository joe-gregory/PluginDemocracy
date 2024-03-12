using System.Globalization;

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
            Community lvl_0_Community = new()
            {
                CanHaveHomes = false,
                CanHaveNonResidentialCitizens = true,

            };

            Community lvl_1_communityCitizen = new()
            {
                CanHaveHomes = false,
                CanHaveNonResidentialCitizens = true,
            };
            User lvl_1_userCitizen = UserFactory.GenerateUser();

            Community lvl_2_grandchildCommunityCitizen = new();
            User lvl_2_grandchildUserCitizen = UserFactory.GenerateUser();

            //Act - Adding citizens
            lvl_0_Community.AddNonResidentialCitizen(lvl_1_communityCitizen);
            lvl_0_Community.AddNonResidentialCitizen(lvl_1_userCitizen);

            lvl_1_communityCitizen.AddNonResidentialCitizen(lvl_2_grandchildCommunityCitizen);
            lvl_1_communityCitizen.AddNonResidentialCitizen(lvl_2_grandchildUserCitizen);

            //Assert - Adding citizens
            Assert.Contains(lvl_1_communityCitizen, lvl_0_Community.Citizens);
            Assert.Contains(lvl_1_userCitizen, lvl_0_Community.Citizens);
            Assert.Contains(lvl_0_Community, lvl_1_communityCitizen.Citizenships);
            Assert.Contains(lvl_0_Community, lvl_1_userCitizen.Citizenships);

            Assert.Contains(lvl_2_grandchildCommunityCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_2_grandchildUserCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildUserCitizen.Citizenships);

            Assert.Equal(2, lvl_0_Community.Citizens.Count);
            Assert.Single(lvl_1_communityCitizen.Citizenships);
            Assert.Single(lvl_1_userCitizen.Citizenships);
            Assert.Equal(2, lvl_1_communityCitizen.Citizens.Count);
            Assert.Single(lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Single(lvl_2_grandchildUserCitizen.Citizenships);

            //Act - Removing citizenCommunity
            lvl_0_Community.RemoveNonResidentialCitizen(lvl_1_communityCitizen);
            //Assert - Removing citizenCommunity
            Assert.DoesNotContain(lvl_1_communityCitizen, lvl_0_Community.Citizens);
            Assert.DoesNotContain(lvl_0_Community, lvl_1_communityCitizen.Citizenships);
            Assert.Empty(lvl_1_communityCitizen.Citizenships);
            //Assert citizenUser is still part of parentCommunity
            Assert.Contains(lvl_1_userCitizen, lvl_0_Community.Citizens);
            Assert.Single(lvl_0_Community.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(lvl_2_grandchildCommunityCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_2_grandchildUserCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildUserCitizen.Citizenships);

            //Act - removing citizenUser
            lvl_0_Community.RemoveNonResidentialCitizen(lvl_1_userCitizen);
            //Assert - removing citizenUser
            Assert.DoesNotContain(lvl_1_userCitizen, lvl_0_Community.Citizens);
            Assert.DoesNotContain(lvl_0_Community, lvl_1_userCitizen.Citizenships);
            Assert.Empty(lvl_1_userCitizen.Citizenships);
            Assert.Empty(lvl_0_Community.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(lvl_2_grandchildCommunityCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_2_grandchildUserCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildUserCitizen.Citizenships);

            //Act - removing grandchildCitizenCommunity
            lvl_1_communityCitizen.RemoveNonResidentialCitizen(lvl_2_grandchildCommunityCitizen);
            //Assert - removing grandchildCitizenCommunity
            Assert.DoesNotContain(lvl_2_grandchildCommunityCitizen, lvl_1_communityCitizen.Citizens);
            Assert.DoesNotContain(lvl_1_communityCitizen, lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Empty(lvl_2_grandchildCommunityCitizen.Citizenships);
            Assert.Single(lvl_1_communityCitizen.Citizens); //only grandchildCitizenUser should remain

            //Assert no changes have been made to the citizenship of grandchildCitizenUser
            Assert.Contains(lvl_2_grandchildUserCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_1_communityCitizen, lvl_2_grandchildUserCitizen.Citizenships);

            //Act - Add grandchildCitizenCommunity back to citizenCommunity and citizenCommunity back to parentCommunity and remove in reverse: grandchildCitizenUser first
            lvl_1_communityCitizen.AddNonResidentialCitizen(lvl_2_grandchildCommunityCitizen);
            lvl_0_Community.AddNonResidentialCitizen(lvl_1_communityCitizen);
            lvl_0_Community.AddNonResidentialCitizen(lvl_1_userCitizen);
            lvl_1_communityCitizen.RemoveNonResidentialCitizen(lvl_2_grandchildCommunityCitizen);

            //Assert no other changes were made
            Assert.Contains(lvl_2_grandchildUserCitizen, lvl_1_communityCitizen.Citizens);
            Assert.Contains(lvl_1_communityCitizen, lvl_0_Community.Citizens);
            Assert.Contains(lvl_1_userCitizen, lvl_0_Community.Citizens);
            Assert.Contains(lvl_0_Community, lvl_1_communityCitizen.Citizenships);
            Assert.Contains(lvl_0_Community, lvl_1_userCitizen.Citizenships);
            Assert.Single(lvl_1_communityCitizen.Citizens);
            Assert.Empty(lvl_2_grandchildCommunityCitizen.Citizenships);
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
                //Name = "Home"
            };
            User owner = UserFactory.GenerateUser();
            owner.FirstName = "Owner";

            User resident = UserFactory.GenerateUser();
            resident.FirstName = "Resident";

            //Act
            gatedCommunity.AddHome(home);
            home.AddOwner(owner, 100);
            home.AddResident(resident);
            home.AddResident(owner);

            //Assert
            Assert.Contains(owner, home.OwnersWithOwnership.Keys);
            Assert.Contains(owner, home.Citizens);
            Assert.Contains(resident, home.Citizens);
            Assert.Contains(owner, gatedCommunity.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

            //Assert.Contains(home, owner.Citizenships);
            Assert.Contains(gatedCommunity, owner.Citizenships);
            //Assert.Contains(home, resident.Citizenships);
            Assert.Contains(gatedCommunity, resident.Citizenships);
            
            //Act
            //Removing owner from owners list should still show up on citizens because it is on the list of residents
            home.RemoveOwner(owner);

            //Assert
            Assert.DoesNotContain(owner, home.OwnersWithOwnership.Keys);
            Assert.Contains(owner, home.Citizens);
            Assert.Contains(owner, home.Residents);
            Assert.Contains(owner, gatedCommunity.Citizens);
            Assert.Contains(resident, home.Residents);
            Assert.Contains(resident, home.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

            //Act
            home.RemoveResident(owner);

            //Assert
            Assert.DoesNotContain(owner, home.OwnersWithOwnership.Keys);
            Assert.DoesNotContain(owner, home.Residents);
            Assert.DoesNotContain(owner, home.Citizens);
            Assert.DoesNotContain(owner, gatedCommunity.Citizens);
            Assert.Contains(resident, home.Residents);
            Assert.Contains(resident, home.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

            //Act
            home.RemoveResident(resident);

            //Assert
            Assert.DoesNotContain(owner, home.OwnersWithOwnership.Keys);
            Assert.DoesNotContain(owner, home.Residents);
            Assert.DoesNotContain(owner, home.Citizens);
            Assert.DoesNotContain(owner, gatedCommunity.Citizens);
            Assert.DoesNotContain(resident, home.Residents);
            Assert.DoesNotContain(resident, home.Citizens);
            Assert.DoesNotContain(resident, gatedCommunity.Citizens);

            //Assert.DoesNotContain(home, owner.Citizenships);
            Assert.DoesNotContain(gatedCommunity, owner.Citizenships);
            //Assert.DoesNotContain(home, resident.Citizenships);
            Assert.DoesNotContain(gatedCommunity, resident.Citizenships);
        }
    }
}