using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Interfaces;
using votesystembackend.Domain.Entities;
using votesystembackend.Domain.Enums;
using votesystembackend.Infrastructure.Repositories;
using votesystembackend.Application.Responses;
using Microsoft.Extensions.Logging;

namespace votesystembackend.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<VoteService> _logger;

        public VoteService(IVoteRepository voteRepo, IUserRepository userRepo, ILogger<VoteService> logger)
        {
            _voteRepo = voteRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<ServiceResult<VoteSession>> CreateSessionAsync(Guid userId, CreateVoteSessionRequest req)
        {
            // Check duplicates: title, description, expiresAt and options
            var dup = await _voteRepo.FindDuplicateSessionAsync(req.Title, req.Description, req.ExpiresAt, req.Options);
            if (dup != null)
                return ServiceResult<VoteSession>.Fail(409, "A vote session with the same title, description, options and expiry already exists.");

            var session = new VoteSession
            {
                Title = req.Title,
                Description = req.Description,
                Type = req.Type,
                Status = VoteStatus.Active,
                CreatedByUserId = userId,
                ExpiresAt = req.ExpiresAt
            };

            foreach (var opt in req.Options)
            {
                session.Options.Add(new VoteOption { Text = opt });
            }

            if (req.Type == VotingType.Private && req.PrivateEmails != null)
            {
                foreach (var e in req.PrivateEmails)
                {
                    session.PrivateAccessList.Add(new PrivateVoteAccess { AllowedEmail = e });
                }
            }

            await _voteRepo.AddSessionAsync(session);
            return ServiceResult<VoteSession>.Created(session, "Created");
        }

        public async Task<ServiceResult<bool>> CastVoteAsync(Guid userId, Guid voteSessionId, CastVoteRequest req)
        {
            var session = await _voteRepo.GetSessionAsync(voteSessionId);
            if (session == null) return ServiceResult<bool>.Fail(404, "Session not found.");

            // Enforce expiry
            if (session.ExpiresAt.HasValue && DateTime.UtcNow > session.ExpiresAt.Value)
            {
                // Optionally: set session.Status = VoteStatus.Closed and persist (requires update method)
                return ServiceResult<bool>.Fail(410, "Voting session expired.");
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return ServiceResult<bool>.Fail(404, "User not found.");

            // private vote checks
            if (session.Type == VotingType.Private)
            {
                var entry = session.PrivateAccessList.FirstOrDefault(x => x.AllowedEmail == user.Email);
                if (entry == null) return ServiceResult<bool>.Fail(403, "You are not allowed to vote in this private session.");
                if (entry.Used) return ServiceResult<bool>.Fail(403, "This email has already voted.");
                entry.Used = true;
            }

            // ensure user hasn't already voted
            if (session.UserVotes.Any(x => x.UserId == userId))
                return ServiceResult<bool>.Fail(409, "User has already voted in this session.");

            var option = session.Options.FirstOrDefault(x => x.Id == req.VoteOptionId);
            if (option == null) return ServiceResult<bool>.Fail(400, "Invalid option.");

            option.TotalVotes++;

            var vote = new UserVote
            {
                UserId = userId,
                VoteSessionId = voteSessionId,
                ChosenOptionId = option.Id
            };

            await _voteRepo.AddVoteAsync(vote);
            return ServiceResult<bool>.Ok(true, "Voted");
        }

        public async Task<ServiceResult<VoteSession?>> GetSessionAsync(Guid id)
        {
            var s = await _voteRepo.GetSessionAsync(id);
            if (s == null) return ServiceResult<VoteSession?>.Fail(404, "Not found.");
            return ServiceResult<VoteSession?>.Ok(s);
        }

        public async Task<ServiceResult<List<VoteSession>>> GetPublicSessionsAsync(int page, int pageSize)
        {
            var list = await _voteRepo.GetPublicSessionsAsync(page, pageSize);
            if (list.Count == 0) return ServiceResult<List<VoteSession>>.Fail(404, "Not found.");
            return ServiceResult<List<VoteSession>>.Ok(list);
        }

        public async Task<ServiceResult<List<VoteSession>>> GetExpiredSessionsAsync(int page, int pageSize)
        {
            var list = await _voteRepo.GetExpiredVoteSessionsAsync(page, pageSize);
            if (list.Count == 0) return ServiceResult<List<VoteSession>>.Fail(404, "Not found.");
            return ServiceResult<List<VoteSession>>.Ok(list);
        }
        public async Task<ServiceResult<List<VoteSession>>> GetSessionsAsync(int page, int pageSize,Guid userId)
        {
            var list = await _voteRepo.GetSessionsAsync(page, pageSize,userId);
            if (list.Count == 0) return ServiceResult<List<VoteSession>>.Fail(404, "Not found.");
            return ServiceResult<List<VoteSession>>.Ok(list);
        }
        public async Task<ServiceResult<List<VoteSession>>> GetUserCreatedSessionsAsync(Guid userId)
        {
            // simple implementation
            var all = await _voteRepo.GetPublicSessionsAsync(1, int.MaxValue);
            var filtered = all.Where(x => x.CreatedByUserId == userId).ToList();
            return ServiceResult<List<VoteSession>>.Ok(filtered);
        }

        public async Task CloseExpiredSessionsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var expired = await _voteRepo.GetExpiredSessionsAsync(now);
                if (expired == null || expired.Count == 0)
                {
                    _logger.LogDebug("No expired sessions found at {time}", now);
                    return;
                }

                _logger.LogInformation("Closing {count} expired sessions at {time}", expired.Count, now);

                foreach (var session in expired)
                {
                    session.Status = VoteStatus.Closed;
                    // optionally set other fields, e.g. ClosedAt if you add it
                    await _voteRepo.UpdateSessionAsync(session);
                    _logger.LogInformation("Closed session {id}", session.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close expired sessions");
                throw;
            }
        }
    }
}
