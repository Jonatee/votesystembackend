using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace votesystembackend.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedAtToVoteSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "VoteSessions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "VoteSessions");
        }
    }
}
