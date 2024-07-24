using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Posthasone_userAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "_userAuthorId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts__userAuthorId",
                table: "Posts",
                column: "_userAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts",
                column: "_userAuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts__userAuthorId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "_userAuthorId",
                table: "Posts");
        }
    }
}
