using API.Entities;

namespace API.Interfaces
{
    public interface IVisitRepository
    {
        Task<Visit?> GetVisitAsync(int sourceUserId, int targetUserId);
        Task<IEnumerable<Visit>> GetVisitsByUserIdAsync(int userId);
        void AddVisit(Visit visit);
    }
}