using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class AutenticacaoEndpointTests
{
    [Fact]
    public void Ctor_WhenMissingSecret_Throws()
    {
        var db = new Mock<IDbConnection>().Object;
        var objectStorage = new FakeObjectStorageService();
        var messaging = new FakeMessagingService();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        Assert.Throws<ArgumentException>(() => new VideoEndpoint(db, objectStorage, messaging, config));
    }
}
