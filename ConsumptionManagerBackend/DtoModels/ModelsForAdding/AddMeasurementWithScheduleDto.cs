using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForAdding
{
    public class AddMeasurementWithScheduleDto
    {
        [Required]
        public AddMeasurementDto Measurement { get; set; }

        [Required]
        public int ScheduleFrequency { get; set; }
    }
}
