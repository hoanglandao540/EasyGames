using System.IO;
using EasyGames.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyGames.Web.Data
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var cs = config.GetConnectionString("Default") ?? "Data Source=EasyGames.db";
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(cs)
                .Options;

            return new AppDbContext(options);
        }
    }
}



