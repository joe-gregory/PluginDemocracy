namespace PluginDemocracy.Models.Tests
{
    public class HomeTests
    {
        /// <summary>
        /// Adding more than 100% (100int) total home ownership in a home should throw an exception. 
        /// </summary>
        [Fact]
        public void HomeMaxOwnershipTest()
        {
            //Arrange
            Community gatedCommunity = new()
            {
                CanHaveHomes = true,
                CanHaveNonResidentialCitizens = false,
            };
            Home home = new();
            gatedCommunity.AddHome(home);
            
            User owner1_60 = UserFactory.GenerateUser();
            User owner2_60 = UserFactory.GenerateUser();
            //Act
            home.AddOwner(owner2_60, 60);
            //Assert
            Assert.Throws<ArgumentException>(() => home.AddOwner(owner2_60, 60));
        }
    }
}
