namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class ElectricityTariff
    {
        public int electricity_tariff_id { get; set; }

        public string tariff_name { get; set; }

        public int energy_supplier_id { get; set; }

        public string tariff_description { get; set; }
    }
}
