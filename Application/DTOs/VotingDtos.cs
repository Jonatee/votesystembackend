using System;
using System.Collections.Generic;
using votesystembackend.Domain.Enums;

namespace votesystembackend.Application.DTOs
{
    public class CreateVoteSessionRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public VotingType Type { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<string> Options { get; set; } = new();
        public List<string>? PrivateEmails { get; set; }
    }

    public class CastVoteRequest
    {
        public Guid VoteOptionId { get; set; }
    }
}
