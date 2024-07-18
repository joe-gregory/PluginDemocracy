using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class PostAuthorignoredanduserauthorandcommunityauthortrackedbyEFCnow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ResidentialCommunities_CommunityAuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserAuthorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityAuthorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommunityAuthorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "_imagesLinks",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "UserAuthorId",
                table: "Posts",
                newName: "_communityAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserAuthorId",
                table: "Posts",
                newName: "IX_Posts__communityAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ResidentialCommunities__communityAuthorId",
                table: "Posts",
                column: "_communityAuthorId",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ResidentialCommunities__communityAuthorId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "_communityAuthorId",
                table: "Posts",
                newName: "UserAuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts__communityAuthorId",
                table: "Posts",
                newName: "IX_Posts_UserAuthorId");

            migrationBuilder.AddColumn<int>(
                name: "CommunityAuthorId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "_imagesLinks",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityAuthorId",
                table: "Posts",
                column: "CommunityAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ResidentialCommunities_CommunityAuthorId",
                table: "Posts",
                column: "CommunityAuthorId",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserAuthorId",
                table: "Posts",
                column: "UserAuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
