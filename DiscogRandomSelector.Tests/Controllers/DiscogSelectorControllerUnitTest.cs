using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

using discogRandomSelector.Controllers;
using discogRandomSelector.Models;
using discogRandomSelector.Services;

namespace discogRandomSelector.Tests
{
    public class DiscogSelectorControllerUnitTest
    {


        [Fact]
        public async void Request_AboveMaximumLimitNumber_IsInvalid()
        {
            // Arrange
            var selectorServiceMock = new Mock<ISelectorService>();
            var discogSelectorController = new DiscogSelectorController(selectorServiceMock.Object);

            // Act
            var response = await discogSelectorController.Get(6);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);

        }

        [Fact]
        public async void Request_BelowMinimumLimitNumber_IsInvalid()
        {
            // Arrange
            var selectorServiceMock = new Mock<ISelectorService>();
            var discogSelectorController = new DiscogSelectorController(selectorServiceMock.Object);

            // Act
            var response = await discogSelectorController.Get(0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async void Request_AllowedNumber_IsValid()
        {
            // Arrange
            var selectorServiceMock = new Mock<ISelectorService>();
            var discogSelectorController = new DiscogSelectorController(selectorServiceMock.Object);

            selectorServiceMock.Setup(x => x.GetTotalItems()).Returns(Task.FromResult(100));
            selectorServiceMock.Setup(x => x.GetItem(It.IsAny<int>())).Returns(Task.FromResult(new Release()));

            // Act
            var response = await discogSelectorController.Get(3);

            // Assert
            Assert.IsType<OkObjectResult>(response);

        }

        [Fact]
        public async void NumberOfReturnedReleases_Matches_RequestedNumber()
        {
            //Arrange
            var selectorServiceMock = new Mock<ISelectorService>();
            var discogSelectorController = new DiscogSelectorController(selectorServiceMock.Object);

            selectorServiceMock.Setup(x => x.GetTotalItems()).Returns(Task.FromResult(100));
            selectorServiceMock.Setup(x => x.GetItem(It.IsAny<int>())).Returns(Task.FromResult(new Release()));

            // Act - Get 1
            var responseCallWith1 = await discogSelectorController.Get(1);

            var okResultCallWith1 = responseCallWith1 as ObjectResult;

            // Assert - Get 1
            Assert.NotNull(okResultCallWith1);
            Assert.IsType<List<Release>>(okResultCallWith1.Value);            
            
            List<Release> releasesCallWith1 = okResultCallWith1.Value as List<Release>;
            Assert.Single(releasesCallWith1);

            //Act - Get 4
            var responseCallWith4 = await discogSelectorController.Get(4);

            var okResultCallWith4 = responseCallWith4 as ObjectResult;

            // Assert - Get 4
            Assert.NotNull(okResultCallWith4);
            Assert.IsType<List<Release>>(okResultCallWith4.Value);            
            
            var releasesCallWith4 = okResultCallWith4.Value as List<Release>;
            Assert.Equal(4, releasesCallWith4.Count);

            //Act - Get 5
            var responseCallWith5 = await discogSelectorController.Get(5);

            var okResultCallWith5 = responseCallWith5 as ObjectResult;

            // Assert - Get 5
            Assert.NotNull(okResultCallWith5);
            Assert.IsType<List<Release>>(okResultCallWith5.Value);            
            
            var releasesCallWith5 = okResultCallWith5.Value as List<Release>;
            Assert.Equal(5, releasesCallWith5.Count);

        }
    }
}