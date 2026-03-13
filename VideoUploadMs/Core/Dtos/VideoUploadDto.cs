using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class VideoUploadDto
    {
        public int IdUsuario { get; set; }
        public string? EmailUsuario { get; set; }
        public string NomeArquivoOriginal { get; set; }
        public string CaminhoStorageOriginal { get; set; }
        public long TamanhoBytes { get; set; }
        public string TipoMime { get; set; }
    }
}
