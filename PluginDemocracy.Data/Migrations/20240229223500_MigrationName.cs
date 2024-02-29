using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfficialLanguagesCodes",
                table: "Communities",
                newName: "_officialLanguagesCodes");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Homes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Homes");

            migrationBuilder.RenameColumn(
                name: "_officialLanguagesCodes",
                table: "Communities",
                newName: "OfficialLanguagesCodes");
        }
    }
}
