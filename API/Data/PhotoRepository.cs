using API.Entities;
using API.Interfaces;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository(DataContext context) : IPhotoRepository
    {
        public async Task<Photo?> GetPhotoById(int id)
        {
            return await context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await context.Photos
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(p => new PhotoForApprovalDto
                {
                    Id = p.Id,
                    Username = p.AppUser.UserName,
                    Url = p.Url,
                    IsApproved = p.IsApproved
                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            context.Photos.Remove(photo);
        }
    }
}