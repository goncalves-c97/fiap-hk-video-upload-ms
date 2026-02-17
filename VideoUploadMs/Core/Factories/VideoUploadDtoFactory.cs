using Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factories
{
    public static class VideoUploadDtoFactory
    {
        public static VideoUploadDto Create(int userId, UploadVideoRequestDto request, string storageKey)
        {
            return new VideoUploadDto
            {
                IdUsuario = userId,
                NomeArquivoOriginal = request.Arquivo.FileName,
                CaminhoStorageOriginal = storageKey,
                TamanhoBytes = request.Arquivo.Length,
                TipoMime = request.Arquivo.ContentType
            };
        }
    }
}
