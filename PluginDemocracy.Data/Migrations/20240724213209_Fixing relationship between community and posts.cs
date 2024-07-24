using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fixingrelationshipbetweencommunityandposts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResidentialCommunityId2",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ResidentialCommunityId2",
                table: "Posts",
                column: "ResidentialCommunityId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ResidentialCommunities_ResidentialCommunityId2",
                table: "Posts",
                column: "ResidentialCommunityId2",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ResidentialCommunities_ResidentialCommunityId2",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ResidentialCommunityId2",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ResidentialCommunityId2",
                table: "Posts");
        }
    }
}
