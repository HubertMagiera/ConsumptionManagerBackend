using ConsumptionManagerBackend.Database.DatabaseModels;

namespace ConsumptionManagerBackend.DtoModels.ModelsForViewing
{
    public class TariffDetailsDto
    {

        public string DayOfWeekName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double PricePerKwh { get; set; }

    }
}
