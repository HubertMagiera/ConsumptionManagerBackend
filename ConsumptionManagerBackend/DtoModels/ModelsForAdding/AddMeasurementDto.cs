using ConsumptionManagerBackend.DtoModels.ModelsForSearching;
using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    //class with data that needs to be provided by a user to sucessfully add new measurement
    public class AddMeasurementDto
    {
        [Required]
        public SearchForUserDeviceDto Device { get; set; }

        [Required]
        public int DeviceModeNumber { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
