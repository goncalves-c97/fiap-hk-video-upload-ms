using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Gateways
{
    public interface IVideoUploadGateway
    {
        public Task<IEnumerable<VideoUpload>> GetAll(int idUsuario);
        public Task<VideoUpload> Insert(VideoUpload videoUpload);
        public Task<VideoUpload?> GetById(int idVideo, int idUsuario);
        public Task DeleteAll(int idUsuario);
    }
}
