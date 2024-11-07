using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class TryingtofixUsernotbeingaddedtoposts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts",
                column: "_userAuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users__userAuthorId",
                table: "Posts",
                column: "_userAuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
