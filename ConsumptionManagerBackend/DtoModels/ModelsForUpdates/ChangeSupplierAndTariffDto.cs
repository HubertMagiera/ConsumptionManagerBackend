using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForUpdates
{
    public class ChangeSupplierAndTariffDto
    {
        [Required]
        public string SupplierName { get; set; }

        [Required]
        public string TariffName { get; set; }
    }
}
