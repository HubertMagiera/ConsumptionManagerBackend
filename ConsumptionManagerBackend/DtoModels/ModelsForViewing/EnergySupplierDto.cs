namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class EnergySupplierDto
    {
        public string EnergySupplierName { get; set; }

        public List<ElectricityTariffDto> tariffs { get; set; }
    }
}
