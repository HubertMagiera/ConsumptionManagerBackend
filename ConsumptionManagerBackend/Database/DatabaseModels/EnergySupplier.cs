namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class EnergySupplier
    {
        public int energy_supplier_id { get; set; }

        public string energy_supplier_name { get; set; }

        public List<ElectricityTariff> tariffs { get; set; }
    }
}
