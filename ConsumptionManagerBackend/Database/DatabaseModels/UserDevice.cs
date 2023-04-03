namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class UserDevice
    {
        public int user_device_id { get; set; }

        public int device_id { get; set; }

        public int user_id { get; set; }

        public bool is_active { get; set; }

        public int device_max_power { get; set; }

        public Device device { get; set; }

        public User user { get; set; }

        public List<DeviceDetails> details { get; set; }

        public List<Measurement> measurements { get; set; }
    }
}
