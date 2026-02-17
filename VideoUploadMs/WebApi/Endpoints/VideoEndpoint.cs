using Core.Controllers;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("Videos")]
    public class VideoEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly IObjectStorageService _objectStorageService;
        private readonly IMessagingService _eventBus;

        private readonly string _jwtSecret;

        public VideoEndpoint(IDbConnection dbConnection, IObjectStorageService objectStorageService, IMessagingService eventBus, IConfiguration configuration)
        {
            _dbConnection = dbConnection;
            _objectStorageService = objectStorageService;
            _eventBus = eventBus;

            string? jwtSecret = configuration["API_AUTHENTICATION_KEY"];

            if (string.IsNullOrEmpty(jwtSecret))
                throw new ArgumentException("A chave de autenticação da API não está configurada no appsettings.json.");

            _jwtSecret = jwtSecret;
        }

        [HttpPost, Route("Upload")]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> Upload([FromForm] UploadVideoRequestDto uploadVideoRequestDto)
        {
            if(uploadVideoRequestDto.Arquivo == null || uploadVideoRequestDto.Arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            string token = GetRequestToken(this);

            string? idUsuarioString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUsuarioString == null)
                return Unauthorized("ID do cliente não encontrado.");

            if (!int.TryParse(idUsuarioString, out int idUsuario))
                return Unauthorized("ID do cliente inválido!");

            VideoUpload videoUpload = await VideosController.UploadVideo(_dbConnection, _objectStorageService, _eventBus, idUsuario, uploadVideoRequestDto);

            return Ok(videoUpload);
        }

        [HttpGet, Route("ListUploads")]
        public async Task<IActionResult> ListUploads()
        {
            string? idUsuarioString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUsuarioString == null)
                return Unauthorized("ID do cliente não encontrado.");

            if (!int.TryParse(idUsuarioString, out int idUsuario))
                return Unauthorized("ID do cliente inválido!");

            return Ok(await VideosController.GetAll(_dbConnection, idUsuario));
        }

        [HttpGet, Route("GetUploadStatus")]
        public async Task<IActionResult> GetUploadStatus(int idVideo)
        {
            string? idUsuarioString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUsuarioString == null)
                return Unauthorized("ID do cliente não encontrado.");

            if (!int.TryParse(idUsuarioString, out int idUsuario))
                return Unauthorized("ID do cliente inválido!");

            VideoUpload? videoUpload = await VideosController.GetById(_dbConnection, idVideo, idUsuario);

            if (videoUpload == null)
                return NotFound("Vídeo não encontrado");

            return Ok(videoUpload.Status);
        }

        [HttpGet, Route("Download")]
        public async Task<IActionResult> Download(int idVideo)
        {
            return Ok();
        }

        [NonAction]
        private static string GetRequestToken(ControllerBase context)
        {
            return context.Request.Headers.Authorization
                .ToString()
                .Replace("Bearer ", string.Empty);
        }
    }
}
