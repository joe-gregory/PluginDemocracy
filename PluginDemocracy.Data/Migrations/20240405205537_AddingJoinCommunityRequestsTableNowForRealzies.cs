using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingJoinCommunityRequestsTableNowForRealzies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Homes_HomeId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Users_UserId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JoinCommunityRequest",
                table: "JoinCommunityRequest");

            migrationBuilder.RenameTable(
                name: "JoinCommunityRequest",
                newName: "JoinCommunityRequests");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequest_UserId",
                table: "JoinCommunityRequests",
                newName: "IX_JoinCommunityRequests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequest_HomeId",
                table: "JoinCommunityRequests",
                newName: "IX_JoinCommunityRequests_HomeId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequest_CommunityId",
                table: "JoinCommunityRequests",
                newName: "IX_JoinCommunityRequests_CommunityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JoinCommunityRequests",
                table: "JoinCommunityRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequests_Communities_CommunityId",
                table: "JoinCommunityRequests",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequests_Homes_HomeId",
                table: "JoinCommunityRequests",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequests_Users_UserId",
                table: "JoinCommunityRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequests_Communities_CommunityId",
                table: "JoinCommunityRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequests_Homes_HomeId",
                table: "JoinCommunityRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequests_Users_UserId",
                table: "JoinCommunityRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JoinCommunityRequests",
                table: "JoinCommunityRequests");

            migrationBuilder.RenameTable(
                name: "JoinCommunityRequests",
                newName: "JoinCommunityRequest");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequests_UserId",
                table: "JoinCommunityRequest",
                newName: "IX_JoinCommunityRequest_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequests_HomeId",
                table: "JoinCommunityRequest",
                newName: "IX_JoinCommunityRequest_HomeId");

            migrationBuilder.RenameIndex(
                name: "IX_JoinCommunityRequests_CommunityId",
                table: "JoinCommunityRequest",
                newName: "IX_JoinCommunityRequest_CommunityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JoinCommunityRequest",
                table: "JoinCommunityRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Homes_HomeId",
                table: "JoinCommunityRequest",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Users_UserId",
                table: "JoinCommunityRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
