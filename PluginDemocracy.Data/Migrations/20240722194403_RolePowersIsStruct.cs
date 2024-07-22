using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RolePowersIsStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_ResidentialCommunities_CommunityId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_RolePowers_PowersId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Users_HolderId",
                table: "Role");

            migrationBuilder.DropTable(
                name: "PetitionAuthor");

            migrationBuilder.DropTable(
                name: "RolePowers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_PowersId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "PowersId",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_Role_HolderId",
                table: "Roles",
                newName: "IX_Roles_HolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Role_CommunityId",
                table: "Roles",
                newName: "IX_Roles_CommunityId");

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanEditHomeOwnership",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanEditResidency",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Powers_CanModifyAccounting",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PetitionUser",
                columns: table => new
                {
                    AuthorsId = table.Column<int>(type: "int", nullable: false),
                    PetitionDraftsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetitionUser", x => new { x.AuthorsId, x.PetitionDraftsId });
                    table.ForeignKey(
                        name: "FK_PetitionUser_Petitions_PetitionDraftsId",
                        column: x => x.PetitionDraftsId,
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetitionUser_Users_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetitionUser_PetitionDraftsId",
                table: "PetitionUser",
                column: "PetitionDraftsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_ResidentialCommunities_CommunityId",
                table: "Roles",
                column: "CommunityId",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Roles_ResidentialCommunities_CommunityId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_HolderId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "PetitionUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanEditHomeOwnership",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanEditResidency",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Powers_CanModifyAccounting",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_HolderId",
                table: "Role",
                newName: "IX_Role_HolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_CommunityId",
                table: "Role",
                newName: "IX_Role_CommunityId");

            migrationBuilder.AddColumn<int>(
                name: "PowersId",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PetitionAuthor",
                columns: table => new
                {
                    PetitionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetitionAuthor", x => new { x.PetitionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PetitionAuthor_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetitionAuthor_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanEditHomeOwnership = table.Column<bool>(type: "bit", nullable: false),
                    CanEditResidency = table.Column<bool>(type: "bit", nullable: false),
                    CanModifyAccounting = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePowers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Role_PowersId",
                table: "Role",
                column: "PowersId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionAuthor_UserId",
                table: "PetitionAuthor",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_ResidentialCommunities_CommunityId",
                table: "Role",
                column: "CommunityId",
                principalTable: "ResidentialCommunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_RolePowers_PowersId",
                table: "Role",
                column: "PowersId",
                principalTable: "RolePowers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Users_HolderId",
                table: "Role",
                column: "HolderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
