using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class JoinCommunityRequestnowhaswhoapproveditandwhen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalDecisionMadeById",
                table: "JoinCommunityRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfApprovalDecision",
                table: "JoinCommunityRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequests_ApprovalDecisionMadeById",
                table: "JoinCommunityRequests",
                column: "ApprovalDecisionMadeById");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequests_Users_ApprovalDecisionMadeById",
                table: "JoinCommunityRequests",
                column: "ApprovalDecisionMadeById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequests_Users_ApprovalDecisionMadeById",
                table: "JoinCommunityRequests");

            migrationBuilder.DropIndex(
                name: "IX_JoinCommunityRequests_ApprovalDecisionMadeById",
                table: "JoinCommunityRequests");

            migrationBuilder.DropColumn(
                name: "ApprovalDecisionMadeById",
                table: "JoinCommunityRequests");

            migrationBuilder.DropColumn(
                name: "DateOfApprovalDecision",
                table: "JoinCommunityRequests");
        }
    }
}
