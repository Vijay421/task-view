using WebApi.Controllers;
using WebApi.Services;

namespace WebApiTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var authController = new WebApi.Controllers.AuthenticationController(null);

        // Act

        // Assert
        Assert.Equal(1 + 1, 2);
    }
}
