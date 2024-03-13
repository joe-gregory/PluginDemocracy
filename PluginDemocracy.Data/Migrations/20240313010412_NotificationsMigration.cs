using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotificationsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_AssigneeId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanAccounting",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanCreateRole",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanJoinCitizen",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanJoinResident",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "Powers_CanVerifyHomeOwnership",
                table: "Roles",
                newName: "Powers_CanModifyAccounting");

            migrationBuilder.RenameColumn(
                name: "Powers_CanRemoveResident",
                table: "Roles",
                newName: "Powers_CanEditResidency");

            migrationBuilder.RenameColumn(
                name: "Powers_CanRemoveCitizen",
                table: "Roles",
                newName: "Powers_CanEditHomeOwnership");

            migrationBuilder.RenameColumn(
                name: "AssigneeId",
                table: "Roles",
                newName: "HolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_AssigneeId",
                table: "Roles",
                newName: "IX_Roles_HolderId");

            migrationBuilder.CreateTable(
                name: "JoinCommunityRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoiningAsOwner = table.Column<bool>(type: "bit", nullable: false),
                    JoiningAsResident = table.Column<bool>(type: "bit", nullable: false),
                    OwnershipPercentage = table.Column<double>(type: "float", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinCommunityRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinCommunityRequest_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequest_CommunityId",
                table: "JoinCommunityRequest",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_HolderId",
                table: "Roles",
                column: "HolderId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_HolderId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "JoinCommunityRequest");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.RenameColumn(
                name: "Powers_CanModifyAccounting",
                table: "Roles",
                newName: "Powers_CanVerifyHomeOwnership");

            migrationBuilder.RenameColumn(
                name: "Powers_CanEditResidency",
                table: "Roles",
                newName: "Powers_CanRemoveResident");

            migrationBuilder.RenameColumn(
                name: "Powers_CanEditHomeOwnership",
                table: "Roles",
                newName: "Powers_CanRemoveCitizen");

            migrationBuilder.RenameColumn(
                name: "HolderId",
                table: "Roles",
                newName: "AssigneeId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_HolderId",
                table: "Roles",
                newName: "IX_Roles_AssigneeId");

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanAccounting",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanCreateRole",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanJoinCitizen",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanJoinResident",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_AssigneeId",
                table: "Roles",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
