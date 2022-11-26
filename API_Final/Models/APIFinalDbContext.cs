using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using API_Final.Models;

namespace API_Final.Models
{
    public class APIFinalDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public APIFinalDbContext(DbContextOptions<APIFinalDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("MatchmakingGameService");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
    }
}
