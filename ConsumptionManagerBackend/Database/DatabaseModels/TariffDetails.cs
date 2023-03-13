using System.ComponentModel.DataAnnotations.Schema;

namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class TariffDetails
    {
        public int tariff_details_id { get; set; }

        public int electricity_tariff_id { get; set; }

        public int day_of_week_id { get; set; }

        public DateTime start_time { get; set; }

        public DateTime end_time { get; set;}

        public double price_per_kwh { get; set; }

        //[ForeignKey("day_of_week_id")]
        public DayOfWeek day_of_week { get; set;}

        //[ForeignKey("electricity_tariff_id")]
        public ElectricityTariff electricity_tariff { get; set; }
    }
}
