using System;
using System.Text.Json.Serialization;

namespace votesystembackend.Domain.Entities
{
    public class VoteOption
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;

        public Guid VoteSessionId { get; set; }
        [JsonIgnore]
        public VoteSession? VoteSession { get; set; }

        public int TotalVotes { get; set; }
    }
}
