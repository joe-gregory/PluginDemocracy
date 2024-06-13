using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext : DbContext
    {
        public PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : base(options) { }
        public DbSet<Community> Communities { get; set; }
        public DbSet<Petition> Petitions { get; set; }
        public DbSet<ESignature> ESignatures { get; set; }
        public DbSet<JoinCommunityRequest> JoinCommunityRequests { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<HomeOwnership> HomeOwnership { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Accounting> Accounting { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionHistoryItem> TransactionHistory { get; set; }
        public DbSet<Constitution> Constitutions { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<BaseDictamen> Dictamens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }  // This allows direct operations on PostComments
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<RedFlag> RedFlags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<BaseVotingStrategy> VotingStrategies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.PetitionDrafts)
                .WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionAuthors",
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
    );

            modelBuilder.Entity<Community>()
                .ToTable("Communities");

            modelBuilder.Entity<Community>()
                .HasMany(typeof(Post), "Posts")
                .WithOne()
                .HasForeignKey("CommunityId")
                .IsRequired();

            // Petition Start
            modelBuilder.Entity<Petition>()
                .HasMany(p => p.Authors)
                .WithMany(u => u.PetitionDrafts)
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionAuthors",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId")
                );

            modelBuilder.Entity<Petition>()
                .HasMany(p => p.AuthorsReadyToPublish)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionAuthorsReadyToPublish",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId")
                );

            modelBuilder.Entity<Petition>()
                .HasMany(p => p.Signatures)
                .WithOne(s => s.Petition)
                .HasForeignKey("PetitionId"); // Using shadow property

            // Configure the SupportingDocuments as a serialized string
            modelBuilder.Entity<Petition>()
                .Property<string>("LinksToSupportingDocumentsSerialized")
                .HasColumnName("LinksToSupportingDocumentsSerialized");

            modelBuilder.Entity<Petition>()
                .Property(p => p.LastUpdated)
                .HasField("_lastUpdated");

            // Configure the Community relationship
            modelBuilder.Entity<Petition>()
                .HasOne(p => p.Community)
                .WithMany() // Adjust based on your relationship (e.g., .WithMany(c => c.Petitions))
                .HasForeignKey("CommunityId") // Assuming a foreign key named "CommunityId"
                .IsRequired(false); // Make it optional if _community is nullable

            // Use HasField to specify backing fields for other properties
            modelBuilder.Entity<Petition>()
                .Property(p => p.Title)
                .HasField("_title");

            modelBuilder.Entity<Petition>()
                .Property(p => p.Description)
                .HasField("_description");

            modelBuilder.Entity<Petition>()
                .Property(p => p.ActionRequested)
                .HasField("_actionRequested");

            modelBuilder.Entity<Petition>()
                .Property(p => p.SupportingArguments)
                .HasField("_supportingArguments");

            modelBuilder.Entity<Petition>()
                .Property(p => p.DeadlineForResponse)
                .HasField("_deadlineForResponse");

            modelBuilder.Entity<Petition>()
                .Navigation(p => p.Signatures)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Petition End

            modelBuilder.Entity<HomeOwnership>()
                .HasOne(ho => ho.Home)
                .WithMany(h => h.Ownerships)
                .HasForeignKey(ho => ho.HomeId);

            modelBuilder.Entity<HomeOwnership>()
                .HasOne(ho => ho._userOwner)
                .WithMany()
                .HasForeignKey("UserId");

            modelBuilder.Entity<HomeOwnership>()
                .HasOne(ho => ho._communityOwner)
                .WithMany()
                .HasForeignKey("CommunityId");

            modelBuilder.Entity<Home>()
                .ToTable("Homes");

            modelBuilder.Entity<Community>()
                .HasMany(c => c.Homes)
                .WithOne(h => h.ParentCommunity);

            modelBuilder.Entity<HomeOwnership>()
                .HasOne(ho => ho.Home)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>()
                .ToTable("Roles");

            modelBuilder.Entity<Role>().OwnsOne(r => r.Powers);

            modelBuilder.Entity<Transaction>()
                .ToTable("Transactions");

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Community)
                .WithMany(c => c.Projects);

            modelBuilder.Entity<BaseDictamen>()
                .HasDiscriminator<string>("DictamenType")
                .HasValue<PropagatedVoteDictamen>("PropagatedVoteDictamen")
                .HasValue<ProposalWithDifferentVotingStrategyDictamen>("ProposalWithDifferentVotingStrategyDictamen");

            modelBuilder.Entity<BaseVotingStrategy>()
                .HasDiscriminator<string>("VotingStrategyType")
                .HasValue<CitizensVotingStrategy>("CitizensVotingStrategy")
                .HasValue<HomeOwnersFractionalVotingStrategy>("HomeOwnersFractionalVotingStrategy")
                .HasValue<HomeOwnersNonFractionalVotingStrategy>("HomeOwnersNonFractionalVotingStrategy")
                .HasValue<UsersVotingStrategy>("UsersVotingStrategy");

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Reactions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .Property(p => p.FundingGoal)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 6);

            // Configure the Id column for ESignatures to have identity
            modelBuilder.Entity<ESignature>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:Identity", "1, 1");

            // Ensure the PetitionId shadow property is configured correctly
            modelBuilder.Entity<ESignature>()
                .Property<int>("PetitionId");

            modelBuilder.Entity<ESignature>()
                .HasOne(e => e.Petition)
                .WithMany(p => p.Signatures)
                .HasForeignKey("PetitionId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
