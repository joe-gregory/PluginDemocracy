using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PluginDemocracyContext>
    {
        public PluginDemocracyContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var connectionString = configuration.GetConnectionString("PluginDemocracyDatabase");

            var optionsBuilder = new DbContextOptionsBuilder<PluginDemocracyContext>();
            optionsBuilder.UseSqlServer(connectionString).EnableDetailedErrors();
            optionsBuilder.EnableDetailedErrors();

            return new PluginDemocracyContext(optionsBuilder.Options);
        }
    }
}
