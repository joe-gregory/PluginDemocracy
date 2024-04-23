using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToPostPropertyInCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId1",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId2",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityId2",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommunityId1",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommunityId2",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunityId1",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommunityId2",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId1",
                table: "Posts",
                column: "CommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId2",
                table: "Posts",
                column: "CommunityId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId1",
                table: "Posts",
                column: "CommunityId1",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId2",
                table: "Posts",
                column: "CommunityId2",
                principalTable: "Communities",
                principalColumn: "Id");
        }
    }
}
