using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingJoinCommunityRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "JoinCommunityRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "JoinCommunityRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequest_HomeId",
                table: "JoinCommunityRequest",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequest_UserId",
                table: "JoinCommunityRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Homes_HomeId",
                table: "JoinCommunityRequest",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Users_UserId",
                table: "JoinCommunityRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Homes_HomeId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinCommunityRequest_Users_UserId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropIndex(
                name: "IX_JoinCommunityRequest_HomeId",
                table: "JoinCommunityRequest");

            migrationBuilder.DropIndex(
                name: "IX_JoinCommunityRequest_UserId",
                table: "JoinCommunityRequest");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "JoinCommunityRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CommunityId",
                table: "JoinCommunityRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinCommunityRequest_Communities_CommunityId",
                table: "JoinCommunityRequest",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");
        }
    }
}
