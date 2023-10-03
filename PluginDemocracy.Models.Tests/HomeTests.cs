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
            Home home = new();
            User owner1_60 = new();
            User owner2_60 = new();
            //Act
            home.AddOwner(owner1_60, 60);
            //Assert
            Assert.Throws<ArgumentException>(() => home.AddOwner(owner2_60, 60));
        }
    }
}
