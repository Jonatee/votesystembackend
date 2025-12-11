using Microsoft.EntityFrameworkCore;
using votesystembackend.Domain.Entities;

namespace votesystembackend.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<VoteSession> VoteSessions { get; set; }
        public DbSet<VoteOption> VoteOptions { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }
        public DbSet<PrivateVoteAccess> PrivateVoteAccesses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // minimal configuration
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Configure VoteSession.CreatedBy -> User (restrict delete)
            modelBuilder.Entity<VoteSession>()
                .HasOne(vs => vs.CreatedBy)
                .WithMany(u => u.CreatedVotingSessions)
                .HasForeignKey(vs => vs.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent multiple cascade paths on SQL Server by restricting deletes for certain relationships
            modelBuilder.Entity<UserVote>()
                .HasOne(uv => uv.VoteSession)
                .WithMany(vs => vs.UserVotes)
                .HasForeignKey(uv => uv.VoteSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserVote>()
                .HasOne(uv => uv.ChosenOption)
                .WithMany()
                .HasForeignKey(uv => uv.ChosenOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserVote>()
                .HasOne(uv => uv.User)
                .WithMany(u => u.UserVotes)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VoteOption>()
                .HasOne(vo => vo.VoteSession)
                .WithMany(vs => vs.Options)
                .HasForeignKey(vo => vo.VoteSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrivateVoteAccess>()
                .HasOne(p => p.VoteSession)
                .WithMany(vs => vs.PrivateAccessList)
                .HasForeignKey(p => p.VoteSessionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
