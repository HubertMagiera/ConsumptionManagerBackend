using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForSearching
{
    public class SearchForUserDeviceDto
    {
        [Required]
        public string DeviceName { get; set; }
        [Required]
        public string DeviceCategory { get; set; }
    }
}
