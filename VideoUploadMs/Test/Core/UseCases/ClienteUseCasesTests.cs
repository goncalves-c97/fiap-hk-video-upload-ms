using Core.Entities;
using Core.Gateways;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Core.UseCases;
using Moq;

namespace Test.Core.UseCases;

public class VideoUploadUseCasesTests
{
    [Fact]
    public async Task GetAllVideoUploads_DelegatesToGateway()
    {
        var gw = new Mock<IVideoUploadGateway>();
        gw.Setup(x => x.GetAll(10)).ReturnsAsync(new[] { new VideoUpload { IdVideo =1, IdUsuario =10 } });

        var result = await VideoUploadUseCases.GetAllVideoUploads(gw.Object,10);

        Assert.Single(result);
        gw.Verify(x => x.GetAll(10), Times.Once);
    }

    [Fact]
    public async Task DeleteAll_DelegatesToGateway()
    {
        var gw = new Mock<IVideoUploadGateway>();

        await VideoUploadUseCases.DeleteAll(gw.Object,10);

        gw.Verify(x => x.DeleteAll(10), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetById_WhenInvalidId_Throws(int id)
    {
        var db = new Mock<IDbConnection>().Object;
        var gateway = new VideoUploadGateway(db);

        await Assert.ThrowsAsync<ArgumentException>(() => VideoUploadUseCases.GetById(gateway, id,10));
    }
}
