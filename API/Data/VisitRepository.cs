using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class VisitRepository : IVisitRepository
    {
        private readonly DataContext _context;
        public VisitRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Visit?> GetVisitAsync(int sourceUserId, int targetUserId)
        {
            return await _context.Visits.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByUserIdAsync(int userId)
        {
            return await _context.Visits
                .Where(v => v.SourceUserId == userId)
                .Include(v => v.TargetUser)
                .ToListAsync();
        }

        public void AddVisit(Visit visit)
        {
            _context.Visits.Add(visit);
        }
    }
}