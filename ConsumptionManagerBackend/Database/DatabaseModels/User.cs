namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_surname { get; set; }
        public int electricity_tariff_id { get; set; }
        public int user_credentials_id { get; set; }
        public int cheaper_energy_limit { get; set; }

        public UserCredentials user_credentials { get; set; }

        public ElectricityTariff electricity_tariff { get; set; }

        public List<UserDevice>user_devices { get; set; }

        public List<Measurement>measurements { get; set; }

    }
}
