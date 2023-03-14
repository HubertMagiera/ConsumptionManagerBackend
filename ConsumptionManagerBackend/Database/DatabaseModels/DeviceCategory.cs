namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class DeviceCategory
    {
        public int device_category_id { get; set; }

        public string device_category_name { get; set;}

        public List<Device> devices_for_category { get; set; }
    }
}
