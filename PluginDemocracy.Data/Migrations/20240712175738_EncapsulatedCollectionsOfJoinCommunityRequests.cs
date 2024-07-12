using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class EncapsulatedCollectionsOfJoinCommunityRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinksToFiles",
                table: "JoinCommunityRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinksToFiles",
                table: "JoinCommunityRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
