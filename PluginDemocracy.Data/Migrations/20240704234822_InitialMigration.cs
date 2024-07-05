using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Admin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ResidentialCommunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    _officialLanguagesCodes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentialCommunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidentialCommunities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Home",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    InternalAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidentialCommunityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Home_ResidentialCommunities_ResidentialCommunityId",
                        column: x => x.ResidentialCommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Petitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommunityId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionRequested = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportingArguments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeadlineForResponse = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResidentialCommunityId = table.Column<int>(type: "int", nullable: true),
                    ResidentialCommunityId1 = table.Column<int>(type: "int", nullable: true),
                    _linksToSupportingDocuments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Petitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Petitions_ResidentialCommunities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId",
                        column: x => x.ResidentialCommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Petitions_ResidentialCommunities_ResidentialCommunityId1",
                        column: x => x.ResidentialCommunityId1,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LatestActivity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommunityAuthorId = table.Column<int>(type: "int", nullable: true),
                    ResidentialCommunityId = table.Column<int>(type: "int", nullable: true),
                    ResidentialCommunityId1 = table.Column<int>(type: "int", nullable: true),
                    UserAuthorId = table.Column<int>(type: "int", nullable: true),
                    _imagesLinks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_ResidentialCommunities_CommunityAuthorId",
                        column: x => x.CommunityAuthorId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_ResidentialCommunities_ResidentialCommunityId",
                        column: x => x.ResidentialCommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_ResidentialCommunities_ResidentialCommunityId1",
                        column: x => x.ResidentialCommunityId1,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserAuthorId",
                        column: x => x.UserAuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    HolderId = table.Column<int>(type: "int", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    PowersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_ResidentialCommunities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_RolePowers_PowersId",
                        column: x => x.PowersId,
                        principalTable: "RolePowers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Users_HolderId",
                        column: x => x.HolderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HomeOwnership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    OwnershipPercentage = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeOwnership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeOwnership_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
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
                name: "JoinCommunityRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommunityId = table.Column<int>(type: "int", nullable: false),
                    HomeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoiningAsOwner = table.Column<bool>(type: "bit", nullable: false),
                    JoiningAsResident = table.Column<bool>(type: "bit", nullable: false),
                    OwnershipPercentage = table.Column<double>(type: "float", nullable: false),
                    LinksToFiles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Messages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinCommunityRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinCommunityRequests_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinCommunityRequests_ResidentialCommunities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "ResidentialCommunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinCommunityRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ESignature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignerId = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetitionId = table.Column<int>(type: "int", nullable: false),
                    SignatureImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Intent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ESignature_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ESignature_Users_SignerId",
                        column: x => x.SignerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "PetitionReadyToPublishAuthor",
                columns: table => new
                {
                    PetitionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetitionReadyToPublishAuthor", x => new { x.PetitionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PetitionReadyToPublishAuthor_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetitionReadyToPublishAuthor_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostComment_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostComment_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostReaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReaction_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostReaction_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ESignature_PetitionId",
                table: "ESignature",
                column: "PetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ESignature_SignerId",
                table: "ESignature",
                column: "SignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Home_ResidentialCommunityId",
                table: "Home",
                column: "ResidentialCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_HomeId",
                table: "HomeOwnership",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeOwnership_OwnerId",
                table: "HomeOwnership",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeUser_ResidentsId",
                table: "HomeUser",
                column: "ResidentsId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequests_CommunityId",
                table: "JoinCommunityRequests",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequests_HomeId",
                table: "JoinCommunityRequests",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinCommunityRequests_UserId",
                table: "JoinCommunityRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionAuthor_UserId",
                table: "PetitionAuthor",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PetitionReadyToPublishAuthor_UserId",
                table: "PetitionReadyToPublishAuthor",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_CommunityId",
                table: "Petitions",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_ResidentialCommunityId",
                table: "Petitions",
                column: "ResidentialCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_ResidentialCommunityId1",
                table: "Petitions",
                column: "ResidentialCommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_PostComment_AuthorId",
                table: "PostComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComment_PostId",
                table: "PostComment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_PostId",
                table: "PostReaction",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_UserId",
                table: "PostReaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityAuthorId",
                table: "Posts",
                column: "CommunityAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ResidentialCommunityId",
                table: "Posts",
                column: "ResidentialCommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ResidentialCommunityId1",
                table: "Posts",
                column: "ResidentialCommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserAuthorId",
                table: "Posts",
                column: "UserAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentialCommunities_UserId",
                table: "ResidentialCommunities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CommunityId",
                table: "Role",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_HolderId",
                table: "Role",
                column: "HolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_PowersId",
                table: "Role",
                column: "PowersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ESignature");

            migrationBuilder.DropTable(
                name: "HomeOwnership");

            migrationBuilder.DropTable(
                name: "HomeUser");

            migrationBuilder.DropTable(
                name: "JoinCommunityRequests");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PetitionAuthor");

            migrationBuilder.DropTable(
                name: "PetitionReadyToPublishAuthor");

            migrationBuilder.DropTable(
                name: "PostComment");

            migrationBuilder.DropTable(
                name: "PostReaction");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Home");

            migrationBuilder.DropTable(
                name: "Petitions");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "RolePowers");

            migrationBuilder.DropTable(
                name: "ResidentialCommunities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
