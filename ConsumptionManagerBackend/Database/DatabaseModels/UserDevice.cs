namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class UserDevice
    {
        public int user_device_id { get; set; }

        public int device_id { get; set; }

        public int user_id { get; set; }

        public bool is_active { get; set; }

        public Device device { get; set; }

        public User user { get; set; }
    }
}
