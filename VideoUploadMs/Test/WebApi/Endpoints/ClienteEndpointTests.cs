using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class VideoEndpointTests
{
    private static VideoEndpoint CreateEndpoint(FakeDbConnection db)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
        }).Build();

        var endpoint = new VideoEndpoint(db, new FakeObjectStorageService(), new FakeMessagingService(), config);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        }, "TestAuth"));

        endpoint.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return endpoint;
    }

    [Fact]
    public async Task GetUploadStatus_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => null
        };
        var endpoint = CreateEndpoint(db);

        var result = await endpoint.GetUploadStatus(123);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Vídeo não encontrado", notFound.Value);
    }

    [Fact]
    public async Task GetUploadStatus_WhenFound_ReturnsOkWithStatus()
    {
        var expected = new VideoUpload { IdVideo =123, IdUsuario =123, Status = StatusVideoEnum.Pending };

        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => expected
        };
        var endpoint = CreateEndpoint(db);

        var result = await endpoint.GetUploadStatus(123);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected.Status, ok.Value);
    }

    [Fact]
    public async Task ListUploads_ReturnsOkWithVideoUploads()
    {
        var expected = new[]
        {
            new VideoUpload { IdVideo =1, IdUsuario =123 },
            new VideoUpload { IdVideo =2, IdUsuario =123 }
        };

        var db = new FakeDbConnection
        {
            SearchByParametersHandler = (_, __) => expected.Cast<object>()
        };
        var endpoint = CreateEndpoint(db);

        var result = await endpoint.ListUploads();

        var ok = Assert.IsType<OkObjectResult>(result);
        var uploads = Assert.IsAssignableFrom<IEnumerable<VideoUpload>>(ok.Value);
        Assert.Equal(2, uploads.Count());
    }

    [Fact]
    public async Task Upload_WhenNoFile_ReturnsBadRequest()
    {
        var db = new FakeDbConnection();
        var endpoint = CreateEndpoint(db);

        var dto = new UploadVideoRequestDto { Arquivo = null! };
        var result = await endpoint.Upload(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Nenhum arquivo enviado.", bad.Value);
    }
}
