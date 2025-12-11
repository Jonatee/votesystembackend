using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using votesystembackend.Infrastructure.Repositories;
using votesystembackend.Domain.Enums;

namespace votesystembackend.Infrastructure.Jobs
{
    public class ExpiredSessionJob
    {
        private readonly IVoteRepository _voteRepo;
        private readonly ILogger<ExpiredSessionJob> _logger;

        public ExpiredSessionJob(IVoteRepository voteRepo, ILogger<ExpiredSessionJob> logger)
        {
            _voteRepo = voteRepo;
            _logger = logger;
        }

        public async Task CloseExpiredSessionsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var expired = await _voteRepo.GetExpiredSessionsAsync(now);
                if (expired == null || expired.Count == 0)
                {
                    _logger.LogDebug("ExpiredSessionJob: no expired sessions found at {time}", now);
                    return;
                }

                _logger.LogInformation("ExpiredSessionJob: closing {count} expired sessions at {time}", expired.Count, now);

                foreach (var session in expired)
                {
                    session.Status = VoteStatus.Closed;
                    await _voteRepo.UpdateSessionAsync(session);
                    _logger.LogInformation("ExpiredSessionJob: closed session {id}", session.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExpiredSessionJob failed");
                throw;
            }
        }
    }
}
