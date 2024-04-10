using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingLoginIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId3",
                table: "HomeOwnership");

            migrationBuilder.RenameColumn(
                name: "HomeId3",
                table: "HomeOwnership",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_HomeOwnership_HomeId3",
                table: "HomeOwnership",
                newName: "IX_HomeOwnership_UserId1");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId1",
                table: "HomeOwnership",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CommunityId1",
                table: "HomeOwnership",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeId",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_CommunityId1",
                table: "HomeOwnership",
                column: "CommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership",
                column: "HomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Communities_CommunityId1",
                table: "HomeOwnership",
                column: "CommunityId1",
                principalTable: "Communities",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Users_UserId1",
                table: "HomeOwnership",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Communities_CommunityId1",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Homes_HomeId1",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Users_UserId1",
                table: "HomeOwnership");

            migrationBuilder.DropIndex(
                name: "IX_HomeOwnership_CommunityId1",
                table: "HomeOwnership");

            migrationBuilder.DropIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropColumn(
                name: "CommunityId1",
                table: "HomeOwnership");

            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "HomeOwnership");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "HomeOwnership",
                newName: "HomeId3");

            migrationBuilder.RenameIndex(
                name: "IX_HomeOwnership_UserId1",
                table: "HomeOwnership",
                newName: "IX_HomeOwnership_HomeId3");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId1",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
