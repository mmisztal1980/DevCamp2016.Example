using Example.Api.Controllers;
using Xunit;

namespace Example.Api.UnitTests
{
    public class ValuesControllerTests
    {
        [Fact]
        public void Get_Id_ValueString()
        {
            // Arrange
            var ctrl = new ValuesController();

            // Act
            var response = ctrl.Get(6);

            // Assert
            Assert.Equal("value", response);
        }
    }
}
