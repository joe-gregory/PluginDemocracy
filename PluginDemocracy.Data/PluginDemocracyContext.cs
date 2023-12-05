using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext : DbContext
    {
        public PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : base(options) { }

        public DbSet<Community> Communities { get; set; }
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

            modelBuilder.Entity<Role>() // Assuming Role is another derived type
                .ToTable("Roles");
           

            modelBuilder.Entity<Transaction>() // Assuming Transaction is a derived type
                .ToTable("Transactions");

            modelBuilder.Entity<Role>().OwnsOne(r => r.Powers);

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

            modelBuilder.Entity<BaseCitizen>()
                .HasMany(b => b.Citizenships)
                .WithMany(c => c.Citizens)
                .UsingEntity(j => j.ToTable("BaseCitizenCommunity")); // Optional: specify the join table name

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Dictamen)
                .WithOne(d => d.Proposal)
                .HasForeignKey<BaseDictamen>(d => d.ProposalId);

            modelBuilder.Entity<Community>()
              .HasMany(c => c.NonResidentialCitizens)
              .WithMany() // Assuming there's no inverse navigation property
              .UsingEntity<Dictionary<string, object>>(
                  "CommunityNonResidentialCitizen",
                  j => j.HasOne<BaseCitizen>().WithMany().HasForeignKey("BaseCitizenId"),
                  j => j.HasOne<Community>().WithMany().HasForeignKey("CommunityId"),
                  j =>
                  {
                      j.HasKey("BaseCitizenId", "CommunityId");
                      j.ToTable("CommunityNonResidentialCitizens");
                  });
        }
    }
}
