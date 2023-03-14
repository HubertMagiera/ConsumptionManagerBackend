namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class Device
    {
        public int device_id { get; set; }

        public string device_name { get; set; }

        public int device_max_power { get; set; }

        public string device_description { get; set; }

        public int device_category_id { get; set; }

        public DeviceCategory device_category { get; set; }

        public List<DeviceDetails> device_details { get; set; }

        public List<UserDevice> user_devices { get; set; }
    }
}
