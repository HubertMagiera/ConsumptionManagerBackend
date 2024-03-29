﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ConsumptionManagerBackend.Database.DatabaseModels
{
    public class TariffDetails
    {
        public int tariff_details_id { get; set; }

        public int electricity_tariff_id { get; set; }

        public int day_of_week_id { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        public double price_per_kwh { get; set; }

        public double price_per_kwh_after_limit { get; set; }

        public DayOfWeek day_of_week { get; set;}

        public ElectricityTariff electricity_tariff { get; set; }
    }
}
