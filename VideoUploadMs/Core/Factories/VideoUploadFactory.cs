using Core.Dtos;
using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factories
{
    public static class VideoUploadFactory
    {
        public static VideoUpload Create(VideoUploadDto videoUploadDto)
        {
            return new VideoUpload
            {
                IdUsuario = videoUploadDto.IdUsuario,
                EmailUsuario = videoUploadDto.EmailUsuario,
                NomeArquivoOriginal = videoUploadDto.NomeArquivoOriginal,
                CaminhoStorageOriginal = videoUploadDto.CaminhoStorageOriginal,
                TamanhoBytes = videoUploadDto.TamanhoBytes,
                TipoMime = videoUploadDto.TipoMime,
                Status = StatusVideoEnum.Pending,
                DataHoraUpload = DateTime.UtcNow,
                TentativasProcessamento = 0
            };
        }
    }
}
