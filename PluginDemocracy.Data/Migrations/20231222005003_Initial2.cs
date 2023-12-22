using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseRedFlaggable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRedFlaggable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Constitutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constitutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VotingStrategies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VotingStrategyType = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false),
                    MinimumAge = table.Column<int>(type: "int", nullable: true),
                    MaximumAge = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingStrategies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConstitutionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Constitutions_ConstitutionId",
                        column: x => x.ConstitutionId,
                        principalTable: "Constitutions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Accounting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounting_AccountingId",
                        column: x => x.AccountingId,
                        principalTable: "Accounting",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_BaseRedFlaggable_Id",
                        column: x => x.Id,
                        principalTable: "BaseRedFlaggable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialLanguages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanHaveHomes = table.Column<bool>(type: "bit", nullable: false),
                    CanHaveNonResidentialCitizens = table.Column<bool>(type: "bit", nullable: false),
                    ProposalsExpirationDays = table.Column<int>(type: "int", nullable: false),
                    VotingStrategyId = table.Column<int>(type: "int", nullable: true),
                    ConstitutionId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    HomeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
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
                name: "Homes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCommunityId = table.Column<int>(type: "int", nullable: true),
                    InternalAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Homes_Communities_ParentCommunityId",
                        column: x => x.ParentCommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    FundingGoal = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Proposal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingStrategyId = table.Column<int>(type: "int", nullable: true),
                    Open = table.Column<bool>(type: "bit", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposal_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposal_VotingStrategies_VotingStrategyId",
                        column: x => x.VotingStrategyId,
                        principalTable: "VotingStrategies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dictamens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    DictamenType = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false),
                    ParentProposalId = table.Column<int>(type: "int", nullable: true),
                    ProposalToCreateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictamens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dictamens_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dictamens_Proposal_ParentProposalId",
                        column: x => x.ParentProposalId,
                        principalTable: "Proposal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dictamens_Proposal_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dictamens_Proposal_ProposalToCreateId",
                        column: x => x.ProposalToCreateId,
                        principalTable: "Proposal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RedFlags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Resolved = table.Column<bool>(type: "bit", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemFlaggedId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProposalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedFlags_BaseRedFlaggable_ItemFlaggedId",
                        column: x => x.ItemFlaggedId,
                        principalTable: "BaseRedFlaggable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RedFlags_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RedFlags_Proposal_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Admin = table.Column<bool>(type: "bit", nullable: false),
                    RedFlagId = table.Column<int>(type: "int", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_RedFlags_RedFlagId",
                        column: x => x.RedFlagId,
                        principalTable: "RedFlags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HomeOwnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnershipPercentage = table.Column<double>(type: "float", nullable: false),
                    HomeId = table.Column<int>(type: "int", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    HomeId1 = table.Column<int>(type: "int", nullable: true),
                    HomeId2 = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeOwnership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Homes_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Homes_HomeId1",
                        column: x => x.HomeId1,
                        principalTable: "Homes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Homes_HomeId2",
                        column: x => x.HomeId2,
                        principalTable: "Homes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                        name: "FK_HomeUser_Homes_ResidentOfHomesId",
                        column: x => x.ResidentOfHomesId,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeUser_Users_ResidentsId",
                        column: x => x.ResidentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssigneeId = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanJoinCitizen = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanRemoveCitizen = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanJoinResident = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanRemoveResident = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanCreateRole = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanVerifyHomeOwnership = table.Column<bool>(type: "bit", nullable: false),
                    Powers_CanAccounting = table.Column<bool>(type: "bit", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_BaseRedFlaggable_Id",
                        column: x => x.Id,
                        principalTable: "BaseRedFlaggable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Roles_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountantId = table.Column<int>(type: "int", nullable: true),
                    DictamenId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionSnapShotId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionHistory_Dictamens_DictamenId",
                        column: x => x.DictamenId,
                        principalTable: "Dictamens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionHistory_Transactions_TransactionSnapShotId",
                        column: x => x.TransactionSnapShotId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionHistory_Users_AccountantId",
                        column: x => x.AccountantId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounting_CommunityId",
                table: "Accounting",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ConstitutionId",
                table: "Articles",
                column: "ConstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CommunityId",
                table: "Communities",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_ConstitutionId",
                table: "Communities",
                column: "ConstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_HomeId",
                table: "Communities",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_UserId",
                table: "Communities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_VotingStrategyId",
                table: "Communities",
                column: "VotingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamens_CommunityId",
                table: "Dictamens",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamens_ParentProposalId",
                table: "Dictamens",
                column: "ParentProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamens_ProposalId",
                table: "Dictamens",
                column: "ProposalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictamens_ProposalToCreateId",
                table: "Dictamens",
                column: "ProposalToCreateId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_CommunityId",
                table: "HomeOwnership",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId1",
                table: "HomeOwnership",
                column: "HomeId1");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId2",
                table: "HomeOwnership",
                column: "HomeId2");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_UserId",
                table: "HomeOwnership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Homes_ParentCommunityId",
                table: "Homes",
                column: "ParentCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeUser_ResidentsId",
                table: "HomeUser",
                column: "ResidentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CommunityId",
                table: "Projects",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_CommunityId",
                table: "Proposal",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposal_VotingStrategyId",
                table: "Proposal",
                column: "VotingStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_RedFlags_CommunityId",
                table: "RedFlags",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_RedFlags_ItemFlaggedId",
                table: "RedFlags",
                column: "ItemFlaggedId");

            migrationBuilder.CreateIndex(
                name: "IX_RedFlags_ProposalId",
                table: "RedFlags",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_AssigneeId",
                table: "Roles",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CommunityId",
                table: "Roles",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_AccountantId",
                table: "TransactionHistory",
                column: "AccountantId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_DictamenId",
                table: "TransactionHistory",
                column: "DictamenId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_TransactionSnapShotId",
                table: "TransactionHistory",
                column: "TransactionSnapShotId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountingId",
                table: "Transactions",
                column: "AccountingId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

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
                name: "FK_Communities_Homes_HomeId",
                table: "Communities",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_Users_UserId",
                table: "Communities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homes_Communities_ParentCommunityId",
                table: "Homes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_Communities_CommunityId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_RedFlags_Communities_CommunityId",
                table: "RedFlags");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "HomeOwnership");

            migrationBuilder.DropTable(
                name: "HomeUser");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "Dictamens");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounting");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "Constitutions");

            migrationBuilder.DropTable(
                name: "Homes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "RedFlags");

            migrationBuilder.DropTable(
                name: "BaseRedFlaggable");

            migrationBuilder.DropTable(
                name: "Proposal");

            migrationBuilder.DropTable(
                name: "VotingStrategies");
        }
    }
}
