using Core.Entities;
using Core.Enums;
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

    [Fact]
    public async Task GetAllVideoStatus_MapsGatewayResultsToDto()
    {
        var firstGuid = Guid.NewGuid();
        var secondGuid = Guid.NewGuid();
        var gw = new Mock<IVideoUploadGateway>();
        gw.Setup(x => x.GetAll(10)).ReturnsAsync(new[]
        {
            new VideoUpload
            {
                IdVideo = 1,
                IdUsuario = 10,
                Guid = firstGuid,
                NomeArquivoOriginal = "video-1.mp4",
                Status = StatusVideoEnum.Pending
            },
            new VideoUpload
            {
                IdVideo = 2,
                IdUsuario = 10,
                Guid = secondGuid,
                NomeArquivoOriginal = "video-2.mp4",
                Status = StatusVideoEnum.Completed
            }
        });

        var result = (await VideoUploadUseCases.GetAllVideoStatus(gw.Object, 10)).ToList();

        Assert.Collection(result,
            item =>
            {
                Assert.Equal(firstGuid, item.Guid);
                Assert.Equal("video-1.mp4", item.NomeArquivoOriginal);
                Assert.Equal("Pending", item.Status);
            },
            item =>
            {
                Assert.Equal(secondGuid, item.Guid);
                Assert.Equal("video-2.mp4", item.NomeArquivoOriginal);
                Assert.Equal("Completed", item.Status);
            });
        gw.Verify(x => x.GetAll(10), Times.Once);
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
