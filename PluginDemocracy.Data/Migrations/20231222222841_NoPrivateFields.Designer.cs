﻿// <auto-generated />
using System;
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
    [Migration("20231222222841_NoPrivateFields")]
    partial class NoPrivateFields
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

            modelBuilder.Entity("PluginDemocracy.Models.Accounting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.ToTable("Accounting");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ConstitutionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Number")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConstitutionId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.BaseDictamen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DictamenType")
                        .IsRequired()
                        .HasMaxLength(55)
                        .HasColumnType("nvarchar(55)");

                    b.Property<DateTime?>("IssueDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProposalId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("ProposalId")
                        .IsUnique();

                    b.ToTable("Dictamens");

                    b.HasDiscriminator<string>("DictamenType").HasValue("BaseDictamen");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("PluginDemocracy.Models.BaseRedFlaggable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("BaseRedFlaggable");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("PluginDemocracy.Models.BaseVotingStrategy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("VotingStrategyType")
                        .IsRequired()
                        .HasMaxLength(55)
                        .HasColumnType("nvarchar(55)");

                    b.HasKey("Id");

                    b.ToTable("VotingStrategies");

                    b.HasDiscriminator<string>("VotingStrategyType").HasValue("BaseVotingStrategy");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("PluginDemocracy.Models.Community", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("CanHaveHomes")
                        .HasColumnType("bit");

                    b.Property<bool>("CanHaveNonResidentialCitizens")
                        .HasColumnType("bit");

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<int>("ConstitutionId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HomeId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficialCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OfficialLanguages")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProposalsExpirationDays")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VotingStrategyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("ConstitutionId");

                    b.HasIndex("HomeId");

                    b.HasIndex("UserId");

                    b.HasIndex("VotingStrategyId");

                    b.ToTable("Communities", (string)null);
                });

            modelBuilder.Entity("PluginDemocracy.Models.Constitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Preamble")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Constitutions");
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

                    b.Property<int?>("ParentCommunityId")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePicture")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ParentCommunityId");

                    b.ToTable("Homes", (string)null);
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<int>("HomeId")
                        .HasColumnType("int");

                    b.Property<int?>("HomeId1")
                        .HasColumnType("int");

                    b.Property<int?>("HomeId2")
                        .HasColumnType("int");

                    b.Property<double>("OwnershipPercentage")
                        .HasColumnType("float");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("HomeId");

                    b.HasIndex("HomeId1");

                    b.HasIndex("HomeId2");

                    b.HasIndex("UserId");

                    b.ToTable("HomeOwnership");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommunityId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("FundingGoal")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Proposal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ClosedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Open")
                        .HasColumnType("bit");

                    b.Property<bool?>("Passed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("PublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VotingStrategyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("VotingStrategyId");

                    b.ToTable("Proposal");
                });

            modelBuilder.Entity("PluginDemocracy.Models.RedFlag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ItemFlaggedId")
                        .HasColumnType("int");

                    b.Property<int?>("ProposalId")
                        .HasColumnType("int");

                    b.Property<bool>("Resolved")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("ItemFlaggedId");

                    b.HasIndex("ProposalId");

                    b.ToTable("RedFlags");
                });

            modelBuilder.Entity("PluginDemocracy.Models.TransactionHistoryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountantId")
                        .HasColumnType("int");

                    b.Property<int>("Action")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DictamenId")
                        .HasColumnType("int");

                    b.Property<int?>("TransactionSnapShotId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountantId");

                    b.HasIndex("DictamenId");

                    b.HasIndex("TransactionSnapShotId");

                    b.ToTable("TransactionHistory");
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

                    b.Property<int?>("RedFlagId")
                        .HasColumnType("int");

                    b.Property<string>("SecondLastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("_cultureCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RedFlagId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("PluginDemocracy.Models.PropagatedVoteDictamen", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseDictamen");

                    b.Property<int?>("ParentProposalId")
                        .HasColumnType("int");

                    b.HasIndex("ParentProposalId");

                    b.HasDiscriminator().HasValue("PropagatedVoteDictamen");
                });

            modelBuilder.Entity("PluginDemocracy.Models.ProposalWithDifferentVotingStrategyDictamen", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseDictamen");

                    b.Property<int?>("ProposalToCreateId")
                        .HasColumnType("int");

                    b.HasIndex("ProposalToCreateId");

                    b.HasDiscriminator().HasValue("ProposalWithDifferentVotingStrategyDictamen");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Role", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseRedFlaggable");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<int?>("AssigneeId")
                        .HasColumnType("int");

                    b.Property<int?>("CommunityId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("CommunityId");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("PluginDemocracy.Models.Transaction", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseRedFlaggable");

                    b.Property<int?>("AccountingId")
                        .HasColumnType("int");

                    b.Property<decimal?>("Amount")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("AccountingId");

                    b.ToTable("Transactions", (string)null);
                });

            modelBuilder.Entity("PluginDemocracy.Models.CitizensVotingStrategy", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseVotingStrategy");

                    b.HasDiscriminator().HasValue("CitizensVotingStrategy");
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnersFractionalVotingStrategy", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseVotingStrategy");

                    b.HasDiscriminator().HasValue("HomeOwnersFractionalVotingStrategy");
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnersNonFractionalVotingStrategy", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseVotingStrategy");

                    b.HasDiscriminator().HasValue("HomeOwnersNonFractionalVotingStrategy");
                });

            modelBuilder.Entity("PluginDemocracy.Models.UsersVotingStrategy", b =>
                {
                    b.HasBaseType("PluginDemocracy.Models.BaseVotingStrategy");

                    b.Property<int?>("MaximumAge")
                        .HasColumnType("int");

                    b.Property<int?>("MinimumAge")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("UsersVotingStrategy");
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

            modelBuilder.Entity("PluginDemocracy.Models.Accounting", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "Community")
                        .WithMany()
                        .HasForeignKey("CommunityId");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Article", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Constitution", null)
                        .WithMany("Articles")
                        .HasForeignKey("ConstitutionId");
                });

            modelBuilder.Entity("PluginDemocracy.Models.BaseDictamen", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "Community")
                        .WithMany("Dictamens")
                        .HasForeignKey("CommunityId");

                    b.HasOne("PluginDemocracy.Models.Proposal", "Proposal")
                        .WithOne("Dictamen")
                        .HasForeignKey("PluginDemocracy.Models.BaseDictamen", "ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Community", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", null)
                        .WithMany("NonResidentialCitizenIn")
                        .HasForeignKey("CommunityId");

                    b.HasOne("PluginDemocracy.Models.Constitution", "Constitution")
                        .WithMany()
                        .HasForeignKey("ConstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.Home", null)
                        .WithMany("NonResidentialCitizenIn")
                        .HasForeignKey("HomeId");

                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany("NonResidentialCitizenIn")
                        .HasForeignKey("UserId");

                    b.HasOne("PluginDemocracy.Models.BaseVotingStrategy", "VotingStrategy")
                        .WithMany()
                        .HasForeignKey("VotingStrategyId");

                    b.Navigation("Constitution");

                    b.Navigation("VotingStrategy");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Home", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "ParentCommunity")
                        .WithMany("Homes")
                        .HasForeignKey("ParentCommunityId");

                    b.Navigation("ParentCommunity");
                });

            modelBuilder.Entity("PluginDemocracy.Models.HomeOwnership", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", null)
                        .WithMany("HomeOwnerships")
                        .HasForeignKey("CommunityId");

                    b.HasOne("PluginDemocracy.Models.Home", "Home")
                        .WithMany()
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.Home", null)
                        .WithMany("HomeOwnerships")
                        .HasForeignKey("HomeId1");

                    b.HasOne("PluginDemocracy.Models.Home", null)
                        .WithMany("Ownerships")
                        .HasForeignKey("HomeId2");

                    b.HasOne("PluginDemocracy.Models.User", null)
                        .WithMany("HomeOwnerships")
                        .HasForeignKey("UserId");

                    b.Navigation("Home");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Post", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("PluginDemocracy.Models.Community", null)
                        .WithMany("Posts")
                        .HasForeignKey("CommunityId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Project", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "Community")
                        .WithMany("Projects")
                        .HasForeignKey("CommunityId");

                    b.Navigation("Community");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Proposal", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "Community")
                        .WithMany("Proposals")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.BaseVotingStrategy", "VotingStrategy")
                        .WithMany()
                        .HasForeignKey("VotingStrategyId");

                    b.Navigation("Community");

                    b.Navigation("VotingStrategy");
                });

            modelBuilder.Entity("PluginDemocracy.Models.RedFlag", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Community", "Community")
                        .WithMany("RedFlags")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PluginDemocracy.Models.BaseRedFlaggable", "ItemFlagged")
                        .WithMany("RedFlags")
                        .HasForeignKey("ItemFlaggedId");

                    b.HasOne("PluginDemocracy.Models.Proposal", null)
                        .WithMany("AddressesRedFlags")
                        .HasForeignKey("ProposalId");

                    b.Navigation("Community");

                    b.Navigation("ItemFlagged");
                });

            modelBuilder.Entity("PluginDemocracy.Models.TransactionHistoryItem", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", "Accountant")
                        .WithMany()
                        .HasForeignKey("AccountantId");

                    b.HasOne("PluginDemocracy.Models.BaseDictamen", "Dictamen")
                        .WithMany()
                        .HasForeignKey("DictamenId");

                    b.HasOne("PluginDemocracy.Models.Transaction", "TransactionSnapShot")
                        .WithMany("History")
                        .HasForeignKey("TransactionSnapShotId");

                    b.Navigation("Accountant");

                    b.Navigation("Dictamen");

                    b.Navigation("TransactionSnapShot");
                });

            modelBuilder.Entity("PluginDemocracy.Models.User", b =>
                {
                    b.HasOne("PluginDemocracy.Models.RedFlag", null)
                        .WithMany("Users")
                        .HasForeignKey("RedFlagId");
                });

            modelBuilder.Entity("PluginDemocracy.Models.PropagatedVoteDictamen", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Proposal", "ParentProposal")
                        .WithMany()
                        .HasForeignKey("ParentProposalId");

                    b.Navigation("ParentProposal");
                });

            modelBuilder.Entity("PluginDemocracy.Models.ProposalWithDifferentVotingStrategyDictamen", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Proposal", "ProposalToCreate")
                        .WithMany()
                        .HasForeignKey("ProposalToCreateId");

                    b.Navigation("ProposalToCreate");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Role", b =>
                {
                    b.HasOne("PluginDemocracy.Models.User", "Assignee")
                        .WithMany("Roles")
                        .HasForeignKey("AssigneeId");

                    b.HasOne("PluginDemocracy.Models.Community", null)
                        .WithMany("Roles")
                        .HasForeignKey("CommunityId");

                    b.HasOne("PluginDemocracy.Models.BaseRedFlaggable", null)
                        .WithOne()
                        .HasForeignKey("PluginDemocracy.Models.Role", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("PluginDemocracy.Models.RolePowers", "Powers", b1 =>
                        {
                            b1.Property<int>("RoleId")
                                .HasColumnType("int");

                            b1.Property<bool>("CanAccounting")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanCreateRole")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanJoinCitizen")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanJoinResident")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanRemoveCitizen")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanRemoveResident")
                                .HasColumnType("bit");

                            b1.Property<bool>("CanVerifyHomeOwnership")
                                .HasColumnType("bit");

                            b1.HasKey("RoleId");

                            b1.ToTable("Roles");

                            b1.WithOwner()
                                .HasForeignKey("RoleId");
                        });

                    b.Navigation("Assignee");

                    b.Navigation("Powers")
                        .IsRequired();
                });

            modelBuilder.Entity("PluginDemocracy.Models.Transaction", b =>
                {
                    b.HasOne("PluginDemocracy.Models.Accounting", null)
                        .WithMany("Transactions")
                        .HasForeignKey("AccountingId");

                    b.HasOne("PluginDemocracy.Models.BaseRedFlaggable", null)
                        .WithOne()
                        .HasForeignKey("PluginDemocracy.Models.Transaction", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PluginDemocracy.Models.Accounting", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("PluginDemocracy.Models.BaseRedFlaggable", b =>
                {
                    b.Navigation("RedFlags");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Community", b =>
                {
                    b.Navigation("Dictamens");

                    b.Navigation("HomeOwnerships");

                    b.Navigation("Homes");

                    b.Navigation("NonResidentialCitizenIn");

                    b.Navigation("Posts");

                    b.Navigation("Projects");

                    b.Navigation("Proposals");

                    b.Navigation("RedFlags");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Constitution", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Home", b =>
                {
                    b.Navigation("HomeOwnerships");

                    b.Navigation("NonResidentialCitizenIn");

                    b.Navigation("Ownerships");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Proposal", b =>
                {
                    b.Navigation("AddressesRedFlags");

                    b.Navigation("Dictamen");
                });

            modelBuilder.Entity("PluginDemocracy.Models.RedFlag", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("PluginDemocracy.Models.User", b =>
                {
                    b.Navigation("HomeOwnerships");

                    b.Navigation("NonResidentialCitizenIn");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("PluginDemocracy.Models.Transaction", b =>
                {
                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
