using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class VideoEndpointTests
{
    private static VideoEndpoint CreateEndpoint(FakeDbConnection db, ClaimsPrincipal? user = null)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
 {
 { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
 }).Build();

        var endpoint = new VideoEndpoint(db, new FakeObjectStorageService(), new FakeMessagingService(), config);

        var httpContext = new DefaultHttpContext();
        httpContext.User = user ?? new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "123")
 }, "TestAuth"));

        endpoint.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return endpoint;
    }

    private static IFormFile CreateFormFile(byte[] bytes, string fileName, string contentType)
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "Arquivo", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
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
        Assert.Matches(@"V.deo n.o encontrado", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetUploadStatus_WhenFound_ReturnsOkWithStatus()
    {
        var expected = new VideoUpload { IdVideo = 123, IdUsuario = 123, Status = StatusVideoEnum.Pending };

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
    public async Task GetUploadStatus_WhenMissingUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.GetUploadStatus(123);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);

        Assert.Matches(@"ID do usu.rio n.o encontrado.", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task GetUploadStatus_WhenInvalidUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "abc")
 }, "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.GetUploadStatus(123);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio inv.lido!", unauthorized.Value.ToString());
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
    public async Task ListUploads_WhenMissingUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.ListUploads();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio n.o encontrado.", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task ListUploads_WhenInvalidUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "not-an-int")
 }, "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.ListUploads();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio inv.lido!", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task ListVideosStatus_ReturnsOkWithMappedStatuses()
    {
        var guid = Guid.NewGuid();
        var db = new FakeDbConnection
        {
            SearchByParametersHandler = (_, __) => new object[]
            {
                new VideoUpload
                {
                    IdVideo = 1,
                    IdUsuario = 123,
                    Guid = guid,
                    NomeArquivoOriginal = "video.mp4",
                    Status = StatusVideoEnum.Processing
                }
            }
        };
        var endpoint = CreateEndpoint(db);

        var result = await endpoint.ListVideosStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var statuses = Assert.IsAssignableFrom<IEnumerable<VideoStatusDto>>(ok.Value).ToList();
        var status = Assert.Single(statuses);
        Assert.Equal(guid, status.Guid);
        Assert.Equal("video.mp4", status.NomeArquivoOriginal);
        Assert.Equal("Processing", status.Status);
    }

    [Fact]
    public async Task ListVideosStatus_WhenMissingUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.ListVideosStatus();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio n.o encontrado.", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task ListVideosStatus_WhenInvalidUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "invalid-user")
 }, "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var result = await endpoint.ListVideosStatus();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio inv.lido!", unauthorized.Value.ToString());
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

    [Fact]
    public async Task Upload_WhenMissingUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var endpoint = CreateEndpoint(db, new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "TestAuth")));

        var dto = new UploadVideoRequestDto
        {
            Arquivo = CreateFormFile(new byte[] { 1, 2, 3 }, "a.mp4", "video/mp4")
        };

        var result = await endpoint.Upload(dto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio n.o encontrado.", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task Upload_WhenInvalidUserIdClaim_ReturnsUnauthorized()
    {
        var db = new FakeDbConnection();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "NaN")
 }, "TestAuth"));
        var endpoint = CreateEndpoint(db, user);

        var dto = new UploadVideoRequestDto
        {
            Arquivo = CreateFormFile(new byte[] { 1, 2, 3 }, "a.mp4", "video/mp4")
        };

        var result = await endpoint.Upload(dto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Matches(@"ID do usu.rio inv.lido!", unauthorized.Value.ToString());
    }

    [Fact]
    public async Task Upload_WhenValid_ReturnsOkWithVideoUpload_AndUsesStorageAndEventBus()
    {
        var db = new FakeDbConnection
        {
            // Insert returns id, then gateway does GetById which uses SearchFirstOrDefault
            NextInsertId = 1,
            SearchFirstOrDefaultHandler = (_, __, ___) => new VideoUpload { IdVideo = 1, IdUsuario = 123, EmailUsuario="a@b.com.br", Status = StatusVideoEnum.Pending }
        };

        var storage = new FakeObjectStorageService();
        var bus = new FakeMessagingService();

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
 {
 { "API_AUTHENTICATION_KEY", "0123456789ABCDEF0123456789ABCDEF" }
 }).Build();

        var endpoint = new VideoEndpoint(db, storage, bus, config);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
 new Claim(ClaimTypes.NameIdentifier, "123"),
 new Claim(ClaimTypes.Email, "a@b.com.br")
 }, "TestAuth"));
        endpoint.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var dto = new UploadVideoRequestDto
        {
            Arquivo = CreateFormFile(new byte[] { 1, 2, 3, 4 }, "video.mp4", "video/mp4")
        };

        var result = await endpoint.Upload(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var uploaded = Assert.IsType<VideoUpload>(ok.Value);
        Assert.Equal(123, uploaded.IdUsuario);

        Assert.Single(storage.Uploads);
        Assert.Single(bus.Published);
        Assert.Equal("video-uploaded", bus.Published[0].Topic);
        Assert.Single(db.InsertAndReturnIds);
    }
}
