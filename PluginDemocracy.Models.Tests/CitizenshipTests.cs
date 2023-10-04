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
        public void AddingAndRemovingCitizensCommunity()
        {
            //Arrange
            Community parentCommunity = new();

            Community citizenCommunity = new();
            User citizenUser = new();

            Community grandchildCitizenCommunity = new();
            User grandchildCitizenUser = new();

            //Act - Adding citizens
            parentCommunity.AddCitizen(citizenCommunity);
            parentCommunity.AddCitizen(citizenUser);

            citizenCommunity.AddCitizen(grandchildCitizenCommunity);
            citizenCommunity.AddCitizen(grandchildCitizenUser);

            //Assert - Adding citizens
            Assert.Contains(citizenCommunity, parentCommunity.Citizens);
            Assert.Contains(citizenUser, parentCommunity.Citizens);
            Assert.Contains(parentCommunity, citizenCommunity.Citizenships);
            Assert.Contains(parentCommunity, citizenUser.Citizenships);

            Assert.Contains(grandchildCitizenCommunity, citizenCommunity.Citizens);
            Assert.Contains(grandchildCitizenUser, citizenCommunity.Citizens);
            Assert.Contains(citizenCommunity, grandchildCitizenCommunity.Citizenships);
            Assert.Contains(citizenCommunity, grandchildCitizenUser.Citizenships);

            Assert.Equal(2, parentCommunity.Citizens.Count);
            Assert.Single(citizenCommunity.Citizenships);
            Assert.Single(citizenUser.Citizenships);
            Assert.Equal(2, citizenCommunity.Citizens.Count);
            Assert.Single(grandchildCitizenCommunity.Citizenships);
            Assert.Single(grandchildCitizenUser.Citizenships);

            //Act - Removing citizenCommunity
            parentCommunity.RemoveCitizen(citizenCommunity);
            //Assert - Removing citizenCommunity
            Assert.DoesNotContain(citizenCommunity, parentCommunity.Citizens);
            Assert.DoesNotContain(parentCommunity, citizenCommunity.Citizenships);
            Assert.Empty(citizenCommunity.Citizenships);
            //Assert citizenUser is still part of parentCommunity
            Assert.Contains(citizenUser, parentCommunity.Citizens);
            Assert.Single(parentCommunity.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(grandchildCitizenCommunity, citizenCommunity.Citizens);
            Assert.Contains(grandchildCitizenUser, citizenCommunity.Citizens);
            Assert.Contains(citizenCommunity, grandchildCitizenCommunity.Citizenships);
            Assert.Contains(citizenCommunity, grandchildCitizenUser.Citizenships);

            //Act - removing citizenUser
            parentCommunity.RemoveCitizen(citizenUser);
            //Assert - removing citizenUser
            Assert.DoesNotContain(citizenUser, parentCommunity.Citizens);
            Assert.DoesNotContain(parentCommunity, citizenUser.Citizenships);
            Assert.Empty(citizenUser.Citizenships);
            Assert.Empty(parentCommunity.Citizens);
            //Assert no changes have been made to the citizens of citizenCommunity
            Assert.Contains(grandchildCitizenCommunity, citizenCommunity.Citizens);
            Assert.Contains(grandchildCitizenUser, citizenCommunity.Citizens);
            Assert.Contains(citizenCommunity, grandchildCitizenCommunity.Citizenships);
            Assert.Contains(citizenCommunity, grandchildCitizenUser.Citizenships);

            //Act - removing grandchildCitizenCommunity
            citizenCommunity.RemoveCitizen(grandchildCitizenCommunity);
            //Assert - removing grandchildCitizenCommunity
            Assert.DoesNotContain(grandchildCitizenCommunity, citizenCommunity.Citizens);
            Assert.DoesNotContain(citizenCommunity, grandchildCitizenCommunity.Citizenships);
            Assert.Empty(grandchildCitizenCommunity.Citizenships);
            Assert.Single(citizenCommunity.Citizens); //only grandchildCitizenUser should remain

            //Assert no changes have been made to the citizenship of grandchildCitizenUser
            Assert.Contains(grandchildCitizenUser, citizenCommunity.Citizens);
            Assert.Contains(citizenCommunity, grandchildCitizenUser.Citizenships);

            //Act - Add grandchildCitizenCommunity back to citizenCommunity and citizenCommunity back to parentCommunity and remove in reverse: grandchildCitizenUser first
            citizenCommunity.AddCitizen(grandchildCitizenCommunity);
            parentCommunity.AddCitizen(citizenCommunity);
            parentCommunity.AddCitizen(citizenUser);
            citizenCommunity.RemoveCitizen(grandchildCitizenCommunity);

            //Assert no other changes were made
            Assert.Contains(grandchildCitizenUser, citizenCommunity.Citizens);
            Assert.Contains(citizenCommunity, parentCommunity.Citizens);
            Assert.Contains(citizenUser, parentCommunity.Citizens);
            Assert.Contains(parentCommunity, citizenCommunity.Citizenships);
            Assert.Contains(parentCommunity, citizenUser.Citizenships);
            Assert.Single(citizenCommunity.Citizens);
            Assert.Empty(grandchildCitizenCommunity.Citizenships);
        }
        [Fact]
        public void AddingAndRemovingCitizensGatedCommunityAndHome()
        {
            //Arrange
            GatedCommunity gatedCommunity = new() 
            {
                Name = "Gated Community",
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
            gatedCommunity.Homes.Add(home);
            gatedCommunity.AddOwnerToHome(home, owner, 100);
            gatedCommunity.AddResidentToHome(home, owner);
            gatedCommunity.AddResidentToHome(home, resident);

            //Assert
            Assert.Contains(owner, home.Owners.Keys);
            Assert.Contains(owner, home.Citizens);
            Assert.Contains(owner, gatedCommunity.Citizens);

            Assert.Contains(resident, home.Citizens);
            Assert.Contains(resident, gatedCommunity.Citizens);

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