using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Try223125Eighteen : Migration
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
                name: "BaseCitizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    HomeId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialLanguages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanHaveHomes = table.Column<bool>(type: "bit", nullable: true),
                    CanHaveNonResidentialCitizens = table.Column<bool>(type: "bit", nullable: true),
                    ProposalsExpirationDays = table.Column<int>(type: "int", nullable: true),
                    VotingStrategyId = table.Column<int>(type: "int", nullable: true),
                    ConstitutionId = table.Column<int>(type: "int", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Admin = table.Column<bool>(type: "bit", nullable: true),
                    RedFlagId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCitizen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseCitizen_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseCitizen_BaseCitizen_HomeId",
                        column: x => x.HomeId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseCitizen_Constitutions_ConstitutionId",
                        column: x => x.ConstitutionId,
                        principalTable: "Constitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseCitizen_VotingStrategies_VotingStrategyId",
                        column: x => x.VotingStrategyId,
                        principalTable: "VotingStrategies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseCitizenCommunity",
                columns: table => new
                {
                    CitizensId = table.Column<int>(type: "int", nullable: false),
                    CitizenshipsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCitizenCommunity", x => new { x.CitizensId, x.CitizenshipsId });
                    table.ForeignKey(
                        name: "FK_BaseCitizenCommunity_BaseCitizen_CitizensId",
                        column: x => x.CitizensId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseCitizenCommunity_BaseCitizen_CitizenshipsId",
                        column: x => x.CitizenshipsId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "HomeOwnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    OwnershipPercentage = table.Column<int>(type: "int", nullable: false),
                    HomeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeOwnership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeOwnership_BaseCitizen_HomeId",
                        column: x => x.HomeId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeOwnership_BaseCitizen_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "BaseCitizen",
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
                        name: "FK_Posts_BaseCitizen_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
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
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    FundingGoal = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    VotingStrategyId = table.Column<int>(type: "int", nullable: true),
                    Open = table.Column<bool>(type: "bit", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposal_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proposal_BaseCitizen_UserId",
                        column: x => x.UserId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposal_VotingStrategies_VotingStrategyId",
                        column: x => x.VotingStrategyId,
                        principalTable: "VotingStrategies",
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
                        name: "FK_Roles_BaseCitizen_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_BaseRedFlaggable_Id",
                        column: x => x.Id,
                        principalTable: "BaseRedFlaggable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_Dictamens_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
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
                        name: "FK_RedFlags_BaseCitizen_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RedFlags_BaseRedFlaggable_ItemFlaggedId",
                        column: x => x.ItemFlaggedId,
                        principalTable: "BaseRedFlaggable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RedFlags_Proposal_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposal",
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
                        name: "FK_TransactionHistory_BaseCitizen_AccountantId",
                        column: x => x.AccountantId,
                        principalTable: "BaseCitizen",
                        principalColumn: "Id");
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
                name: "IX_BaseCitizenCommunity_CitizenshipsId",
                table: "BaseCitizenCommunity",
                column: "CitizenshipsId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityNonResidentialCitizens_CommunityId",
                table: "CommunityNonResidentialCitizens",
                column: "CommunityId");

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
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_OwnerId",
                table: "HomeOwnership",
                column: "OwnerId");

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
                name: "IX_Proposal_UserId",
                table: "Proposal",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Accounting_BaseCitizen_CommunityId",
                table: "Accounting",
                column: "CommunityId",
                principalTable: "BaseCitizen",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseCitizen_RedFlags_RedFlagId",
                table: "BaseCitizen",
                column: "RedFlagId",
                principalTable: "RedFlags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_BaseCitizen_CommunityId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposal_BaseCitizen_UserId",
                table: "Proposal");

            migrationBuilder.DropForeignKey(
                name: "FK_RedFlags_BaseCitizen_CommunityId",
                table: "RedFlags");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "BaseCitizenCommunity");

            migrationBuilder.DropTable(
                name: "CommunityNonResidentialCitizens");

            migrationBuilder.DropTable(
                name: "HomeOwnership");

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
                name: "BaseCitizen");

            migrationBuilder.DropTable(
                name: "Constitutions");

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
