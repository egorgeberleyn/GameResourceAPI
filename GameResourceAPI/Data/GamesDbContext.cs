namespace GameResourceAPI.Data
{
    public class GamesDbContext : DbContext
    {
        public GamesDbContext(DbContextOptions<GamesDbContext> options): base(options) { }        
        
        public DbSet<Game> Games { get; set; }
        public DbSet<CompanyDev> CompanyDevs { get; set; }
    }
}
