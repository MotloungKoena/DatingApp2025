using API.Entities;
using API.DTOs;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<Photo?> GetPhotoById(int id);
        void RemovePhoto(Photo photo);
    }
}