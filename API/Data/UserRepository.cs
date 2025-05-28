using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberAsync(string username, bool
isCurrentUser)
        {
            var query = context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .AsQueryable();
            if (isCurrentUser) query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);
            if (userParams.Gender != null)
            {
                query = query.Where(x => x.Gender == userParams.Gender);
            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }
        public void AddVisit(Visit visit)
        {
            context.Visits.Add(visit);
        }
        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await context.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }
        //         public async Task<AppUser?> GetUserByUsernameAsync(string username)
        // {
        //     return await context.Users
        //         .Include(x => x.Photos)
        //         //.SingleOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower());
        //         .SingleOrDefaultAsync(x => x.NormalizedUserName == username.ToUpper());
        // }


        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users
            .Include(x => x.Photos)
            .ToListAsync();
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public async Task<AppUser?> GetUserByPhotoId(int photoId)
        {
            return await context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p => p.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
        }

        public async Task<Visit?> GetVisit(int sourceUserId, int targetUserId)
        {
            return await context.Visits.FindAsync(sourceUserId, targetUserId);
        }

        /*public async Task<PagedList<VisitDto>> GetUserVisits(VisitsParams visitsParams, int userId)
        {
            var visits = context.Visits.AsQueryable();

            if (visitsParams.Predicate == "visited")
                visits = visits.Where(x => x.SourceUserId == userId);
            else
                visits = visits.Where(x => x.TargetUserId == userId);

            if (visitsParams.Filter == "month")
            {
                var monthAgo = DateTime.UtcNow.AddMonths(-1);
                visits = visits.Where(x => x.LastVisited >= monthAgo);
            }

            var visitsList = await visits
    .Include(x => x.SourceUser).ThenInclude(p => p!.Photos)
    .Include(x => x.TargetUser).ThenInclude(p => p!.Photos)
    .ToListAsync();

            var visitDtos = visitsList.Select(x => new VisitDto
            {
                Username = visitsParams.Predicate == "visited" ? x.TargetUser!.UserName : x.SourceUser!.UserName,
                KnownAs = visitsParams.Predicate == "visited" ? x.TargetUser!.KnownAs : x.SourceUser!.KnownAs,
                PhotoUrl = visitsParams.Predicate == "visited"
        ? x.TargetUser!.Photos.FirstOrDefault(p => p.IsMain)?.Url ?? ""
        : x.SourceUser!.Photos.FirstOrDefault(p => p.IsMain)?.Url ?? "",
                VisitedOn = x.LastVisited
            });

            return await PagedList<VisitDto>.CreateAsync(visitDtos.AsQueryable(), visitsParams.PageNumber, visitsParams.PageSize);
        }*/
        public async Task<PagedList<VisitDto>> GetUserVisits(VisitsParams visitsParams, int userId)
        {
            var visits = context.Visits.AsQueryable();

            if (visitsParams.Predicate == "visited")
                visits = visits.Where(x => x.SourceUserId == userId);
            else
                visits = visits.Where(x => x.TargetUserId == userId);

            if (visitsParams.Filter == "month")
            {
                var monthAgo = DateTime.UtcNow.AddMonths(-1);
                visits = visits.Where(x => x.LastVisited >= monthAgo);
            }

            /*var visitDtos = visits
                .OrderByDescending(x => x.LastVisited)
                .Select(x => new VisitDto
                {
                    Username = visitsParams.Predicate == "visited" ? x.TargetUser!.UserName : x.SourceUser!.UserName,
                    KnownAs = visitsParams.Predicate == "visited" ? x.TargetUser!.KnownAs : x.SourceUser!.KnownAs,
                    PhotoUrl = visitsParams.Predicate == "visited"
                        ? x.TargetUser!.Photos.FirstOrDefault(p => p.IsMain)!.Url
                        : x.SourceUser!.Photos.FirstOrDefault(p => p.IsMain)!.Url,
                    VisitedOn = x.LastVisited
                });*/
                var visitDtos = visits
    .Include(x => x.SourceUser).ThenInclude(p => p.Photos)
    .Include(x => x.TargetUser).ThenInclude(p => p.Photos)
    .Select(x => new VisitDto
    {
        Id = visitsParams.Predicate == "visited" ? x.TargetUser!.Id : x.SourceUser!.Id,
        Username = visitsParams.Predicate == "visited" ? x.TargetUser!.UserName : x.SourceUser!.UserName,
        KnownAs = visitsParams.Predicate == "visited" ? x.TargetUser!.KnownAs : x.SourceUser!.KnownAs,
        PhotoUrl = visitsParams.Predicate == "visited"
            ? x.TargetUser.Photos.FirstOrDefault(p => p.IsMain).Url
            : x.SourceUser.Photos.FirstOrDefault(p => p.IsMain).Url,
        City = visitsParams.Predicate == "visited" ? x.TargetUser.City : x.SourceUser.City,
        Age = visitsParams.Predicate == "visited"
            ? x.TargetUser.GetAge()
            : x.SourceUser.GetAge(),
        Created = visitsParams.Predicate == "visited" ? x.TargetUser.Created : x.SourceUser.Created,
        LastActive = visitsParams.Predicate == "visited" ? x.TargetUser.LastActive : x.SourceUser.LastActive,
        VisitedOn = x.LastVisited
    }).AsQueryable();


            return await PagedList<VisitDto>.CreateAsync(visitDtos, visitsParams.PageNumber, visitsParams.PageSize);
        }
    }
}
