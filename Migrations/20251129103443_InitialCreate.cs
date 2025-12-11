using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace votesystembackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoteSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteSessions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PrivateVoteAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateVoteAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateVoteAccesses_VoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoteOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteOptions_VoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChosenOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVotes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserVotes_VoteOptions_ChosenOptionId",
                        column: x => x.ChosenOptionId,
                        principalTable: "VoteOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserVotes_VoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalTable: "VoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateVoteAccesses_VoteSessionId",
                table: "PrivateVoteAccesses",
                column: "VoteSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserVotes_ChosenOptionId",
                table: "UserVotes",
                column: "ChosenOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVotes_UserId",
                table: "UserVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVotes_VoteSessionId",
                table: "UserVotes",
                column: "VoteSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteOptions_VoteSessionId",
                table: "VoteOptions",
                column: "VoteSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteSessions_CreatedById",
                table: "VoteSessions",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateVoteAccesses");

            migrationBuilder.DropTable(
                name: "UserVotes");

            migrationBuilder.DropTable(
                name: "VoteOptions");

            migrationBuilder.DropTable(
                name: "VoteSessions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
