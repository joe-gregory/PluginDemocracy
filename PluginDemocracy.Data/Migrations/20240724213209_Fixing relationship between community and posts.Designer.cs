﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PluginDemocracy.Data;

#nullable disable

namespace PluginDemocracy.Data.Migrations
{
    [DbContext(typeof(PluginDemocracyContext))]
    [Migration("20240724213209_Fixing relationship between community and posts")]
    partial class Fixingrelationshipbetweencommunityandposts
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HomeUser", b =>
                {
                    b.Property<int>("ResidentOfHomesId")
                        .HasColumnType("int");

                    b.Property<int>("ResidentsId")
                        .HasColumnType("int");

                    b.HasKey("ResidentOfHomesId", "ResidentsId");

                    b.HasIndex("ResidentsId");

                    b.ToTable("HomeUser");
                });

            modelBuilder.Entity("PetitionReadyToPublishAuthor", b =>
                {
                    b.Property<int>("PetitionId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PetitionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("PetitionReadyToPublishAuthor");
                });

            modelBuilder.Entity("PetitionUser", b =>
                {
                    b.Property<int>("AuthorsId")
                        .HasColumnType("int");

                    b.Property<int>("PetitionDraftsId")
                        .HasColumnType("int");

                    b.HasKey("AuthorsId", "PetitionDraftsId");

                    b.HasIndex("PetitionDraftsId");

                    b.ToTable("PetitionUser");
                });

            modelBuilder.Entity("PluginDemocracy.Models.ESignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DocumentHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Intent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PetitionId")
                        .HasColumnType("int");

                    b.Property<string>("SignatureImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SignedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SignerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PetitionId");

                    b.HasIndex("SignerId");

                    b.ToTable("ESignature");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Home", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("InternalAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<int?>("ResidentialCommunityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ResidentialCommunityId");

                    b.ToTable("Home");
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("HomeId")
                        .HasColumnType("int");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<double>("OwnershipPercentage")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("HomeId");

                    b.HasIndex("OwnerId");

                    b.ToTable("HomeOwnership");
                });

            modelBuilder.Entity("PluginDemocracy.Models.JoinCommunityRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ApprovalDecisionMadeById")
                        .HasColumnType("int");

                    b.Property<bool?>("Approved")
                        .HasColumnType("bit");

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateOfApprovalDecision")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateRequested")
                        .HasColumnType("datetime2");

                    b.Property<int>("HomeId")
                        .HasColumnType("int");

                    b.Property<bool>("JoiningAsOwner")
                        .HasColumnType("bit");

                    b.Property<bool>("JoiningAsResident")
                        .HasColumnType("bit");

                    b.Property<string>("LinksToFiles")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("OwnershipPercentage")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApprovalDecisionMadeById");

                    b.HasIndex("CommunityId");

                    b.HasIndex("HomeId");

                    b.HasIndex("UserId");

                    b.ToTable("JoinCommunityRequests");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("JoinCommunityRequestId")
                        .HasColumnType("int");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JoinCommunityRequestId");

                    b.HasIndex("SenderId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Read")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Petition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionRequested")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeadlineForResponse")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("LinksToSupportingDocuments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Published")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ResidentialCommunityId")
                        .HasColumnType("int");

                    b.Property<int?>("ResidentialCommunityId1")
                        .HasColumnType("int");

                    b.Property<string>("SupportingArguments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("ResidentialCommunityId");

                    b.HasIndex("ResidentialCommunityId1");

                    b.ToTable("Petitions");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagesLinks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LatestActivity")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ResidentialCommunityId")
                        .HasColumnType("int");

                    b.Property<int?>("ResidentialCommunityId1")
                        .HasColumnType("int");

                    b.Property<int?>("ResidentialCommunityId2")
                        .HasColumnType("int");

                    b.Property<int?>("_communityAuthorId")
                        .HasColumnType("int");

                    b.Property<int?>("_userAuthorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ResidentialCommunityId");

                    b.HasIndex("ResidentialCommunityId1");

                    b.HasIndex("ResidentialCommunityId2");

                    b.HasIndex("_communityAuthorId");

                    b.HasIndex("_userAuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PluginDemocracy.Models.PostComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("PostComment");
                });

            modelBuilder.Entity("PluginDemocracy.Models.PostReaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PostId")
                        .HasColumnType("int");

                    b.Property<int>("ReactionType")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("PostReaction");
                });

            modelBuilder.Entity("PluginDemocracy.Models.ResidentialCommunity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficialCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("_officialLanguagesCodes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ResidentialCommunities");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("HolderId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ComplexProperty<Dictionary<string, object>>("Powers", "PluginDemocracy.Models.Role.Powers#RolePowers", b1 =>
                        {
                            b1.Property<bool>("CanEditHomeOwnership")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanEditResidency")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanModifyAccounting")
                                .HasColumnType("bit");
                        });

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("HolderId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Admin")
                        .HasColumnType("bit");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EmailConfirmationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondLastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HomeUser", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Home", null)
                        .WithMany()
                        .HasForeignKey("ResidentOfHomesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany()
                        .HasForeignKey("ResidentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PetitionReadyToPublishAuthor", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Petition", null)
                        .WithMany()
                        .HasForeignKey("PetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PetitionUser", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany()
                        .HasForeignKey("AuthorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.Petition", null)
                        .WithMany()
                        .HasForeignKey("PetitionDraftsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PluginDemocracy.Models.ESignature", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Petition", "Petition")
                        .WithMany("Signatures")
                        .HasForeignKey("PetitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", "Signer")
                        .WithMany()
                        .HasForeignKey("SignerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Petition");

                    b.Navigation("Signer");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Home", b =>
                {
                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", "ResidentialCommunity")
                        .WithMany("Homes")
                        .HasForeignKey("ResidentialCommunityId");

                    b.Navigation("ResidentialCommunity");
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnership", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Home", "Home")
                        .WithMany("Ownerships")
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", "Owner")
                        .WithMany("HomeOwnerships")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Home");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PluginDemocracy.Models.JoinCommunityRequest", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", "ApprovalDecisionMadeBy")
                        .WithMany()
                        .HasForeignKey("ApprovalDecisionMadeById");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", "Community")
                        .WithMany("JoinCommunityRequests")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.Home", "Home")
                        .WithMany()
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApprovalDecisionMadeBy");

                    b.Navigation("Community");

                    b.Navigation("Home");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Message", b =>
                {
                    b.HasOne("PluginDemocracy.Models.JoinCommunityRequest", null)
                        .WithMany("Messages")
                        .HasForeignKey("JoinCommunityRequestId");

                    b.HasOne("PluginDemocracy.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Notification", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany("Notifications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Petition", b =>
                {
                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", "Community")
                        .WithMany("Petitions")
                        .HasForeignKey("CommunityId");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", null)
                        .WithMany("PetitionsByLatestActivity")
                        .HasForeignKey("ResidentialCommunityId");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", null)
                        .WithMany("PublishedPetitions")
                        .HasForeignKey("ResidentialCommunityId1");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Post", b =>
                {
                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", null)
                        .WithMany("Posts")
                        .HasForeignKey("ResidentialCommunityId");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", null)
                        .WithMany("PostsByLatestActivity")
                        .HasForeignKey("ResidentialCommunityId1");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", null)
                        .WithMany("PostsByPublishedDate")
                        .HasForeignKey("ResidentialCommunityId2");

                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", "_communityAuthor")
                        .WithMany()
                        .HasForeignKey("_communityAuthorId");

                    b.HasOne("PluginDemocracy.Models.User", "_userAuthor")
                        .WithMany()
                        .HasForeignKey("_userAuthorId");

                    b.Navigation("_communityAuthor");

                    b.Navigation("_userAuthor");
                });

            modelBuilder.Entity("PluginDemocracy.Models.PostComment", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("PluginDemocracy.Models.PostReaction", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Post", null)
                        .WithMany("Reactions")
                        .HasForeignKey("PostId");

                    b.HasOne("PluginDemocracy.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Role", b =>
                {
                    b.HasOne("PluginDemocracy.Models.ResidentialCommunity", "Community")
                        .WithMany("Roles")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.User", "Holder")
                        .WithMany("Roles")
                        .HasForeignKey("HolderId");

                    b.Navigation("Community");

                    b.Navigation("Holder");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Home", b =>
                {
                    b.Navigation("Ownerships");
                });

            modelBuilder.Entity("PluginDemocracy.Models.JoinCommunityRequest", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Petition", b =>
                {
                    b.Navigation("Signatures");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("PluginDemocracy.Models.ResidentialCommunity", b =>
                {
                    b.Navigation("Homes");

                    b.Navigation("JoinCommunityRequests");

                    b.Navigation("Petitions");

                    b.Navigation("PetitionsByLatestActivity");

                    b.Navigation("Posts");

                    b.Navigation("PostsByLatestActivity");

                    b.Navigation("PostsByPublishedDate");

                    b.Navigation("PublishedPetitions");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.User", b =>
                {
                    b.Navigation("HomeOwnerships");

                    b.Navigation("Notifications");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
