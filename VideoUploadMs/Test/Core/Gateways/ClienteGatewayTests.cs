using Core.Entities;
using Core.Gateways;
using Test.Helpers.Fakes;

namespace Test.Core.Gateways;

public class VideoUploadGatewayTests
{
    [Fact]
    public async Task GetAll_CallsDbWithExpectedWhereAndParams()
    {
        var fake = new FakeDbConnection();
        string? capturedTable = null;
        string? capturedWhere = null;
        object? capturedParams = null;

        fake.SearchByParametersHandler = (where, param) =>
        {
            capturedWhere = where;
            capturedParams = param;
            return Array.Empty<object>();
        };

        var gw = new VideoUploadGateway(fake);
        await gw.GetAll(10);

        // table name isn't exposed by fake handler; validate the where clause and params
        Assert.Equal("id_usuario = @IdUsuario", capturedWhere);
        Assert.NotNull(capturedParams);
    }

    [Fact]
    public async Task GetById_CallsDbWithExpectedWhereAndParams()
    {
        var fake = new FakeDbConnection();
        string? capturedTable = null;
        string? capturedWhere = null;
        object? capturedParams = null;

        fake.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            capturedTable = table;
            capturedWhere = where;
            capturedParams = param;
            return null;
        };

        var gw = new VideoUploadGateway(fake);
        await gw.GetById(5, 10);

        Assert.Equal("VideoUpload", capturedTable);
        Assert.Equal("id_usuario = @Id AND id_video = @IdVideo", capturedWhere);
        Assert.NotNull(capturedParams);
    }

    [Fact]
    public async Task DeleteAll_DeletesWithExpectedWhereAndParams()
    {
        var fake = new FakeDbConnection();
        var gw = new VideoUploadGateway(fake);

        await gw.DeleteAll(10);

        Assert.Single(fake.Deletes);
        Assert.Equal("VideoUpload", fake.Deletes[0].Table);
        Assert.Equal("id_usuario = @IdUsuario", fake.Deletes[0].WhereClause);
    }

    [Fact]
    public async Task Insert_InsertsExpectedFields_AndReturnsEntityFromGetById()
    {
        var fake = new FakeDbConnection { NextInsertId = 99 };
        fake.SearchFirstOrDefaultHandler = (_, __, ___) => new VideoUpload { IdVideo = 99, IdUsuario = 10 };

        var gw = new VideoUploadGateway(fake);
        var entity = new VideoUpload(10, "orig.mp4", "path/orig.mp4")
        {
            TamanhoBytes = 123,
            TipoMime = "video/mp4",
            DataHoraUpload = DateTime.UtcNow
        };

        var inserted = await gw.Insert(entity);

        Assert.Equal(99, inserted.IdVideo);
        Assert.Equal(10, inserted.IdUsuario);

        var insert = fake.InsertAndReturnIds.Single();
        Assert.Equal("VideoUpload", insert.Table);
        Assert.Equal("id_video", insert.IdColumn);
        Assert.True(insert.Values.ContainsKey("guid"));
        Assert.True(insert.Values.ContainsKey("id_usuario"));
        Assert.True(insert.Values.ContainsKey("nome_arquivo_original"));
        Assert.True(insert.Values.ContainsKey("caminho_storage_original"));
    }
}
