using ConsumptionManagerBackend.Database.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace ConsumptionManagerBackend.Database
{
    public class EnergySaverDbContext:DbContext
    {
        public EnergySaverDbContext(DbContextOptions<EnergySaverDbContext> options) : base(options)
        {

        }
        //this will be mapped to database class
        public DbSet<UserCredentials> user_credentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //specifying that entities have primary keys and indicating them
            modelBuilder.Entity<UserCredentials>().HasKey(key => key.user_credentials_id);
        }
    }
}
