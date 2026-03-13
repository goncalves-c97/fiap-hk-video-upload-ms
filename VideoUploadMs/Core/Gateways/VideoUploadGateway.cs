using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;

namespace Core.Gateways
{
    public class VideoUploadGateway(IDbConnection dbConnection) : IVideoUploadGateway
    {
        private readonly IDbConnection _dbConnection = dbConnection;
        private const string _tableName = nameof(VideoUpload);

        public async Task<IEnumerable<VideoUpload>> GetAll(int idUsuario)
        {
            return await _dbConnection.SearchByParametersAsync<VideoUpload>(
                _tableName,
                "id_usuario = @IdUsuario",
                new { IdUsuario = idUsuario }
            );
        } 

        public async Task<VideoUpload> Insert(VideoUpload videoUpload)
        {
            int registeredId = await _dbConnection.InsertAndReturnIdAsync(_tableName, new Dictionary<string, object>
            {
                { "guid", videoUpload.Guid },
                { "id_usuario", videoUpload.IdUsuario },
                { "email_usuario", videoUpload.EmailUsuario },
                { "nome_arquivo_original", videoUpload.NomeArquivoOriginal },
                { "caminho_storage_original", videoUpload.CaminhoStorageOriginal },
                { "caminho_zip_processado", videoUpload.CaminhoZipProcessado },
                { "tamanho_bytes", videoUpload.TamanhoBytes },
                { "tipo_mime", videoUpload.TipoMime },
                { "status", videoUpload.Status },
                { "data_hora_upload", videoUpload.DataHoraUpload },
                { "data_hora_inicio_processamento", videoUpload.DataHoraInicioProcessamento },
                { "data_hora_fim_processamento", videoUpload.DataHoraFimProcessamento },
                { "mensagem_erro", videoUpload.MensagemErro },
                { "tentativas_processamento", videoUpload.TentativasProcessamento }
            }, "id_video");

            return await GetById(registeredId, videoUpload.IdUsuario);
        }

        public async Task<VideoUpload?> GetById(int idVideo, int idUsuario)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<VideoUpload>(
                _tableName,
                "id_usuario = @Id AND id_video = @IdVideo",
                new { Id = idUsuario, IdVideo = idVideo }
            );
        }

        public async Task DeleteAll(int idUsuario)
        {
            await _dbConnection.DeleteAsync(_tableName, "id_usuario = @IdUsuario", new { IdUsuario = idUsuario });
        }
    }
}