using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext : DbContext
    {
        public PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : base(options) { }

        public DbSet<Community> Communities { get; set; }
        public DbSet<HomeOwnership> HomeOwnership { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Accounting> Accounting { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionHistoryItem> TransactionHistory { get; set; }
        public DbSet<Constitution> Constitutions { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<BaseDictamen> Dictamens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<RedFlag> RedFlags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<BaseVotingStrategy> VotingStrategies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Community>()
                .ToTable("Communities");

            // Configure the many-to-many relationship between BaseCitizen and Community
            modelBuilder.Entity<BaseCitizen>()
                .HasMany(b => b.Citizenships)
                .WithMany() // No inverse collection specified in Community since Community.Citizenships is a computed property
                .UsingEntity<Dictionary<string, object>>(
                    "BaseCitizenCommunity",
                    b => b.HasOne<Community>().WithMany().HasForeignKey("CommunityId"),
                    c => c.HasOne<BaseCitizen>().WithMany().HasForeignKey("BaseCitizenId"));

            modelBuilder.Entity<Role>()
                .ToTable("Roles");

            modelBuilder.Entity<Role>().OwnsOne(r => r.Powers);

            modelBuilder.Entity<Transaction>()
                .ToTable("Transactions");

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Community) // Navigation property in Project
                .WithMany(c => c.Projects) // Navigation property in Community
                .HasForeignKey(p => p.CommunityId); // Foreign key in Project

            // Configure TPH for BaseDictamen
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

            // Specify precision for decimal properties
            modelBuilder.Entity<Project>()
                .Property(p => p.FundingGoal)
                .HasPrecision(18, 6); // Change precision and scale as needed

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 6);

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Dictamen)
                .WithOne(d => d.Proposal)
                .HasForeignKey<BaseDictamen>(d => d.ProposalId);
        }
    }
}
