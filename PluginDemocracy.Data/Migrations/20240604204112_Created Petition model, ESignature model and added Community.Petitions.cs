using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatedPetitionmodelESignaturemodelandaddedCommunityPetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Dictamens",
                newName: "TitleKey");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Dictamens",
                newName: "DescriptionKey");

            migrationBuilder.CreateTable(
                name: "Petitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Petitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Petitions_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ESignatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignerId = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetitionId = table.Column<int>(type: "int", nullable: false),
                    SignatureImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashOfSignedDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Intent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ESignatures_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ESignatures_Users_SignerId",
                        column: x => x.SignerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ESignatures_PetitionId",
                table: "ESignatures",
                column: "PetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ESignatures_SignerId",
                table: "ESignatures",
                column: "SignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_CommunityId",
                table: "Petitions",
                column: "CommunityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESignatures");

            migrationBuilder.DropTable(
                name: "Petitions");

            migrationBuilder.RenameColumn(
                name: "TitleKey",
                table: "Dictamens",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "DescriptionKey",
                table: "Dictamens",
                newName: "Description");
        }
    }
}
