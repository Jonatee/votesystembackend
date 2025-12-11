using System;

namespace votesystembackend.Domain.Entities
{
    public class PrivateVoteAccess
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid VoteSessionId { get; set; }
        public VoteSession? VoteSession { get; set; }

        public string AllowedEmail { get; set; } = string.Empty;

        public bool Used { get; set; } = false;
    }
}
