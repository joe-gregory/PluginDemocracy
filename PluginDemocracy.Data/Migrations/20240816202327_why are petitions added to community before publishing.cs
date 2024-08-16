using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class whyarepetitionsaddedtocommunitybeforepublishing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId",
                table: "Petitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId1",
                table: "Petitions");

            migrationBuilder.DropIndex(
                name: "IX_Petitions_ResidentialCommunityId",
                table: "Petitions");

            migrationBuilder.DropIndex(
                name: "IX_Petitions_ResidentialCommunityId1",
                table: "Petitions");

            migrationBuilder.DropColumn(
                name: "ResidentialCommunityId",
                table: "Petitions");

            migrationBuilder.DropColumn(
                name: "ResidentialCommunityId1",
                table: "Petitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResidentialCommunityId",
                table: "Petitions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResidentialCommunityId1",
                table: "Petitions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_ResidentialCommunityId",
                table: "Petitions",
                column: "ResidentialCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_ResidentialCommunityId1",
                table: "Petitions",
                column: "ResidentialCommunityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId",
                table: "Petitions",
                column: "ResidentialCommunityId",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId1",
                table: "Petitions",
                column: "ResidentialCommunityId1",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id");
        }
    }
}
