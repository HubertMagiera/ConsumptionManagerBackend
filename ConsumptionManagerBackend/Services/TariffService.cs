using ConsumptionManagerBackend.DtoModels.ModelsForViewing;

namespace ConsumptionManagerBackend.Services
{
    public class TariffService : ITariffService
    {
        public List<ElectricityTariffDto> GetElectricityTariffsForEnergySupplier(string energySupplierName)
        {
            throw new NotImplementedException();
        }

        public List<EnergySupplierDto> GetEnergySuppliers()
        {
            throw new NotImplementedException();
        }
    }
}
