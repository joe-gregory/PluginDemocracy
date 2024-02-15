using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixHomeIdForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership");

            migrationBuilder.DropIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "HomeOwnership");

            migrationBuilder.RenameColumn(
                name: "_cultureCode",
                table: "Users",
                newName: "CultureCode");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId1",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeId3",
                table: "HomeOwnership",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId3",
                table: "HomeOwnership",
                column: "HomeId3");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership",
                column: "HomeId1",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId3",
                table: "HomeOwnership",
                column: "HomeId3",
                principalTable: "Homes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId3",
                table: "HomeOwnership");

            migrationBuilder.DropIndex(
                name: "IX_HomeOwnership_HomeId3",
                table: "HomeOwnership");

            migrationBuilder.DropColumn(
                name: "HomeId3",
                table: "HomeOwnership");

            migrationBuilder.RenameColumn(
                name: "CultureCode",
                table: "Users",
                newName: "_cultureCode");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId1",
                table: "HomeOwnership",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HomeId",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership",
                column: "HomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId",
                table: "HomeOwnership",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership",
                column: "HomeId1",
                principalTable: "Homes",
                principalColumn: "Id");
        }
    }
}
