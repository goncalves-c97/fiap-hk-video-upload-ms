using Microsoft.AspNetCore.Http;

namespace Core.Dtos
{
    public class UploadVideoRequestDto
    {
        public IFormFile Arquivo { get; set; }
    }
}
