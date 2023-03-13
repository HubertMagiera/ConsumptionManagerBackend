namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_surname { get; set; }
        public int electricity_tariff_id { get; set; }
        public int user_credentials_id { get; set; }

        public UserCredentials user_credentials { get; set; }

        public ElectricityTariff electricity_tariff { get; set; }

    }
}
