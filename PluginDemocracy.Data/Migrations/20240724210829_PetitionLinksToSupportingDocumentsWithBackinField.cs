using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class PetitionLinksToSupportingDocumentsWithBackinField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "_linksToSupportingDocuments",
                table: "Petitions",
                newName: "LinksToSupportingDocuments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinksToSupportingDocuments",
                table: "Petitions",
                newName: "_linksToSupportingDocuments");
        }
    }
}
