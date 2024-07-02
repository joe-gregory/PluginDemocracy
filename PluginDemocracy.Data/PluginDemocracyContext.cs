using Microsoft.EntityFrameworkCore;
using PluginDemocracy.Models;

namespace PluginDemocracy.Data
{
    public class PluginDemocracyContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
