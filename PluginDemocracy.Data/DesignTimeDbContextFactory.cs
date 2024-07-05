using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PluginDemocracyContext>
    {
        public PluginDemocracyContext CreateDbContext(string[] args)
        {
            // Use the location of the executing assembly to find the base path
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var basePath = Path.GetFullPath(Path.Combine(exePath ?? string.Empty, "..", "..", "..", "..", "PluginDemocracy.API"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
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
