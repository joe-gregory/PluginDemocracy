using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Deletinguserdeleteshisvotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Users_VoterId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Users_VoterId",
                table: "Votes",
                column: "VoterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Users_VoterId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Users_VoterId",
                table: "Votes",
                column: "VoterId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
