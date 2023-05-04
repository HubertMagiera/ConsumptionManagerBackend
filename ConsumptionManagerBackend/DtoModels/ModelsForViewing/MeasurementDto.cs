using System.ComponentModel.DataAnnotations;

namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class MeasurementDto
    {
        public string DeviceName { get; set; }

        public string DeviceCategory { get; set; }

        public double EnergyUsed { get; set; }

        public double TotalPrice { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
