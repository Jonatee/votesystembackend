using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace votesystembackend.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivateVoteAccesses_VoteSessions_VoteSessionId",
                table: "PrivateVoteAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_Users_UserId",
                table: "UserVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_VoteSessions_VoteSessionId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteSessions_Users_CreatedById",
                table: "VoteSessions");

            migrationBuilder.DropIndex(
                name: "IX_VoteSessions_CreatedById",
                table: "VoteSessions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "VoteSessions");

            migrationBuilder.CreateIndex(
                name: "IX_VoteSessions_CreatedByUserId",
                table: "VoteSessions",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateVoteAccesses_VoteSessions_VoteSessionId",
                table: "PrivateVoteAccesses",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_Users_UserId",
                table: "UserVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_VoteSessions_VoteSessionId",
                table: "VoteOptions",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSessions_Users_CreatedByUserId",
                table: "VoteSessions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivateVoteAccesses_VoteSessions_VoteSessionId",
                table: "PrivateVoteAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_Users_UserId",
                table: "UserVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteOptions_VoteSessions_VoteSessionId",
                table: "VoteOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteSessions_Users_CreatedByUserId",
                table: "VoteSessions");

            migrationBuilder.DropIndex(
                name: "IX_VoteSessions_CreatedByUserId",
                table: "VoteSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "VoteSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteSessions_CreatedById",
                table: "VoteSessions",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateVoteAccesses_VoteSessions_VoteSessionId",
                table: "PrivateVoteAccesses",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_Users_UserId",
                table: "UserVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteOptions_VoteSessions_VoteSessionId",
                table: "VoteOptions",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteSessions_Users_CreatedById",
                table: "VoteSessions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
