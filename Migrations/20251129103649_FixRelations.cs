using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace votesystembackend.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_VoteOptions_ChosenOptionId",
                table: "UserVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_VoteSessions_VoteSessionId",
                table: "UserVotes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_VoteOptions_ChosenOptionId",
                table: "UserVotes",
                column: "ChosenOptionId",
                principalTable: "VoteOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_VoteSessions_VoteSessionId",
                table: "UserVotes",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_VoteOptions_ChosenOptionId",
                table: "UserVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVotes_VoteSessions_VoteSessionId",
                table: "UserVotes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_VoteOptions_ChosenOptionId",
                table: "UserVotes",
                column: "ChosenOptionId",
                principalTable: "VoteOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVotes_VoteSessions_VoteSessionId",
                table: "UserVotes",
                column: "VoteSessionId",
                principalTable: "VoteSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
