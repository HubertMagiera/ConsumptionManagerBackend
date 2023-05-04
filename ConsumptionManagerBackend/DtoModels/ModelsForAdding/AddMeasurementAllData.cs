namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    //class used to register new measurement
    // user does not see this class
    // fields in this class are populated by code
    public class AddMeasurementAllData
    {
        public int measurement_id { get; set; }

        public int user_id { get; set; }

        public int user_device_id { get; set; }

        public double energy_used { get; set; }

        public DateTime measurement_start_date { get; set; }

        public DateTime measurement_end_date { get; set; }

        public DateTime measurement_added_date { get; set; }

        public double price_of_used_energy { get; set; }

        public int device_power_in_mode { get; set; }
    }
}
