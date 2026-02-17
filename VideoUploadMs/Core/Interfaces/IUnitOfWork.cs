using Core.Interfaces.Gateways;

namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IVideoUploadGateway VideoUploadRepository { get; }

    }
}
