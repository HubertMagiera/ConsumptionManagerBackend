using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    public class AddUserDeviceDetailsDto
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceCategory { get; set; }

        [Required]
        public int ModeNumber { get; set; }

        public string DeviceModeDescription { get; set; }

        [Required]
        public int DevicePowerInMode { get; set; }
    }
}
