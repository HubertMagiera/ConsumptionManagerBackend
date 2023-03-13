namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class DayOfWeek
    {
        public int day_of_week_id { get; set; }

        public string day_name { get; set; }

        public List<TariffDetails> tariffs_for_day { get; set; }
    }
}
