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
        public DbSet<BaseVotingStrategy> VotingStrategies {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().OwnsOne(r => r.Powers);
        }
       
    }
}
