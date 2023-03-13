using System.ComponentModel.DataAnnotations.Schema;

namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class ElectricityTariff
    {
        public int electricity_tariff_id { get; set; }

        public string tariff_name { get; set; }

        public int energy_supplier_id { get; set; }

        public string tariff_description { get; set; }

        public EnergySupplier energy_supplier { get; set; }

        public List<TariffDetails> tariff_details { get; set;}

        public List<User> tariff_users { get; set; }
    }
}
