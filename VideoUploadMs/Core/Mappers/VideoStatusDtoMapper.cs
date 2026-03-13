using Core.Dtos;
using Core.Entities;

namespace Core.Mappers;

public static class VideoStatusDtoMapper
{
    public static VideoStatusDto Map(VideoUpload videoUpload)
    {
        ArgumentNullException.ThrowIfNull(videoUpload);

        return new VideoStatusDto
        {
            Guid = videoUpload.Guid,
            NomeArquivoOriginal = videoUpload.NomeArquivoOriginal,
            Status = videoUpload.Status.ToString()
        };
    }

    public static IEnumerable<VideoStatusDto> Map(IEnumerable<VideoUpload> videoUploads)
    {
        ArgumentNullException.ThrowIfNull(videoUploads);

        return videoUploads.Select(Map);
    }
}
