using System;
using System.Threading.Tasks;
using AutoFixture;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Models;
using UrlShortener.Models.Repositories;
using UrlShortener.Services;
using UrlShortener.Utilities;

namespace UrlShortener.UnitTests.Services
{
    [TestClass]
    public class UrlShortenerServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private MockRepository _mockRepository;
        private Mock<ICosmosDbRepository> _cosmosDbRepositoryMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        
        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _cosmosDbRepositoryMock = _mockRepository.Create<ICosmosDbRepository>();
            _contextAccessorMock = _mockRepository.Create<IHttpContextAccessor>();
        }
        
        [TestMethod]
        [DataRow("https://www.wikipedia.org/")]
        [DataRow("http://www.wikipedia.org/")]
        [DataRow("https://en.wikipedia.org/wiki/Main_Page")]
        [DataRow("www.wikipeida.org")]
        public async Task CreateShortUrl_DataRow_Successful(string uri)
        {
            // Arrange
            var urlToShortenItem = new UrlToShorten
            {
                LongUrl = uri
            };
            
            _cosmosDbRepositoryMock.Setup(x => x.CreateShortUrl(It.IsAny<UrlInformation>()))
                .Returns(Task.FromResult(true))
                .Verifiable();
            
            MockHttpAccessorContext();

            // Act
            var sut = CreateUrlShortenerServiceSut();
            var result = await sut.CreateShortUrl(urlToShortenItem);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ShortUrlResponse>();
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateShortUrl_AnyInput_ArgumentException()
        {
            // Arrange
            
            // Act
            var sut = CreateUrlShortenerServiceSut();
            var result = await sut.CreateShortUrl(_fixture.Create<UrlToShorten>());
            
            // Assert
            Assert.Fail();
        }
        
        [TestMethod]
        public async Task CreateShortUrl_AnyInput_Fails()
        {
            // Arrange
            var urlToShortenItem = new UrlToShorten
            {
                LongUrl = "https://www.wikipedia.org/"
            };
            
            _cosmosDbRepositoryMock.Setup(x => x.CreateShortUrl(It.IsAny<UrlInformation>()))
                .Returns(Task.FromResult(false))
                .Verifiable();
            
            MockHttpAccessorContext();

            // Act
            var sut = CreateUrlShortenerServiceSut();
            var result = await sut.CreateShortUrl(urlToShortenItem);
            
            // Assert
            result.Should().BeNull();
        }

        private void MockHttpAccessorContext()
        {
            _contextAccessorMock.Setup(x => x.HttpContext.Request.Scheme)
                .Returns("http://localhost")
                .Verifiable();

            _contextAccessorMock.Setup(x => x.HttpContext.Request.Host)
                .Returns(new HostString("5000"))
                .Verifiable();
        }

        [TestMethod]
        public async Task GetShortUrl_AnyInput_Successful()
        {
            // Arrange
            _cosmosDbRepositoryMock.Setup(x => x.GetShortUrl(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<UrlInformation>())).Verifiable();
            
            // Act
            var sut = CreateUrlShortenerServiceSut();
            var result = await sut.GetShortUrl(_fixture.Create<string>());
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
        }
        
        [TestMethod]
        public async Task GetShortUrl_AnyInput_SuccessfulNull()
        {
            // Arrange
            _cosmosDbRepositoryMock.Setup(x => x.GetShortUrl(It.IsAny<string>()))
                .Returns(Task.FromResult((UrlInformation) null)).Verifiable();
            
            // Act
            var sut = CreateUrlShortenerServiceSut();
            var result = await sut.GetShortUrl(_fixture.Create<string>());
            
            // Assert
            result.Should().BeNull();
        }
        

        private UrlShortenerService CreateUrlShortenerServiceSut()
        {
            return new UrlShortenerService(_cosmosDbRepositoryMock.Object, new Random(), new Clock(),
               _contextAccessorMock.Object);
        }
    }
}