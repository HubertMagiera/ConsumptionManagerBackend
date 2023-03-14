namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class DeviceDetails
    {
        public int device_details_id { get; set; }

        public int user_device_id { get; set; }

        public int device_mode_number { get; set; }

        public int device_power_in_mode { get; set; }

        public string device_mode_description { get; set; }

        public UserDevice user_device { get; set; }
    }
}
