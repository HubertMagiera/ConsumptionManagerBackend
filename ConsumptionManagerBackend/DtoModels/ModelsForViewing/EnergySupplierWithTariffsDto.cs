﻿namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class EnergySupplierWithTariffsDto
    {
        public string EnergySupplierName { get; set; }

        public List<ElectricityTariffWithDetailsDto> tariffs { get; set; }
    }
}
