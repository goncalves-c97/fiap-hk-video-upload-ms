using Core.Dtos;
using Core.Entities;
using Core.Gateways;
using Core.Interfaces;
using Core.UseCases;

namespace Core.Controllers
{
    public static class VideosController
    {

        public static async Task<VideoUpload> UploadVideo(IDbConnection dbConnection, IObjectStorageService objectStorageService, IMessagingService eventBus, int idUsuario, string emailUsuario, UploadVideoRequestDto clienteDto)
        {
            VideoUploadGateway gateway = new(dbConnection);
            VideoUpload videoUpload = await VideoUploadUseCases.UploadVideo(gateway, objectStorageService, eventBus, idUsuario, emailUsuario, clienteDto);
            return videoUpload;
        }

        public static async Task<IEnumerable<VideoUpload>> GetAll(IDbConnection dbConnection, int idUsuario)
        {
            VideoUploadGateway gateway = new(dbConnection);
            IEnumerable<VideoUpload> clientes = await VideoUploadUseCases.GetAllVideoUploads(gateway, idUsuario);
            return clientes;
        }

        public static async Task<IEnumerable<VideoStatusDto>> GetAllVideoStatus(IDbConnection dbConnection, int idUsuario)
        {
            VideoUploadGateway gateway = new(dbConnection);
            IEnumerable<VideoStatusDto> videoStatus = await VideoUploadUseCases.GetAllVideoStatus(gateway, idUsuario);
            return videoStatus;
        }

        public static async Task DeleteAll(IDbConnection dbConnection, int idUsuario)
        {
            VideoUploadGateway gateway = new(dbConnection);
            await VideoUploadUseCases.DeleteAll(gateway, idUsuario);
        }

        public static async Task<VideoUpload?> GetById(IDbConnection dbConnection, int id, int idUsuario)
        {
            VideoUploadGateway gateway = new(dbConnection);
            return await VideoUploadUseCases.GetById(gateway, id, idUsuario);
        }
    }
}
