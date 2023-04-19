using ConsumptionManagerBackend.Database.DatabaseModels;

namespace ConsumptionManagerBackend.DtoModels
{
    public class MeasurementsWithSchedule
    {
        public Measurement Measurement { get; set; }

        public Schedule Schedule { get; set; }
    }
}
