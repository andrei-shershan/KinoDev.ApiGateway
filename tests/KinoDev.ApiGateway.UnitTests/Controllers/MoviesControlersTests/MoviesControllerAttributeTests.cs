using KinoDev.ApiGateway.WebApi.Controllers;
using KinoDev.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Reflection;

namespace KinoDev.ApiGateway.UnitTests.Controllers.MoviesControlersTests
{
    public class MoviesControllerAttributeTests
    {
        [Fact]
        public void Controller_HasCorrectAttributes()
        {
            // Arrange
            var controllerType = typeof(MoviesController);
            
            // Act & Assert
            var routeAttribute = controllerType.GetCustomAttribute<RouteAttribute>();
            Assert.NotNull(routeAttribute);
            Assert.Equal("api/[controller]", routeAttribute.Template);
            
            var apiControllerAttribute = controllerType.GetCustomAttribute<ApiControllerAttribute>();
            Assert.NotNull(apiControllerAttribute);
            
            var authorizeAttribute = controllerType.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(authorizeAttribute);
            Assert.Equal($"{Roles.Admin}", authorizeAttribute.Roles);
        }
        
        [Fact]
        public void GetShowingMoviesAsync_HasCorrectAttributes()
        {
            // Arrange
            var methodInfo = typeof(MoviesController).GetMethod(nameof(MoviesController.GetShowingMoviesAsync));
            Assert.NotNull(methodInfo);
            
            // Act & Assert
            var httpGetAttribute = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            Assert.NotNull(httpGetAttribute);
            Assert.Equal("showing", httpGetAttribute.Template);
            
            var allowAnonymousAttribute = methodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
            Assert.NotNull(allowAnonymousAttribute);
            
            var outputCacheAttribute = methodInfo.GetCustomAttribute<OutputCacheAttribute>();
            Assert.NotNull(outputCacheAttribute);
            Assert.Equal(60, outputCacheAttribute.Duration);
            Assert.Equal(new[] { "date" }, outputCacheAttribute.VaryByQueryKeys);
        }
        
        [Fact]
        public void GetMoviesAsync_HasCorrectAttributes()
        {
            // Arrange
            var methodInfo = typeof(MoviesController).GetMethod(nameof(MoviesController.GetMoviesAsync));
            Assert.NotNull(methodInfo);
            
            // Act & Assert
            var httpGetAttribute = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            Assert.NotNull(httpGetAttribute);
            Assert.Null(httpGetAttribute.Template);
        }
        
        [Fact]
        public void GetMovieByIdAsync_HasCorrectAttributes()
        {
            // Arrange
            var methodInfo = typeof(MoviesController).GetMethod(nameof(MoviesController.GetMovieByIdAsync));
            Assert.NotNull(methodInfo);
            
            // Act & Assert
            var httpGetAttribute = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            Assert.NotNull(httpGetAttribute);
            Assert.Equal("{id}", httpGetAttribute.Template);
        }
        
        [Fact]
        public void CreateMovieAsync_HasCorrectAttributes()
        {
            // Arrange
            var methodInfo = typeof(MoviesController).GetMethod(nameof(MoviesController.CreateMovieAsync));
            Assert.NotNull(methodInfo);
            
            // Act & Assert
            var httpPostAttribute = methodInfo.GetCustomAttribute<HttpPostAttribute>();
            Assert.NotNull(httpPostAttribute);
            Assert.Null(httpPostAttribute.Template);
        }
    }
}
