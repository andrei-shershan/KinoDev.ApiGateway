using KinoDev.ApiGateway.WebApi.Controllers;
using KinoDev.ApiGateway.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace KinoDev.ApiGateway.UnitTests.Controllers.PortalSettingsControllerTests;

public class PortalSettingsControllerTests
{
    private readonly Mock<IOptions<PortalSettings>> _portalSettingsMock;
    private readonly PortalSettings _portalSettingsValue;
    
    public PortalSettingsControllerTests()
    {
        _portalSettingsValue = new PortalSettings
        {
            GithubLink = "https://github.com/kinodev",
            LinkedinLink = "https://linkedin.com/company/kinodev"
        };
        
        _portalSettingsMock = new Mock<IOptions<PortalSettings>>();
        _portalSettingsMock.Setup(x => x.Value).Returns(_portalSettingsValue);
    }
    
    [Fact]
    public void Get_WhenSettingsExist_ReturnsOkWithSettings()
    {
        // Arrange
        var controller = new PortalSettingsController(_portalSettingsMock.Object);
        
        // Act
        var result = controller.Get();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PortalSettings>(okResult.Value);
        
        Assert.Equal(_portalSettingsValue.GithubLink, returnValue.GithubLink);
        Assert.Equal(_portalSettingsValue.LinkedinLink, returnValue.LinkedinLink);
    }
    
    [Fact]
    public void Get_WhenSettingsAreNull_ReturnsNotFound()
    {
        // Arrange
        _portalSettingsMock.Setup(x => x.Value).Returns((PortalSettings)null);
        var controller = new PortalSettingsController(_portalSettingsMock.Object);
        
        // Act
        var result = controller.Get();
        
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Portal settings not found.", notFoundResult.Value);
    }
}
