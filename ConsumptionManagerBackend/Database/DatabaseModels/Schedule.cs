namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class Schedule
    {
        public int schedule_id { get; set; }

        public int measurement_id { get; set; }

        public int schedule_frequency { get; set; }

        public Measurement measurement { get; set; }
    }
}
