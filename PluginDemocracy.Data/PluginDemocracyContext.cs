using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext : DbContext
    {
        public PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : base(options) { }
        public DbSet<Community> Communities { get; set; }
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

            modelBuilder.Entity<Community>()
                .ToTable("Communities");

            //modelBuilder.Entity<Community>()
            //    .HasMany(typeof(User), "_userNonResidentialCitizens")
            //    .WithMany("NonResidentialCitizenIn");

            //modelBuilder.Entity<Community>()
            //    .HasMany(typeof(Community), "_communityNonResidentialCitizens")
            //    .WithMany("NonResidentialCitizenIn");

            modelBuilder.Entity<Home>()
                .ToTable("Homes");

            // Configure TPH for Community

            //modelBuilder.Entity<Community>()
            //    .HasDiscriminator<string>("CommunityType")
            //    .HasValue<Community>("Community")
            //    .HasValue<Home>("Home");

            modelBuilder.Entity<Community>()
                 .HasMany(c => c.Homes)
                 .WithOne(h => h.ParentCommunity);

            //modelBuilder.Entity<Home>()
            //    .Ignore(h => h.Citizenships)

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
                .HasOne(p => p.Community) // Navigation property in Project
                .WithMany(c => c.Projects); // Navigation property in Community

            //Configure TPH for BaseDictamen
            modelBuilder.Entity<BaseDictamen>()
                .HasDiscriminator<string>("DictamenType")
                .HasValue<PropagatedVoteDictamen>("PropagatedVoteDictamen")
                .HasValue<ProposalWithDifferentVotingStrategyDictamen>("ProposalWithDifferentVotingStrategyDictamen");

            //Configure TPH for BaseVotingStrategy
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

        }
    }
}
