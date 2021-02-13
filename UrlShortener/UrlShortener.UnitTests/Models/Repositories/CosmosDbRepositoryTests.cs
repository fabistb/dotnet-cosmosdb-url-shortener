using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Configuration;
using UrlShortener.Models;
using UrlShortener.Models.Repositories;

namespace UrlShortener.UnitTests.Models.Repositories
{
    [TestClass]
    public class CosmosDbRepositoryTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<CosmosClient> _cosmosClientMock;
        private Mock<Container> _cosmosContainerMock;
        private Mock<IOptions<CosmosDbOptions>> _cosmosDbOptionsMock;
        private MockRepository _mockRepository;
        private Mock<ItemResponse<UrlInformation>> _urlInformationResponseMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _cosmosClientMock = _mockRepository.Create<CosmosClient>();
            _cosmosContainerMock = _mockRepository.Create<Container>();
            _cosmosDbOptionsMock = _mockRepository.Create<IOptions<CosmosDbOptions>>();
            _urlInformationResponseMock = _mockRepository.Create<ItemResponse<UrlInformation>>();
        }

        [TestMethod]
        public async Task CreateShortUrl_AnyInput_Successful()
        {
            // Arrange
            SetupDefaultCosmosDbMocks();

            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_cosmosContainerMock.Object);

            _cosmosContainerMock.Setup(x => x.CreateItemAsync(It.IsAny<UrlInformation>(), It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_urlInformationResponseMock.Object))
                .Verifiable();

            // Act
            var sut = CreateCosmosDbRepositorySut();
            var result = await sut.CreateShortUrl(_fixture.Create<UrlInformation>());

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task CreateShortUrl_AnyInput_Fails()
        {
            // Arrange
            SetupDefaultCosmosDbMocks();

            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_cosmosContainerMock.Object);

            _cosmosContainerMock.Setup(x => x.CreateItemAsync(It.IsAny<UrlInformation>(), It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Throws(new CosmosException(_fixture.Create<string>(), HttpStatusCode.Conflict, _fixture.Create<int>(),
                    _fixture.Create<string>(), _fixture.Create<double>()))
                .Verifiable();

            // Act
            var sut = CreateCosmosDbRepositorySut();
            var result = await sut.CreateShortUrl(_fixture.Create<UrlInformation>());

            // Assert
            result.Should().BeFalse();
        }

        private void SetupDefaultCosmosDbMocks()
        {
            _cosmosDbOptionsMock.SetupGet(x => x.Value)
                .Returns(new CosmosDbOptions())
                .Verifiable();

            _urlInformationResponseMock.Setup(x => x.Resource)
                .Returns(_fixture.Create<UrlInformation>())
                .Verifiable();
        }


        [TestMethod]
        public async Task GetShortUrl_AnyInput_Successful()
        {
            // Arrange
            SetupDefaultCosmosDbMocks();

            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_cosmosContainerMock.Object);

            _cosmosContainerMock.Setup(x => x.ReadItemAsync<UrlInformation>(It.IsAny<string>(),
                    It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_urlInformationResponseMock.Object))
                .Verifiable();

            // Act
            var sut = CreateCosmosDbRepositorySut();
            var result = await sut.GetShortUrl(_fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetShortUrl_AnyInput_Null()
        {
            // Arrange
            SetupDefaultCosmosDbMocks();

            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_cosmosContainerMock.Object);

            _cosmosContainerMock.Setup(x => x.ReadItemAsync<UrlInformation>(It.IsAny<string>(),
                    It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Throws(new CosmosException(_fixture.Create<string>(), HttpStatusCode.NotFound, _fixture.Create<int>(),
                    _fixture.Create<string>(), _fixture.Create<double>()))
                .Verifiable();

            // Act
            var sut = CreateCosmosDbRepositorySut();
            var result = await sut.GetShortUrl(_fixture.Create<string>());

            // Assert
            result.Should().BeNull();
        }

        private CosmosDbRepository CreateCosmosDbRepositorySut()
        {
            return new CosmosDbRepository(_cosmosClientMock.Object, _cosmosDbOptionsMock.Object);
        }
    }
}