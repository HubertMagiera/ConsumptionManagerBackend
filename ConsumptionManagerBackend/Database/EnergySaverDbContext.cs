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
            //defining relationships between classes
            modelBuilder.Entity<UserCredentials>(userCredentials => 
            {
                userCredentials.HasKey(key => key.user_credentials_id);
                userCredentials.Property(cred => cred.user_email).HasMaxLength(50);
                userCredentials.Property(cred => cred.user_email).IsRequired();
                userCredentials.Property(cred => cred.user_password).IsRequired();
            });

            modelBuilder.Entity<User>(u =>
            {
                u.HasOne(usr => usr.user_credentials)
                .WithOne(credentials => credentials.user)
                .HasForeignKey<User>(usr => usr.user_credentials_id);

                u.Property(usr => usr.user_name).HasMaxLength(30);
                u.Property(usr => usr.user_surname).HasMaxLength(30);

                u.Property(usr => usr.user_name).IsRequired();
                u.Property(usr => usr.user_surname).IsRequired();
                u.Property(usr => usr.user_id).IsRequired();
                u.Property(usr => usr.electricity_tariff_id).IsRequired();
                u.Property(usr => usr.user_credentials_id).IsRequired();

                u.HasOne(usr => usr.electricity_tariff)
                .WithMany(tariff => tariff.tariff_users)
                .HasForeignKey(usr => usr.electricity_tariff_id);

                u.HasKey(key => key.user_id);
            });

            modelBuilder.Entity<ElectricityTariff>(et =>
            {
                et.HasKey(key => key.electricity_tariff_id);

                et.HasOne(tariff => tariff.energy_supplier)
                .WithMany(supplier => supplier.tariffs)
                .HasForeignKey(tariff => tariff.energy_supplier_id);

                et.Property(prop => prop.electricity_tariff_id).IsRequired();
                et.Property(prop => prop.tariff_name).IsRequired();
                et.Property(prop => prop.energy_supplier_id).IsRequired();

                et.Property(prop => prop.tariff_description).HasMaxLength(100);
                et.Property(prop => prop.tariff_name).HasMaxLength(30);
            });

            modelBuilder.Entity<DatabaseModels.DayOfWeek>(dow =>
            {
                dow.HasKey(key => key.day_of_week_id);

                dow.Property(prop => prop.day_of_week_id).IsRequired();
                dow.Property(prop => prop.day_name).IsRequired();

                dow.Property(prop => prop.day_name).HasMaxLength(20);
            });

            modelBuilder.Entity<TariffDetails>(td =>
            {
                td.HasKey(key => key.tariff_details_id);

                td.HasOne(details => details.day_of_week)
                .WithMany(day => day.tariffs_for_day)
                .HasForeignKey(details => details.day_of_week_id);

                td.HasOne(details => details.electricity_tariff)
                .WithMany(tariff => tariff.tariff_details)
                .HasForeignKey(details => details.electricity_tariff_id);

                td.Property(prop => prop.tariff_details_id).IsRequired();
                td.Property(prop => prop.electricity_tariff_id).IsRequired();
                td.Property(prop => prop.day_of_week_id).IsRequired();
                td.Property(prop => prop.start_time).IsRequired();
                td.Property(prop => prop.end_time).IsRequired();
                td.Property(prop => prop.price_per_kwh).IsRequired();
            });

            modelBuilder.Entity<EnergySupplier>(es =>
            {
                es.HasKey(key => key.energy_supplier_id);

                es.Property(prop => prop.energy_supplier_id).IsRequired();
                es.Property(prop => prop.energy_supplier_name).IsRequired();

                es.Property(prop => prop.energy_supplier_name).HasMaxLength(30);
            });
        }
    }
}
