using ConsumptionManagerBackend.DtoModels.ModelsForViewing;
using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    public class AddUserDeviceDto
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceCategoryName { get; set; }

        [Required]
        public int DeviceMaxPower { get; set; }
    }
}
