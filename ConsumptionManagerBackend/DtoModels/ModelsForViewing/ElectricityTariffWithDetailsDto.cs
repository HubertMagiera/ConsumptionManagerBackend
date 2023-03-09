using ConsumptionManagerBackend.Database.DatabaseModels;

namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class ElectricityTariffWithDetailsDto
    {
        public string TariffName { get; set; }

        public string TariffDescription { get; set; }

        public string EnergySupplierName { get; set; }

        public List<TariffDetailsDto> TariffDetails { get; set; }
    }
}
