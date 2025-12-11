using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using votesystembackend.Domain.Entities;
using votesystembackend.Infrastructure.Persistence;
using votesystembackend.Domain.Enums;

namespace votesystembackend.Infrastructure.Repositories
{
    public interface IVoteRepository
    {
        Task AddSessionAsync(VoteSession session);
        Task<VoteSession?> GetSessionAsync(Guid id);
        Task<List<VoteSession>> GetPublicSessionsAsync(int page, int pageSize);
        Task AddVoteAsync(UserVote vote);

        // New: find duplicate session matching title, description, expiresAt and options
        Task<VoteSession?> FindDuplicateSessionAsync(string title, string description, DateTime? expiresAt, List<string> options);

        // New for background job: get expired sessions up to 'now'
        Task<List<VoteSession>> GetExpiredSessionsAsync(DateTime now);
        Task<List<VoteSession>> GetExpiredVoteSessionsAsync(int page, int pageSize);
        Task<List<VoteSession>> GetSessionsAsync(int page, int pageSize,Guid userId);

        // New for background job: update session (persist changes)
        Task UpdateSessionAsync(VoteSession session);
    }

    public class VoteRepository : IVoteRepository
    {
        private readonly AppDbContext _db;

        public VoteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddSessionAsync(VoteSession session)
        {
            _db.VoteSessions.Add(session);
            await _db.SaveChangesAsync();
        }

        public Task<VoteSession?> GetSessionAsync(Guid id)
            => _db.VoteSessions
                .Include(x => x.Options)
                .Include(x => x.PrivateAccessList)
                .Include(x => x.UserVotes)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<List<VoteSession>> GetPublicSessionsAsync(int page, int pageSize)
            => _db.VoteSessions
                .Where(x => x.Type == VotingType.Public && x.ExpiresAt > DateTime.Now)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task AddVoteAsync(UserVote vote)
        {
            _db.UserVotes.Add(vote);
            await _db.SaveChangesAsync();
        }

        public async Task<VoteSession?> FindDuplicateSessionAsync(string title, string description, DateTime? expiresAt, List<string> options)
        {
            // Basic candidates matching title, description and expiresAt
            var candidates = await _db.VoteSessions
                .Include(x => x.Options)
                .Where(x => x.Title == title && x.Description == description && x.ExpiresAt == expiresAt)
                .ToListAsync();

            if (candidates == null || candidates.Count == 0)
                return null;

            // Normalize incoming options
            var normalizedOptions = options?.Select(o => (o ?? string.Empty).Trim().ToLowerInvariant()).OrderBy(x => x).ToList() ?? new List<string>();

            foreach (var cand in candidates)
            {
                var candOpts = cand.Options.Select(o => (o.Text ?? string.Empty).Trim().ToLowerInvariant()).OrderBy(x => x).ToList();
                if (candOpts.Count != normalizedOptions.Count)
                    continue;

                bool allMatch = candOpts.SequenceEqual(normalizedOptions);
                if (allMatch)
                    return cand;
            }

            return null;
        }

        public Task<List<VoteSession>> GetExpiredSessionsAsync(DateTime now)
        {
            return _db.VoteSessions
                .Where(s => s.ExpiresAt != null && s.ExpiresAt <= now && s.Status != VoteStatus.Closed)
                .Include(s => s.Options)
                .Include(s => s.PrivateAccessList)
                .Include(s => s.UserVotes)
                .ToListAsync();
        }
        public Task<List<VoteSession>> GetExpiredVoteSessionsAsync(int page, int pageSize)
        {
            return _db.VoteSessions
                .AsNoTracking()
                .Where(x=>x.ExpiresAt < DateTime.Now)
                .OrderByDescending(x => x.CreatedAt)
                .Include(s => s.Options)
                .Include(s => s.PrivateAccessList)
                .Include(s => s.UserVotes)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateSessionAsync(VoteSession session)
        {
            var tracked = _db.ChangeTracker.Entries<VoteSession>().FirstOrDefault(e => e.Entity.Id == session.Id);
            if (tracked == null)
            {
                _db.VoteSessions.Attach(session);
                _db.Entry(session).State = EntityState.Modified;
            }

            // set ClosedAt if closing now
            if (session.Status == VoteStatus.Closed && session.ClosedAt == null)
            {
                session.ClosedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public Task<List<VoteSession>> GetSessionsAsync(int page, int pageSize, Guid userId)
        {
            return _db.VoteSessions
                 .Where(x => x.UserVotes.Any(v => v.UserId == userId))
                 .OrderByDescending(x => x.CreatedAt)
                 .Include(s => s.Options)
                 .Include(s => s.PrivateAccessList)
                 .Include(s => s.UserVotes)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }

    }
}
