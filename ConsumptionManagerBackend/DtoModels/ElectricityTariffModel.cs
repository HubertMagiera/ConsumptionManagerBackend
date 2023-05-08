using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels
{
    //class indicates for which tariff user wants to get its details
    public class ElectricityTariffModel
    {
        [Required]
        public string EnergySupplierName { get; set; }
        [Required]
        public string ElectricityTariffName { get; set; }
    }
}
