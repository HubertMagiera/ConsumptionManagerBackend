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
        public DbSet<DeviceDetails>device_details { get; set; }
        public DbSet<Device> device { get; set; }
        public DbSet<DeviceCategory>device_category { get; set; }
        public DbSet<UserDevice>user_device { get; set; }
        public DbSet<Measurement> measurement { get; set; }
        public DbSet<Schedule>schedule { get; set; }

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
                u.Property(usr => usr.cheaper_energy_limit).IsRequired();

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
                td.Property(prop => prop.price_per_kwh_after_limit).IsRequired();
            });

            modelBuilder.Entity<EnergySupplier>(es =>
            {
                es.HasKey(key => key.energy_supplier_id);

                es.Property(prop => prop.energy_supplier_id).IsRequired();
                es.Property(prop => prop.energy_supplier_name).IsRequired();

                es.Property(prop => prop.energy_supplier_name).HasMaxLength(30);
            });

            modelBuilder.Entity<DeviceCategory>(dc =>
            {
                dc.HasKey(key => key.device_category_id);

                dc.Property(property => property.device_category_id).IsRequired();
                dc.Property(property => property.device_category_name).IsRequired();

                dc.Property(property => property.device_category_name).HasMaxLength(50);
            });

            modelBuilder.Entity<DeviceDetails>(dd =>
            {
                dd.HasKey(key => key.device_details_id);

                dd.Property(property => property.device_details_id).IsRequired();
                dd.Property(property => property.user_device_id).IsRequired();
                dd.Property(property => property.device_mode_number).IsRequired();
                dd.Property(property => property.device_power_in_mode).IsRequired();

                dd.Property(property => property.device_details_id).HasMaxLength(50);

                dd.HasOne(property => property.user_device)
                    .WithMany(device => device.details)
                    .HasForeignKey(property => property.user_device_id);
            });

            modelBuilder.Entity<Device>(dev =>
            {
                dev.HasKey(key => key.device_id);

                dev.Property(property => property.device_id).IsRequired();
                dev.Property(property => property.device_name).IsRequired();
                dev.Property(property => property.device_category_id).IsRequired();

                dev.Property(property => property.device_name).HasMaxLength(100);
                dev.Property(property => property.device_description).HasMaxLength(1000);

                dev.HasOne(property => property.device_category)
                    .WithMany(category => category.devices_for_category)
                    .HasForeignKey(property => property.device_category_id);
            });

            modelBuilder.Entity<UserDevice>(ud =>
            {
                ud.HasKey(key => key.user_device_id);

                ud.Property(property => property.user_device_id).IsRequired();
                ud.Property(property => property.device_id).IsRequired();
                ud.Property(property => property.is_active).IsRequired();
                ud.Property(property => property.user_id).IsRequired();
                ud.Property(property => property.device_max_power).IsRequired();

                ud.HasOne(property => property.user)
                    .WithMany(usr => usr.user_devices)
                    .HasForeignKey(property => property.user_id);

                ud.HasOne(property => property.device)
                    .WithMany(device => device.user_devices)
                    .HasForeignKey(property => property.device_id);
            });

            modelBuilder.Entity<Measurement>(m =>
            {
                m.HasKey(property => property.measurement_id);

                m.Property(property => property.measurement_id).IsRequired();
                m.Property(property => property.user_id).IsRequired();
                m.Property(property => property.user_device_id).IsRequired();
                m.Property(property => property.measurement_start_date).IsRequired();
                m.Property(property => property.measurement_end_date).IsRequired();
                m.Property(property => property.energy_used).IsRequired();
                m.Property(property => property.price_of_used_energy).IsRequired();
                m.Property(property => property.measurement_added_date).IsRequired();

                m.HasOne(property => property.user)
                    .WithMany(usr => usr.measurements)
                    .HasForeignKey(property => property.user_id);

                m.HasOne(property => property.userDevice)
                    .WithMany(ud => ud.measurements)
                    .HasForeignKey(property => property.user_device_id);
            });

            modelBuilder.Entity<Schedule>(s =>
            {
                s.HasKey(property => property.schedule_id);

                s.Property(property => property.schedule_id).IsRequired();
                s.Property(property => property.measurement_id).IsRequired();
                s.Property(property => property.schedule_frequency).IsRequired();

                s.HasOne(property => property.measurement)
                    .WithOne(m => m.schedule)
                    .HasForeignKey<Schedule>(property => property.measurement_id);
            });
        }
    }
}
