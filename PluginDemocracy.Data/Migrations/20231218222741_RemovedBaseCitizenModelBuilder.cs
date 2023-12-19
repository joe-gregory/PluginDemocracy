using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedBaseCitizenModelBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounting_BaseCitizen_CommunityId",
                table: "Accounting");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizen_BaseCitizen_CommunityId",
                table: "BaseCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizen_BaseCitizen_HomeId",
                table: "BaseCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizen_Constitutions_ConstitutionId",
                table: "BaseCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizen_RedFlags_RedFlagId",
                table: "BaseCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizen_VotingStrategies_VotingStrategyId",
                table: "BaseCitizen");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_CitizensId",
                table: "BaseCitizenCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_CitizenshipsId",
                table: "BaseCitizenCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_Dictamens_BaseCitizen_CommunityId",
                table: "Dictamens");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_BaseCitizen_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_BaseCitizen_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_BaseCitizen_CommunityId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_BaseCitizen_CommunityId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_BaseCitizen_CommunityId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_BaseCitizen_UserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_RedFlags_BaseCitizen_CommunityId",
                table: "RedFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_BaseCitizen_AssigneeId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_BaseCitizen_CommunityId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_BaseCitizen_AccountantId",
                table: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "CommunityNonResidentialCitizens");

            migrationBuilder.DropIndex(
                name: "IX_BaseCitizen_CommunityId",
                table: "BaseCitizen");

            migrationBuilder.DropIndex(
                name: "IX_BaseCitizen_ConstitutionId",
                table: "BaseCitizen");

            migrationBuilder.DropIndex(
                name: "IX_BaseCitizen_HomeId",
                table: "BaseCitizen");

            migrationBuilder.DropIndex(
                name: "IX_BaseCitizen_RedFlagId",
                table: "BaseCitizen");

            migrationBuilder.DropIndex(
                name: "IX_BaseCitizen_VotingStrategyId",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Admin",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "CanHaveHomes",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "CanHaveNonResidentialCitizens",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "ConstitutionId",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "OfficialCurrency",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "ProposalsExpirationDays",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "RedFlagId",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "BaseCitizen");

            migrationBuilder.DropColumn(
                name: "VotingStrategyId",
                table: "BaseCitizen");

            migrationBuilder.RenameColumn(
                name: "CitizenshipsId",
                table: "BaseCitizenCommunity",
                newName: "NonResidentialCitizensId");

            migrationBuilder.RenameColumn(
                name: "CitizensId",
                table: "BaseCitizenCommunity",
                newName: "NonResidentialCitizenInId");

            migrationBuilder.RenameIndex(
                name: "IX_BaseCitizenCommunity_CitizenshipsId",
                table: "BaseCitizenCommunity",
                newName: "IX_BaseCitizenCommunity_NonResidentialCitizensId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDate",
                table: "Proposal",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "OwnershipPercentage",
                table: "HomeOwnership",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialLanguages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanHaveHomes = table.Column<bool>(type: "bit", nullable: false),
                    CanHaveNonResidentialCitizens = table.Column<bool>(type: "bit", nullable: false),
                    ProposalsExpirationDays = table.Column<int>(type: "int", nullable: false),
                    VotingStrategyId = table.Column<int>(type: "int", nullable: true),
                    ConstitutionId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_BaseCitizen_Id",
                        column: x => x.Id,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Communities_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Communities_Constitutions_ConstitutionId",
                        column: x => x.ConstitutionId,
                        principalTable: "Constitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Communities_VotingStrategies_VotingStrategyId",
                        column: x => x.VotingStrategyId,
                        principalTable: "VotingStrategies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Admin = table.Column<bool>(type: "bit", nullable: false),
                    RedFlagId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_BaseCitizen_Id",
                        column: x => x.Id,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_RedFlags_RedFlagId",
                        column: x => x.RedFlagId,
                        principalTable: "RedFlags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Home",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CommunityId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Home_Communities_CommunityId1",
                        column: x => x.CommunityId1,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Home_Communities_Id",
                        column: x => x.Id,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeUser",
                columns: table => new
                {
                    ResidentOfHomesId = table.Column<int>(type: "int", nullable: false),
                    ResidentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeUser", x => new { x.ResidentOfHomesId, x.ResidentsId });
                    table.ForeignKey(
                        name: "FK_HomeUser_Home_ResidentOfHomesId",
                        column: x => x.ResidentOfHomesId,
                        principalTable: "Home",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeUser_Users_ResidentsId",
                        column: x => x.ResidentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CommunityId",
                table: "Communities",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_ConstitutionId",
                table: "Communities",
                column: "ConstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_VotingStrategyId",
                table: "Communities",
                column: "VotingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Home_CommunityId1",
                table: "Home",
                column: "CommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_HomeUser_ResidentsId",
                table: "HomeUser",
                column: "ResidentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RedFlagId",
                table: "Users",
                column: "RedFlagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounting_Communities_CommunityId",
                table: "Accounting",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_NonResidentialCitizensId",
                table: "BaseCitizenCommunity",
                column: "NonResidentialCitizensId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizenCommunity_Communities_NonResidentialCitizenInId",
                table: "BaseCitizenCommunity",
                column: "NonResidentialCitizenInId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamens_Communities_CommunityId",
                table: "Dictamens",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_Home_HomeId",
                table: "HomeOwnership",
                column: "HomeId",
                principalTable: "Home",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Communities_CommunityId",
                table: "Projects",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Communities_CommunityId",
                table: "Proposal",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_Users_UserId",
                table: "Proposal",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RedFlags_Communities_CommunityId",
                table: "RedFlags",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Communities_CommunityId",
                table: "Roles",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_AssigneeId",
                table: "Roles",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_Users_AccountantId",
                table: "TransactionHistory",
                column: "AccountantId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounting_Communities_CommunityId",
                table: "Accounting");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_NonResidentialCitizensId",
                table: "BaseCitizenCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseCitizenCommunity_Communities_NonResidentialCitizenInId",
                table: "BaseCitizenCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_Dictamens_Communities_CommunityId",
                table: "Dictamens");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeOwnership_Home_HomeId",
                table: "HomeOwnership");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Communities_CommunityId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Communities_CommunityId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Users_UserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_RedFlags_Communities_CommunityId",
                table: "RedFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Communities_CommunityId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_AssigneeId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistory_Users_AccountantId",
                table: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "HomeUser");

            migrationBuilder.DropTable(
                name: "Home");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "Proposal");

            migrationBuilder.RenameColumn(
                name: "NonResidentialCitizensId",
                table: "BaseCitizenCommunity",
                newName: "CitizenshipsId");

            migrationBuilder.RenameColumn(
                name: "NonResidentialCitizenInId",
                table: "BaseCitizenCommunity",
                newName: "CitizensId");

            migrationBuilder.RenameIndex(
                name: "IX_BaseCitizenCommunity_NonResidentialCitizensId",
                table: "BaseCitizenCommunity",
                newName: "IX_BaseCitizenCommunity_CitizenshipsId");

            migrationBuilder.AlterColumn<int>(
                name: "OwnershipPercentage",
                table: "HomeOwnership",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "HomeId",
                table: "HomeOwnership",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "BaseCitizen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanHaveHomes",
                table: "BaseCitizen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanHaveNonResidentialCitizens",
                table: "BaseCitizen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConstitutionId",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "BaseCitizen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseCitizen",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "BaseCitizen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeId",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialCurrency",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "BaseCitizen",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProposalsExpirationDays",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedFlagId",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "BaseCitizen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VotingStrategyId",
                table: "BaseCitizen",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommunityNonResidentialCitizens",
                columns: table => new
                {
                    BaseCitizenId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityNonResidentialCitizens", x => new { x.BaseCitizenId, x.CommunityId });
                    table.ForeignKey(
                        name: "FK_CommunityNonResidentialCitizens_BaseCitizen_BaseCitizenId",
                        column: x => x.BaseCitizenId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityNonResidentialCitizens_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseCitizen_CommunityId",
                table: "BaseCitizen",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseCitizen_ConstitutionId",
                table: "BaseCitizen",
                column: "ConstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseCitizen_HomeId",
                table: "BaseCitizen",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseCitizen_RedFlagId",
                table: "BaseCitizen",
                column: "RedFlagId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseCitizen_VotingStrategyId",
                table: "BaseCitizen",
                column: "VotingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNonResidentialCitizens_CommunityId",
                table: "CommunityNonResidentialCitizens",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounting_BaseCitizen_CommunityId",
                table: "Accounting",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_BaseCitizen_CommunityId",
                table: "BaseCitizen",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_BaseCitizen_HomeId",
                table: "BaseCitizen",
                column: "HomeId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_Constitutions_ConstitutionId",
                table: "BaseCitizen",
                column: "ConstitutionId",
                principalTable: "Constitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_RedFlags_RedFlagId",
                table: "BaseCitizen",
                column: "RedFlagId",
                principalTable: "RedFlags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_VotingStrategies_VotingStrategyId",
                table: "BaseCitizen",
                column: "VotingStrategyId",
                principalTable: "VotingStrategies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_CitizensId",
                table: "BaseCitizenCommunity",
                column: "CitizensId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizenCommunity_BaseCitizen_CitizenshipsId",
                table: "BaseCitizenCommunity",
                column: "CitizenshipsId",
                principalTable: "BaseCitizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamens_BaseCitizen_CommunityId",
                table: "Dictamens",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeOwnership_BaseCitizen_HomeId",
                table: "HomeOwnership",
                column: "HomeId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_BaseCitizen_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_BaseCitizen_CommunityId",
                table: "Posts",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_BaseCitizen_CommunityId",
                table: "Projects",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_BaseCitizen_CommunityId",
                table: "Proposal",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposal_BaseCitizen_UserId",
                table: "Proposal",
                column: "UserId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RedFlags_BaseCitizen_CommunityId",
                table: "RedFlags",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_BaseCitizen_AssigneeId",
                table: "Roles",
                column: "AssigneeId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_BaseCitizen_CommunityId",
                table: "Roles",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistory_BaseCitizen_AccountantId",
                table: "TransactionHistory",
                column: "AccountantId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");
        }
    }
}
