using System;
using System.Collections.Generic;
using votesystembackend.Domain.Enums;

namespace votesystembackend.Domain.Entities
{
    public class VoteSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public VotingType Type { get; set; }
        public VoteStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        // New: when the session was closed by the system/job
        public DateTime? ClosedAt { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public List<VoteOption> Options { get; set; } = new();
        public List<PrivateVoteAccess> PrivateAccessList { get; set; } = new();
        public List<UserVote> UserVotes { get; set; } = new();
    }
}
