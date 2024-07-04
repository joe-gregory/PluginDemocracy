using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResidentialCommunity> Communities { get; set; }
        public DbSet<JoinCommunityRequest> JoinCommunityRequests { get; set; }
        public DbSet<Petition> Petitions { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// Since EFC has difficulty storing CultureInfo objects, I convert to string when saving and 
            /// convert back to CultureInfo when reading.
            modelBuilder.Entity<User>().Property(u => u.Culture).HasConversion(c => c.Name, s => new(s));

            modelBuilder.Entity<ResidentialCommunity>().Property("_officialLanguagesCodes");
        }
    }
}
