using ConsumptionManagerBackend.Database.DatabaseModels;

namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class TariffDetailsDto
    {

        public string DayOfWeekName { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public double PricePerKwh { get; set; }

        public double PricePerKwhAfterLimit { get; set; }

    }
}
