using Core.Dtos;
using Core.Entities;
using Core.Events;
using Core.Factories;
using Core.Gateways;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Core.Mappers;

namespace Core.UseCases
{
    public static class VideoUploadUseCases
    {
        public static async Task<VideoUpload> UploadVideo(IVideoUploadGateway videoUploadGateway, IObjectStorageService objectStorageService, IMessagingService eventBus, int idUsuario, string emailUsuario, UploadVideoRequestDto uploadVideoRequestDto)
        {
           if(idUsuario == 0)
                throw new ArgumentException("idUsuario não informado ou inválido!");

           if(string.IsNullOrEmpty(emailUsuario))
                throw new ArgumentException("emailUsuario não informado!");

            VideoUploadDto videoUploadDto = VideoUploadDtoFactory.Create(idUsuario, emailUsuario, uploadVideoRequestDto, "video_upload");

            VideoUpload videoUpload = VideoUploadFactory.Create(videoUploadDto);

            string key = await objectStorageService.UploadAsync(uploadVideoRequestDto.Arquivo.OpenReadStream(), videoUpload.CaminhoStorageOriginal, videoUpload.TipoMime);
            
            VideoUploadedEvent videoUploadedEvent = new VideoUploadedEvent
            {
                VideoId = videoUpload.Guid,
                UserId = videoUpload.IdUsuario,
                UserEmail = videoUpload.EmailUsuario,
                OriginalVideoName = videoUpload.NomeArquivoOriginal,
                StoragePath = key,
                UploadedAt = videoUpload.DataHoraUpload
            };

            await eventBus.PublishAsync("video-uploaded", videoUploadedEvent);

            return await videoUploadGateway.Insert(videoUpload);
        }

        public static async Task<IEnumerable<VideoUpload>> GetAllVideoUploads(IVideoUploadGateway videoUploadGateway, int idUsuario)
        {
            return await videoUploadGateway.GetAll(idUsuario);
        }

        public static async Task<IEnumerable<VideoStatusDto>> GetAllVideoStatus(IVideoUploadGateway videoUploadGateway, int idUsuario)
        {
            IEnumerable<VideoUpload> userVideos = await videoUploadGateway.GetAll(idUsuario);
            return VideoStatusDtoMapper.Map(userVideos);
        }

        public static async Task DeleteAll(IVideoUploadGateway videoUploadGateway, int idUsuario)
        {
            await videoUploadGateway.DeleteAll(idUsuario);
        }

        public static async Task<VideoUpload?> GetById(VideoUploadGateway gateway, int id, int idUsuario)
        {
            if (id <= 0)
                throw new ArgumentException("id não informado ou inválido!");

            return await gateway.GetById(id, idUsuario);
        }
    }
}