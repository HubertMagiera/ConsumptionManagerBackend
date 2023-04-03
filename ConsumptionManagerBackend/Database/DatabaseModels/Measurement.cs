namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class Measurement
    {
        public int measurement_id { get; set; }

        public int user_id { get; set; }

        public int user_device_id { get; set; }

        public int energy_used { get; set; }

        public DateTime measurement_start_date { get; set; }

        public DateTime measurement_end_date { get; set; }

        public double price_of_used_energy { get; set; }

        public User user { get; set; }

        public UserDevice userDevice { get; set; }

        public Schedule schedule { get; set; }
    }
}
