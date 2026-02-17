using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Events;
using Core.Factories;
using Core.Gateways;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace Core.UseCases
{
    public static class VideoUploadUseCases
    {
        private static int GetUserIdFromToken(string secret)
        {
            int userId = JwtReaderHelperClass.GetClaimValue(secret, ClaimTypes.NameIdentifier) != null ? int.Parse(JwtReaderHelperClass.GetClaimValue(secret, ClaimTypes.NameIdentifier)!) : throw new ArgumentException("Token JWT inválido: claim 'NameIdentifier' não encontrado!");
            return userId;
        }

        public static async Task<VideoUpload> UploadVideo(IVideoUploadGateway videoUploadGateway, IObjectStorageService objectStorageService, IMessagingService eventBus, int idUsuario, UploadVideoRequestDto uploadVideoRequestDto)
        {
           if(idUsuario == 0)
                throw new ArgumentException("idUsuario não informado ou inválido!");

            VideoUploadDto videoUploadDto = VideoUploadDtoFactory.Create(idUsuario, uploadVideoRequestDto, "video_upload");

            VideoUpload videoUpload = VideoUploadFactory.Create(videoUploadDto);

            await objectStorageService.UploadAsync(uploadVideoRequestDto.Arquivo.OpenReadStream(), videoUpload.CaminhoStorageOriginal, videoUpload.TipoMime);
            
            VideoUploadedEvent videoUploadedEvent = new VideoUploadedEvent
            {
                VideoId = videoUpload.Guid,
                UserId = videoUpload.IdUsuario,
                StoragePath = videoUpload.CaminhoStorageOriginal,
                UploadedAt = videoUpload.DataHoraUpload
            };

            await eventBus.PublishAsync("video-uploaded", videoUploadedEvent);

            return await videoUploadGateway.Insert(videoUpload);
        }

        public static async Task<IEnumerable<VideoUpload>> GetAllVideoUploads(IVideoUploadGateway videoUploadGateway, int idUsuario)
        {
            return await videoUploadGateway.GetAll(idUsuario);
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