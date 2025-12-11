using System;

namespace votesystembackend.Domain.Entities
{
    public class UserVote
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid VoteSessionId { get; set; }
        public VoteSession? VoteSession { get; set; }

        public Guid ChosenOptionId { get; set; }
        public VoteOption? ChosenOption { get; set; }

        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}
