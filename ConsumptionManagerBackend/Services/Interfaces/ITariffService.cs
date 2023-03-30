using ConsumptionManagerBackend.DtoModels;
using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services.Interfaces
{
    public interface ITariffService
    {
        List<EnergySupplierDto> GetEnergySuppliers();

        List<ElectricityTariffDto> GetElectricityTariffsForEnergySupplier(string energySupplierName);

        List<ElectricityTariffWithDetailsDto> GetDetailsForElectricityTariff(ElectricityTariffModel tariff);
    }
}
