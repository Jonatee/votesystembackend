using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Responses;
using votesystembackend.Domain.Entities;

namespace votesystembackend.Application.Interfaces
{
    public interface IVoteService
    {
        Task<ServiceResult<VoteSession>> CreateSessionAsync(Guid userId, CreateVoteSessionRequest req);
        Task<ServiceResult<bool>> CastVoteAsync(Guid userId, Guid voteSessionId, CastVoteRequest req);
        Task<ServiceResult<VoteSession?>> GetSessionAsync(Guid id);
        Task<ServiceResult<List<VoteSession>>> GetPublicSessionsAsync(int page, int pageSize);
        Task<ServiceResult<List<VoteSession>>> GetExpiredSessionsAsync(int page, int pageSize);
        Task<ServiceResult<List<VoteSession>>> GetSessionsAsync(int page, int pageSize,Guid userId);
        Task<ServiceResult<List<VoteSession>>> GetUserCreatedSessionsAsync(Guid userId);
    }
}
