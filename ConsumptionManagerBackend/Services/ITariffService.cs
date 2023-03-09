using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services
{
    public interface ITariffService
    {
        List<EnergySupplierDto> GetEnergySuppliers();

        List<ElectricityTariffDto> GetElectricityTariffsForEnergySupplier(string energySupplierName);
    }
}
