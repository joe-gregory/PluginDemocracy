using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommunityOfficialLanguagesCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfficialLanguages",
                table: "Communities",
                newName: "OfficialLanguagesCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfficialLanguagesCodes",
                table: "Communities",
                newName: "OfficialLanguages");
        }
    }
}
