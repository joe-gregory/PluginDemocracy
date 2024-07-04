using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResidentialCommunity> ResidentialCommunities { get; set; }
        public DbSet<JoinCommunityRequest> JoinCommunityRequests { get; set; }
        public DbSet<Petition> Petitions { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// Since EFC has difficulty storing CultureInfo objects, I convert to string when saving and 
            /// convert back to CultureInfo when reading.
            modelBuilder.Entity<User>().Property(u => u.Culture).HasConversion(c => c.Name, s => new(s));

            modelBuilder.Entity<ResidentialCommunity>().Ignore(ResidentialCommunity => ResidentialCommunity.OfficialLanguages);
            modelBuilder.Entity<ResidentialCommunity>().Property("_officialLanguagesCodes");
            modelBuilder.Entity<ResidentialCommunity>().Ignore(residentialCommunity => residentialCommunity.Citizens);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(rc => rc.HomeOwnerships);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(rc => rc.HomeOwners);

            modelBuilder.Entity<Home>().Ignore(Home => Home.Citizens);
            modelBuilder.Entity<Home>().Ignore(Home => Home.Owners);

            // Configure the many-to-many relationship between Petition and User
            modelBuilder.Entity<Petition>()
                .HasMany(p => p.Authors)
                .WithMany(u => u.PetitionDrafts)
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionAuthor", // Name of the join table
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId"));
            // Configure the many-to-many relationship between Petition and User for AuthorsReadyToPublish
            modelBuilder.Entity<Petition>()
                .HasMany(p => p.AuthorsReadyToPublish)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionReadyToPublishAuthor",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId"));
            modelBuilder.Entity<Petition>()
                .HasOne(p => p.Community)
                .WithMany(c => c.Petitions);
            modelBuilder.Entity<Petition>().Property("_linksToSupportingDocuments");

            modelBuilder.Entity<Post>().Ignore(post => post.Author);
            modelBuilder.Entity<Post>().HasOne("CommunityAuthor").WithMany("Posts");
            modelBuilder.Entity<Post>().HasOne("UserAuthor");
            modelBuilder.Entity<Post>().Property("_imagesLinks");

        }
    }
}
