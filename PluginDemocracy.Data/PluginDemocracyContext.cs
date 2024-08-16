using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;
using System.Globalization;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext(DbContextOptions<PluginDemocracyContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResidentialCommunity> ResidentialCommunities { get; set; }
        public DbSet<JoinCommunityRequest> JoinCommunityRequests { get; set; }
        public DbSet<Petition> Petitions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /// Since EFC has difficulty storing CultureInfo objects, I convert to string when saving and 
            /// convert back to CultureInfo when reading.
            modelBuilder.Entity<User>().Property(u => u.Culture).HasConversion(c => c.Name, s => new(s));
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Ignore(u => u.Citizenships);
            modelBuilder.Entity<User>().HasMany(u => u.HomeOwnerships).WithOne(h => h.Owner);
            modelBuilder.Entity<User>().HasMany(u => u.ResidentOfHomes).WithMany(h => h.Residents);
            modelBuilder.Entity<User>().HasMany(u => u.Roles).WithOne(r => r.Holder);
            modelBuilder.Entity<User>().HasMany(u => u.Notifications).WithOne();
            modelBuilder.Entity<User>().HasMany(u => u.PetitionDrafts).WithMany(p => p.Authors);
            // Apply UsePropertyAccessMode for collections
            modelBuilder.Entity<User>().Navigation(u => u.HomeOwnerships).UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<User>().Navigation(u => u.ResidentOfHomes).UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<User>().Navigation(u => u.Roles).UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<User>().Navigation(u => u.Notifications).UsePropertyAccessMode(PropertyAccessMode.Field);
            modelBuilder.Entity<User>().Navigation(u => u.PetitionDrafts).UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<ResidentialCommunity>().Ignore(ResidentialCommunity => ResidentialCommunity.OfficialLanguages);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(residentialCommunity => residentialCommunity.Citizens);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(rc => rc.HomeOwnerships);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(rc => rc.HomeOwners);
            modelBuilder.Entity<ResidentialCommunity>().Property("_officialLanguagesCodes");
            modelBuilder.Entity<ResidentialCommunity>().HasMany(rc => rc.Posts);
            // Ensure that Roles are deleted when removed from ResidentialCommunity.Roles
            modelBuilder.Entity<ResidentialCommunity>().HasMany(rc => rc.Roles).WithOne(r => r.Community).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ResidentialCommunity>().HasMany(c => c.Homes).WithOne(h => h.ResidentialCommunity);
            modelBuilder.Entity<ResidentialCommunity>().HasMany(c => c.Petitions).WithOne(p => p.Community);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(c => c.PetitionsByLatestActivity);
            modelBuilder.Entity<ResidentialCommunity>().Ignore(c => c.PublishedPetitions);

            modelBuilder.Entity<JoinCommunityRequest>().Property(jcr => jcr.LinksToFiles).HasField("_linksToFiles").UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Home>().Ignore(Home => Home.Citizens);
            modelBuilder.Entity<Home>().Ignore(Home => Home.Owners);
            modelBuilder.Entity<Home>().HasMany(h => h.Ownerships).WithOne(ho => ho.Home).OnDelete(DeleteBehavior.Cascade);

            // Configure the many-to-many relationship between Petition and User for AuthorsReadyToPublish
            modelBuilder.Entity<Petition>()
                .HasMany(p => p.AuthorsReadyToPublish)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PetitionReadyToPublishAuthor",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Petition>().WithMany().HasForeignKey("PetitionId"));
            modelBuilder.Entity<Petition>().HasOne(p => p.Community).WithMany(c => c.Petitions);
            modelBuilder.Entity<Petition>().Property(p => p.LinksToSupportingDocuments).HasField("_linksToSupportingDocuments").UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Post>().Ignore(post => post.Author);
            modelBuilder.Entity<Post>().HasOne("_userAuthor");
            modelBuilder.Entity<Post>().HasOne("_communityAuthor");
            modelBuilder.Entity<Post>().Property(p => p.ImagesLinks).HasField("_imagesLinks").UsePropertyAccessMode(PropertyAccessMode.Field);


            modelBuilder.Entity<Role>().ComplexProperty(r => r.Powers);
        }
    }
}
