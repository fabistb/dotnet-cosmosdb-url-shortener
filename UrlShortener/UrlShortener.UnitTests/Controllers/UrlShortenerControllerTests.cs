using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Controllers;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.UnitTests.Controllers
{
    [TestClass]
    public class UrlShortenerControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private MockRepository _mockRepository;
        private Mock<IUrlShortenerService> _urlShortenerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _urlShortenerMock = _mockRepository.Create<IUrlShortenerService>();
        }

        [TestMethod]
        public async Task CreateShortUrl_AnyInput_Successful()
        {
            // Arrange
            _urlShortenerMock.Setup(x => x.CreateShortUrl(It.IsAny<UrlToShorten>()))
                .Returns(Task.FromResult(_fixture.Create<ShortUrlResponse>()))
                .Verifiable();

            // Act
            var sut = CreateUrlShortenerControllerSut();
            var result = await sut.CreateShortUrl(_fixture.Create<UrlToShorten>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task CreateShortUrl_AnyInput_BadRequest()
        {
            // Arrange
            _urlShortenerMock.Setup(x => x.CreateShortUrl(It.IsAny<UrlToShorten>()))
                .Returns(Task.FromResult((ShortUrlResponse) null)).Verifiable();

            // Act
            var sut = CreateUrlShortenerControllerSut();
            var result = await sut.CreateShortUrl(_fixture.Create<UrlToShorten>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task RedirectToUrl_AnyInput_Successful()
        {
            // Arrange
            _urlShortenerMock.Setup(x => x.GetShortUrl(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<string>()))
                .Verifiable();

            // Act
            var sut = CreateUrlShortenerControllerSut();
            var result = await sut.RedirectToUrl(_fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectResult>();
        }

        [TestMethod]
        public async Task RedirectToUrl_AnyInput_NotFound()
        {
            // Arrange
            _urlShortenerMock.Setup(x => x.GetShortUrl(It.IsAny<string>()))
                .Returns(Task.FromResult((string) null))
                .Verifiable();

            // Act
            var sut = CreateUrlShortenerControllerSut();
            var result = await sut.RedirectToUrl(_fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }


        private UrlShortenerController CreateUrlShortenerControllerSut()
        {
            return new UrlShortenerController(new NullLogger<BaseController>(), _urlShortenerMock.Object);
        }
    }
}