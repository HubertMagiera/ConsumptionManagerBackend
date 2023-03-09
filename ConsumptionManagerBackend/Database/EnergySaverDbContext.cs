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
        public DbSet<User> user { get; set; }
        public DbSet<ElectricityTariff> electricity_tariff { get; set; }
        public DbSet<DatabaseModels.DayOfWeek> day_of_week { get; set; }
        public DbSet<TariffDetails> tariff_details { get; set; }
        public DbSet<EnergySupplier> energy_supplier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //specifying that entities have primary keys and indicating them
            modelBuilder.Entity<UserCredentials>().HasKey(key => key.user_credentials_id);

            modelBuilder.Entity<User>().HasKey(key => key.user_id);

            modelBuilder.Entity<ElectricityTariff>().HasKey(key => key.electricity_tariff_id);

            modelBuilder.Entity<DatabaseModels.DayOfWeek>().HasKey(key => key.day_of_week_id);

            modelBuilder.Entity<TariffDetails>().HasKey(key => key.tariff_details_id);

            modelBuilder.Entity<EnergySupplier>().HasKey(key => key.energy_supplier_id);
        }
    }
}
